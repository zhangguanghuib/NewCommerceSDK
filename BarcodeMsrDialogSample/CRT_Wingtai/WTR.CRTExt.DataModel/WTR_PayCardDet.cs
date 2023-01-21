using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;


    [DataContract]
    public class WTR_PayCardDet : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "CHANNEL")]
        public string CHANNEL { get; set; }

        [DataMember(Name = "DATAAREAID")]
        public string DATAAREAID { get; set; }

        [DataMember(Name = "STORE")]
        public string STORE { get; set; }

        [DataMember(Name = "TERMINAL")]
        public string TERMINAL { get; set; }

        [DataMember(Name = "TRANSACTIONID")]
        public string TRANSACTIONID { get; set; }

        [DataMember(Name = "CARDTYPEID")]
        public string CARDTYPEID { get; set; }

        [DataMember(Name = "ISMANUAL")]
        public int ISMANUAL { get; set; }

        [DataMember(Name = "APPROVALCODE")]
        public string APPROVALCODE { get; set; }

        [DataMember(Name = "STAN")]
        public string STAN { get; set; }

        [DataMember(Name = "RETRIEVALREFERENCENUMBER")]
        public string RETRIEVALREFERENCENUMBER { get; set; }


        [DataMember(Name = "RESPONSECODE")]
        public string RESPONSECODE { get; set; }

        [DataMember(Name = "CARDTYPE")]
        public string CARDTYPE { get; set; }

        [DataMember(Name = "AMOUNTTENDERED")]
        public decimal AMOUNTTENDERED { get; set; }


        public WTR_PayCardDet()
            : base("WTR_PayCardDet")
        {
            Initialize();
        }

        public WTR_PayCardDet(string className)
        : base(className)
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
            this.Id = 0;
            this.ISMANUAL = 0;
            this.AMOUNTTENDERED = 0;
            this.ExtensionProperties = new List<CommerceProperty>();

        }
    }
}
