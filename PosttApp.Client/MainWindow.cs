#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;

#endregion

namespace com.posttapp {
	public partial class MainWindow : MonoMac.AppKit.NSWindow {

		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow(IntPtr handle) : base (handle) {
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow(NSCoder coder) : base (coder) {
			Initialize();
		}
		
		// Shared initialization code
		void Initialize() {
      Console.WriteLine("MainWindow.Initialize()");
    }
		
		#endregion

    public void Activate() {
      generalTabClicked(this);
      this.MakeKeyAndOrderFront(this);
    }

    private void ResizeToFit(int width, int height) {
      RectangleF frame = tabsView.Window.Frame;

      float toolBarHeight = frame.Size.Height - this.ContentView.Frame.Height;
      RectangleF frameResized = new RectangleF(frame.X, frame.Y + (frame.Size.Height - height) - toolBarHeight, width, height + toolBarHeight);

      tabsView.Window.SetFrame(frameResized, true, true);
    }

    partial void generalTabClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("generalTabClicked");
      tabsView.Select(tviGeneral);
      this.Title = tabsView.Selected.Label;

      ResizeToFit(412, 186);
    }

    partial void accountTabClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("accountTabClicked");
      tabsView.Select(tviAccount);
      this.Title = tabsView.Selected.Label;

      ResizeToFit(412, 286);
    }

    partial void forgotPasswordClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Forgot password clicked");
      NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("http://ge.tt/"));
    }

    partial void createAccountClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Create account clicked");
      NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("http://ge.tt/"));
    }

    partial void signInClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Sign in clicked");

      var appDelegate = (NSApplication.SharedApplication.Delegate as AppDelegate);

      if (appDelegate.Account == null || !appDelegate.Account.IsAuthenticated) {
        SignIn();
      }
      else {
        appDelegate.Account = null;
        SignOut();
      }
    }

    private void SignIn() {
      this.MakeFirstResponder (null);

      txtEmail.Enabled = txtPassword.Enabled = btnSignIn.Enabled = false;
      btnCreateAccount.Hidden = txtForgotPassword.Hidden = true;
      inProgress.StartAnimation(this);

      var appDelegate = (NSApplication.SharedApplication.Delegate as AppDelegate);

      appDelegate.Account = new Account() {
        Email = txtEmail.StringValue,
        Password = txtPassword.StringValue,
        AccessToken = string.Empty
      };

      Console.WriteLine("Logging it with {0} and {1}", appDelegate.Account.Email, appDelegate.Account.Password);

      GettProvider.Instance.Authenticate(appDelegate.Account.Email, appDelegate.Account.Password, account => {
        Console.WriteLine("Authenticated: {0}", account.AccessToken);

        // store retrieved access token for future requests
        appDelegate.Account.AccessToken = account.AccessToken;

        BeginInvokeOnMainThread(() => {
          inProgress.StopAnimation(this);

          btnSignIn.Enabled = true;
          btnSignIn.SetTitleWithMnemonic("Sign Out");

          btnCreateAccount.Hidden = true;
          txtForgotPassword.Hidden = true;
        });
      }, failed => {
        Console.WriteLine("Failed to authenticate: {0}", failed);

        BeginInvokeOnMainThread(() => {
          SignOut();
        });
      });
    }

    private void SignOut() {
      inProgress.StopAnimation(this);

      txtEmail.Enabled = txtPassword.Enabled = btnSignIn.Enabled = true;
      btnSignIn.SetTitleWithMnemonic("Sign In");

      btnCreateAccount.Hidden = false;
      txtForgotPassword.Hidden = false;
    }
	}
}

