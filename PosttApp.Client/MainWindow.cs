#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

#endregion

namespace com.posttapp {
	public partial class MainWindow : MonoMac.AppKit.NSWindow {
    const string API_TOKEN = "tfvpf59kj3lyp66r3cm84vabirv34n29";
    public GettProvider Gett = new GettProvider(API_TOKEN);

    public Account Account { get; set; }

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

      var gett = new GettProvider("tfvpf59kj3lyp66r3cm84vabirv34n29");
      gett.Authenticate(Account.Email, Account.Password, account => {

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

