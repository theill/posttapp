using System.Runtime.Serialization;

namespace io.postt.macos {
  [DataContract]
  public class Upload {
    [DataMember(Name = "puturl")]
    public string PutUrl {
      get;
      set;
    }

    [DataMember(Name = "posturl")]
    public string PostUrl {
      get;
      set;
    }

    public override string ToString() {
      return string.Format("[PutUrl={0}, PostUrl={1}]", PutUrl, PostUrl);
    }
  }
}