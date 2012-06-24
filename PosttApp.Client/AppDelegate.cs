#region Using directives
using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
#endregion

namespace com.posttapp {
	public partial class AppDelegate : NSApplicationDelegate {
		private MainWindowController mainWindowController;
    private NSStatusItem mainItem;
    public Account Account = new Account();

		public AppDelegate() {
		}

		public override void FinishedLaunching(NSObject notification) {
      Console.WriteLine("FinishedLaunching(notification={0})", notification);
      mainWindowController = new MainWindowController();

      InitializeStatusBar();
    }

    public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) {
      return false;
    }

    void InitializeStatusBar() {
      // creating the status item with a length of -2 is equivalent to the call
      // [[NSStatusBar systemStatusBar] statusItemWithLength:NSSquareStatusItemLength]
      mainItem = NSStatusBar.SystemStatusBar.CreateStatusItem(-2);
      mainItem.HighlightMode = true;
      mainItem.Menu = menu;
      mainItem.View = new StatusItemView(mainItem, ItemDropped);
    }

    void ItemDropped(string item) {
      Console.WriteLine("Item dropped: {0}", item);
      
      if (Account.IsAuthenticated) {
        GettProvider.Instance.CreateShare("test1", Account.AccessToken, item.Replace("file://localhost", "").Replace("%20", " "));
      }
      else {
        GettProvider.Instance.Authenticate(Account.Email, Account.Password, (account) => {
          // user is logged in

          // store retrieved access token for future requests
          Account.AccessToken = account.AccessToken;

          GettProvider.Instance.CreateShare("test1", account.AccessToken, item.Replace("file://localhost", "").Replace("%20", " "));
        }, (error) => {
          // TODO: handle error message when uploading
          Console.WriteLine("Got an error: {0}", error);
        });
      }
    }

    partial void launchGettWebsiteClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Open ge.tt...");
      NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("http://ge.tt/"));
    }

    partial void preferencesClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Open Preferences...");

      NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
      mainWindowController.Window.Activate();
    }

    partial void helpClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Open help section...");
      NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("http://posttapp.com/help"));
    }

    partial void quitClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Quit application");
      NSApplication.SharedApplication.Terminate(this);
    }

	}
}