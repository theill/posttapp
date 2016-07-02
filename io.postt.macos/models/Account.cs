using System;

namespace io.postt.macos {
  public class Account {
    public string Email { get; set; }
    public string Password { get; set; }
    public string Fullname { get; set; }
    public string AccessToken { get; set; }

    public bool IsAuthenticated {
      get {
        return !string.IsNullOrEmpty(this.AccessToken);
      }
    }
  }
}