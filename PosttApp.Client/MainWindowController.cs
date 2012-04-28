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
    private NSMenu menu;

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
      InitializeStatusBar();
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
    }

    void OpenGettWebsite(object sender, EventArgs e) {
      Console.WriteLine("Open ge.tt...");
      // TODO:
    }

    void OpenPreferences(object sender, EventArgs e) {
      Console.WriteLine("Open Preferences...");

      NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
      Window.MakeKeyAndOrderFront(this);
    }

    void QuitApplication(object sender, EventArgs e) {
      Console.WriteLine("Quit application");

      NSApplication.SharedApplication.Terminate(this);
    }

    void InitializeStatusBar() {
      menu = new NSMenu();
      menu.AddItem(new NSMenuItem("Open ge.tt...", OpenGettWebsite));
      menu.AddItem(new NSMenuItem("Preferences...", OpenPreferences));
      menu.AddItem(new NSMenuItem("Quit", QuitApplication));

      // creating the status item with a length of -2 is equivalent to the call
      // [[NSStatusBar systemStatusBar] statusItemWithLength:NSSquareStatusItemLength]
      NSStatusItem mainItem = NSStatusBar.SystemStatusBar.CreateStatusItem(-2);
      mainItem.HighlightMode = true;
      mainItem.Menu = menu;
      mainItem.View = new StatusItemView(mainItem, ItemDropped);
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