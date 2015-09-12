using System.Runtime.Serialization;

namespace RemodelHelper.Models
{
    [DataContract]
    public class RemodelData
    {
        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public Item[] Items { get; set; }

        [DataMember]
        public UpdateData[] NewSlots { get; set; }
    }
}
