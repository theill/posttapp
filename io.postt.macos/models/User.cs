using System.Runtime.Serialization;

namespace io.postt.macos {
  [DataContract]
  public class User {
    [DataMember(Name = "userid")]
    public string UserID {
      get;
      set;
    }

    [DataMember(Name = "fullname")]
    public string Fullname {
      get;
      set;
    }

    [DataMember(Name = "email")]
    public string Email {
      get;
      set;
    }

    [DataMember(Name = "files")]
    public long Files {
      get;
      set;
    }

    [DataMember(Name = "downloads")]
    public long Downloads {
      get;
      set;
    }

    [DataMember(Name = "storage")]
    public Storage Storage {
      get;
      set;
    }

    public override string ToString() {
      return string.Format("[UserID={0}, Fullname={1}, Email={2}, Files={3}, Downloads={4}, Storage={5}]", UserID, Fullname, Email, Files, Downloads, Storage);
    }
  }
}