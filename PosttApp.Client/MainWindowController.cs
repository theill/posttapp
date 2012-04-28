#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.IO;

using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Runtime.Serialization.Json;
using System.Drawing;
using System.Text;

#endregion

namespace com.posttapp {
  public partial class MainWindowController : MonoMac.AppKit.NSWindowController {

		#region Constructors
		
    // Called when created from unmanaged code
    public MainWindowController(IntPtr handle) : base (handle) {
      Initialize();
    }
		
    // Called when created directly from a XIB file
    [Export ("initWithCoder:")]
    public MainWindowController(NSCoder coder) : base (coder) {
      Initialize();
    }
		
    // Call to load from the XIB/NIB file
    public MainWindowController() : base ("MainWindow") {
      Initialize();
    }
		
    // Shared initialization code
    void Initialize() {
    }

		#endregion

    //strongly typed window accessor
    public new MainWindow Window {
      get {
        return (MainWindow)base.Window;
			}
		}

    public override void WindowDidLoad() {
      base.WindowDidLoad();
      InitializeStatusBar();
    }

    void InitializeStatusBar() {
      NSStatusBar sb = NSStatusBar.SystemStatusBar;

      NSMenu menu = new NSMenu();
      NSMenuItem mi = new NSMenuItem("Quit", (sender, e) => {
        Console.WriteLine("We got a click on quit!");

      });
      menu.AddItem(mi);

      NSStatusItem mainItem = sb.CreateStatusItem(24);
      mainItem.Title = "";
      mainItem.Image = NSImage.ImageNamed("cloud.png");
      mainItem.HighlightMode = true;
      mainItem.Menu = menu;
      mainItem.View = new StatusItemView(mainItem, ItemDropped);
      mainItem.View.RegisterForDraggedTypes(new string[] {NSPasteboard.NSFilenamesType, NSPasteboard.NSFileContentsType, NSPasteboard.NSStringType });
    }

    bool IsAuthenticated {
      get {
        return !string.IsNullOrEmpty(Window.Account.AccessToken);
      }
    }

    void ItemDropped(string item) {
      Console.WriteLine("Item dropped: {0}", item);

      if (IsAuthenticated) {
        Window.Gett.CreateShare("test1", Window.Account.AccessToken, item.Replace("file://localhost", "").Replace("%20", " "));
      }
      else {
        Window.Gett.Authenticate(this.Window.Account.Email, this.Window.Account.Password, (account) => {
          // user is logged in

          Window.Gett.CreateShare("test1", account.AccessToken, item.Replace("file://localhost", "").Replace("%20", " "));
        }, (error) => {
          // TODO: handle error message when uploading
        });
      }
    }

  }
}