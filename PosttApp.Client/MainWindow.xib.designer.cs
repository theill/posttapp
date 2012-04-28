// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace com.posttapp
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		MonoMac.AppKit.NSTextField txtEmail { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSecureTextField txtPassword { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtForgotPassword { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnSignIn { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnCreateAccount { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator inProgress { get; set; }

		[Action ("forgotPasswordClicked:")]
		partial void forgotPasswordClicked (MonoMac.Foundation.NSObject sender);

		[Action ("createAccountClicked:")]
		partial void createAccountClicked (MonoMac.Foundation.NSObject sender);

		[Action ("signInClicked:")]
		partial void signInClicked (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtForgotPassword != null) {
				txtForgotPassword.Dispose ();
				txtForgotPassword = null;
			}

			if (btnSignIn != null) {
				btnSignIn.Dispose ();
				btnSignIn = null;
			}

			if (btnCreateAccount != null) {
				btnCreateAccount.Dispose ();
				btnCreateAccount = null;
			}

			if (inProgress != null) {
				inProgress.Dispose ();
				inProgress = null;
			}
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField DirectUrlTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton UploadButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DirectUrlTextField != null) {
				DirectUrlTextField.Dispose ();
				DirectUrlTextField = null;
			}

			if (UploadButton != null) {
				UploadButton.Dispose ();
				UploadButton = null;
			}
		}
	}
}