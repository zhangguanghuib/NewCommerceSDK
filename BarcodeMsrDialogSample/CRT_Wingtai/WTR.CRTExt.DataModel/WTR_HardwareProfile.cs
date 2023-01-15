using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WTR.CRT.DataModel
{
    [DataContract]
    public class WTR_HardwareProfile : CommerceEntity
    {
        public WTR_HardwareProfile() : base("WTR_HARDWAREPROFILE")
        {
            Initialize();
        }

        [OnDeserializing]
        public new void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.ProfileId = "";
            this.WTR_PAYMENTTERMINAL1COMPORT = "";
            this.WTR_PAYMENTTERMINAL1TYPE = 0;
            this.WTR_PAYMENTTERMINAL2TYPE = 0;
            this.WTR_PAYMENTTERMINAL2COMPORT = "";
            this.ExtensionProperties = new List<CommerceProperty>();
        }
       

      
        public WTR_HardwareProfile(string className) : base(className)
        {
            Initialize();
        }
        [Key]
        [Column("PROFILEID")]
        [DataMember]
        public string ProfileId { get; set; }

        [Column("WTR_PAYMENTTERMINAL1TYPE")]
        [DataMember]
        public  int WTR_PAYMENTTERMINAL1TYPE { get; set; }

        [Column("WTR_PAYMENTTERMINAL1COMPORT")]
        [DataMember]
        public string WTR_PAYMENTTERMINAL1COMPORT { get; set; }

        [Column("WTR_PAYMENTTERMINAL2TYPE")]
        [DataMember]
        public int WTR_PAYMENTTERMINAL2TYPE { get; set; }

        [Column("WTR_PAYMENTTERMINAL2COMPORT")]
        [DataMember]
        public string WTR_PAYMENTTERMINAL2COMPORT { get; set; }
    }
}