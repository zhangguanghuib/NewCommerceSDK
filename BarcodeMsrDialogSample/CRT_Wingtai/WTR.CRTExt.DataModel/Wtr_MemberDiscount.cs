namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using System.Collections.Generic;


    [DataContract]
    public class Wtr_MemberDiscount : CommerceEntity
    {


        public Wtr_MemberDiscount()
            : base("Wtr_MemberDiscount")
        {
            Initialize();
        }

        public Wtr_MemberDiscount(string className)
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
            this.Amount = Convert.ToDecimal(0);
        }

        private const string IdColumn = "RECID";

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }


        [Column("MemberNumber")]
        [DataMember]
        public string MemberNumber { get; set; }

        [Column("BenefitCode")]
        [DataMember]
        public string BenefitCode { get; set; }

        [Column("BenefitName")]
        [DataMember]
        public string BenefitName { get; set; }

        [Column("BenefitTypeCode")]
        [DataMember]
        public string BenefitTypeCode { get; set; }

        [Column("BenefitSubTypeCode")]
        [DataMember]
        public string BenefitSubTypeCode { get; set; }

        [Column("Status")]
        [DataMember]
        public string Status { get; set; }

        [Column("Description")]
        [DataMember]
        public string Description { get; set; }

        [Column("LocationCode")]
        [DataMember]
        public string LocationCode { get; set; }

        [Column("LocationName")]
        [DataMember]
        public string LocationName { get; set; }

        [Column("Amount")]
        [DataMember]
        public decimal Amount { get; set; }

        [Column("DiscountType")]
        [DataMember]
        public string DiscountType { get; set; }

        [Column("ValidFrom")]
        [DataMember]
        public string ValidFrom { get; set; }

        [Column("ExpiryDate")]
        [DataMember]
        public DateTime? ExpiryDate
        {
            get { return (DateTime?)this["ExpiryDate"]; }
            set { this["ExpiryDate"] = value; }
        }

        [Column("BenefitType")]
        [DataMember]
        public string BenefitType { get; set; }

        [Column("BenefitSubType")]
        [DataMember]
        public string BenefitSubType { get; set; }

        [Column("IssueDate")]
        [DataMember]
        public DateTime? IssueDate
        {
            get { return (DateTime?)this["IssueDate"]; }
            set { this["IssueDate"] = value; }
        }

    }
}
