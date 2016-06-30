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
    public MainWindowController(IntPtr handle) : base(handle) {
      Initialize();
    }

    // Called when created directly from a XIB file
    [Export("initWithCoder:")]
    public MainWindowController(NSCoder coder) : base(coder) {
      Initialize();
    }

    // Call to load from the XIB/NIB file
    public MainWindowController() : base("MainWindow") {
      Initialize();
    }

    // Shared initialization code
    void Initialize() {
    }

    #endregion

    //strongly typed window accessor
    public new MainWindow Window {
      get {
        Console.WriteLine("Getting 'Window'");
        return (MainWindow)base.Window;
      }
    }

    public override void WindowDidLoad() {
      base.WindowDidLoad();
    }
  }
}