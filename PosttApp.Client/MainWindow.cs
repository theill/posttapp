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

    private Account account;
    public Account Account {
      get {
        if (account == null) {
          account = new Account();
        }

        return account;
      }

      set {
        account = value ?? new Account();
      }
    }

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
    }

    partial void createAccountClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Create account clicked");
    }

    partial void signInClicked(MonoMac.Foundation.NSObject sender) {
      Console.WriteLine("Sign in clicked");

      if (Account == null || string.IsNullOrEmpty(Account.AccessToken)) {
        SignIn();
      }
      else {
        Account = null;
        SignOut();
      }
    }

    private void SignIn() {
      txtEmail.Enabled = txtPassword.Enabled = btnSignIn.Enabled = false;
      btnCreateAccount.Hidden = txtForgotPassword.Hidden = true;
      inProgress.StartAnimation(this);

      this.Account = new Account() {
        Email = txtEmail.StringValue,
        Password = txtPassword.StringValue,
        AccessToken = string.Empty
      };

      Console.WriteLine("Logging it with {0} and {1}", Account.Email, Account.Password);

      GettProvider.Instance.Authenticate(Account.Email, Account.Password, account => {
        Console.WriteLine("Authenticated: {0}", account.AccessToken);

        // store retrieved access token for future requests
        this.Account.AccessToken = account.AccessToken;

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

