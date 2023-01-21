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
    public class WTR_CRMTransactionDetails : CommerceEntity
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

        [DataMember(Name = "MEMBERID")]
        public string MEMBERID { get; set; }

        [DataMember(Name = "CARDID")]
        public string CARDID { get; set; }

        [DataMember(Name = "MEMBERTIER")]
        public string MEMBERTIER { get; set; }

        [DataMember(Name = "EARNEDPOINTS")]
        public int EARNEDPOINTS { get; set; }

        [DataMember(Name = "REDEEMEDPOINTS")]
        public int REDEEMEDPOINTS { get; set; }

        [DataMember(Name = "BALANCEPOINTS")]
        public int BALANCEPOINTS { get; set; }

        [DataMember(Name = "EXPIRINGPOINTS")]
        public int EXPIRINGPOINTS { get; set; }

        [DataMember(Name = "EXPIRINGDATE")]
        public string EXPIRINGDATE { get; set; }

        [DataMember(Name = "MEMVOUCHERCODE")]
        public string MEMVOUCHERCODE { get; set; }

        [DataMember(Name = "CRMREFID")]
        public string CRMREFID { get; set; }

        [DataMember(Name = "DOLLARSTONEXTTIER")]
        public decimal DOLLARSTONEXTTIER { get; set; }

        [DataMember(Name = "WTR_ORIGTRANSACTIONDATE")]
        public string WTR_ORIGTRANSACTIONDATE { get; set; }

        [DataMember(Name = "WTR_MEMREDEMPTIONPOINTAMOUNT")]
        public string WTR_MEMREDEMPTIONPOINTAMOUNT { get; set; }

        public WTR_CRMTransactionDetails()
            : base("WTR_CRMTransactionDetails")
        {
            Initialize();
        }

        public WTR_CRMTransactionDetails(string className)
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
            this.ExtensionProperties = new List<CommerceProperty>();

        }
    }


}
