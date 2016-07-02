// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace io.postt {
  [Register("AppDelegate")]
  partial class AppDelegate {
    [Outlet]
    MonoMac.AppKit.NSMenu menu { get; set; }

    [Action("launchGettWebsiteClicked:")]
    partial void launchGettWebsiteClicked(MonoMac.Foundation.NSObject sender);

    [Action("preferencesClicked:")]
    partial void preferencesClicked(MonoMac.Foundation.NSObject sender);

    [Action("helpClicked:")]
    partial void helpClicked(MonoMac.Foundation.NSObject sender);

    [Action("quitClicked:")]
    partial void quitClicked(MonoMac.Foundation.NSObject sender);

    void ReleaseDesignerOutlets() {
      if (menu != null) {
        menu.Dispose();
        menu = null;
      }
    }
  }
}
