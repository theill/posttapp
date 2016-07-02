using System;

namespace io.postt {
	public interface IPosttProvider {
		bool Authenticated { get; }

		void Authenticate(string username, string password, Action<Account> loggedIn, Action<string> loginFailed);
		void Upload(string path, string filename, Action<string> urlCallback);
	}
}

