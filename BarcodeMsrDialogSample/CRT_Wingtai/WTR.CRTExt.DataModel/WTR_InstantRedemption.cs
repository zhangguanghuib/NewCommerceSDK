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
    using System;

    [DataContract]
    public class WTR_InstantRedemption : CommerceEntity
    {
        private const string IdColumn = "RECID";
        public WTR_InstantRedemption() : base("WTR_InstantRedemption")
        {
            Initialize();
        }

        public WTR_InstantRedemption(string className) : base(className)
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
            this.GivenRebateValue = 0;
            this.PointsUsage = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
        }

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }

        [DataMember(Name = "MemberNo")]
        public string MemberNo { get; set; }

        [DataMember(Name = "TransactDate")]
        public string TransactDate { get; set; }

        [DataMember(Name = "TransactTime")]
        public string TransactTime { get; set; }

        [DataMember(Name = "DollarToPointsRatio")]
        public string DollarToPointsRatio { get; set; }

        [DataMember(Name = "PointsToBeDeducted")]
        public decimal PointsToBeDeducted { get; set; }

        [DataMember(Name = "PointsUsage")]
        public decimal PointsUsage { get; set; }

        [DataMember(Name = "GivenRebateValue")]
        public decimal GivenRebateValue { get; set; }

        [DataMember(Name = "ReceiptNo")]
        public string ReceiptNo { get; set; }

        [DataMember(Name = "ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [DataMember(Name = "AscentisTransRef")]
        public string AscentisTransRef { get; set; }

        [DataMember(Name = "AscentisTransID")]
        public string AscentisTransID { get; set; }
    }
}
