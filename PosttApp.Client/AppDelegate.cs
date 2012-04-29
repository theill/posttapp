#region Using directives
using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#endregion

namespace com.posttapp {
	public partial class AppDelegate : NSApplicationDelegate {
		private MainWindowController mainWindowController;

		public AppDelegate() {
		}

		public override void FinishedLaunching(NSObject notification) {
      Console.WriteLine("FinishedLaunching(notification={0})", notification);
      mainWindowController = new MainWindowController();
      mainWindowController.Window.MakeKeyAndOrderFront(this);
    }

    public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) {
      return false;
    }
	}
}