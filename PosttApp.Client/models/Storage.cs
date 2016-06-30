using System.Runtime.Serialization;

namespace com.posttapp {
  [DataContract]
  public class Storage {
    [DataMember(Name = "used")]
    public long Used {
      get;
      set;
    }

    [DataMember(Name = "limit")]
    public long Limit {
      get;
      set;
    }

    [DataMember(Name = "extra")]
    public long Extra {
      get;
      set;
    }

    public override string ToString() {
      return string.Format("[Used={0}, Limit={1}, Extra={2}]", Used, Limit, Extra);
    }
  }
}