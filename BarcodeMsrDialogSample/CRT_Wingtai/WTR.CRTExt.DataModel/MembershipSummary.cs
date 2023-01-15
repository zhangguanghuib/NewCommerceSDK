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
    public class ExpirySummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        public ExpirySummary()
            : base("ExpirySummary")
        {
            Initialize();
        }

        public ExpirySummary(string className)
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
            this.Amount = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
        }
    }

    [DataContract]
    public class AccountSummary : CommerceEntity
    {

        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "PointType")]
        public string PointType { get; set; }

        [DataMember(Name = "Earned")]
        public decimal Earned { get; set; }

        [DataMember(Name = "TransferIn")]
        public decimal TransferIn { get; set; }

        [DataMember(Name = "TransferOut")]
        public decimal TransferOut { get; set; }

        [DataMember(Name = "Refund")]
        public decimal Refund { get; set; }

        [DataMember(Name = "Used")]
        public decimal Used { get; set; }

        [DataMember(Name = "Expired")]
        public decimal Expired { get; set; }

        [DataMember(Name = "NearestExpiringDate")]
        public DateTime? NearestExpiringDate { get; set; }

        [DataMember(Name = "NearestExpiringAmount")]
        public decimal NearestExpiringAmount { get; set; }

        [DataMember(Name = "Balance")]
        public decimal Balance { get; set; }

        [DataMember(Name = "ExpirySummaries")]
        public ICollection<ExpirySummary> ExpirySummaries { get; set; }

        public AccountSummary()
            : base("AccountSummary")
        {
            Initialize();
        }

        public AccountSummary(string className)
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

    [DataContract]
    public class BenefitSummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        public BenefitSummary()
            : base("BenefitSummary")
        {
            Initialize();
        }

        public BenefitSummary(string className)
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

    [DataContract]
    public class PointBucketSummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Balance")]
        public decimal Balance { get; set; }

        [DataMember(Name = "Earned")]
        public decimal Earned { get; set; }

        [DataMember(Name = "TransferIn")]
        public decimal TransferIn { get; set; }

        [DataMember(Name = "TransferOut")]
        public decimal TransferOut { get; set; }

        [DataMember(Name = "Refund")]
        public decimal Refund { get; set; }

        [DataMember(Name = "Used")]
        public decimal Used { get; set; }

        [DataMember(Name = "ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        public PointBucketSummary()
            : base("PointBucketSummary")
        {
            Initialize();
        }

        public PointBucketSummary(string className)
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

    [DataContract]
    public class SpendingSummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "TotalPurchase")]
        public decimal TotalPurchase { get; set; }


        [DataMember(Name = "NoOfTransactions")]
        public decimal NoOfTransactions { get; set; }


        [DataMember(Name = "AvgPurchase")]
        public decimal AvgPurchase { get; set; }

        public SpendingSummary()
           : base("SpendingSummary")
        {
            Initialize();
        }

        public SpendingSummary(string className)
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

    [DataContract]
    public class CardSummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "CardNumber")]
        public string CardNumber { get; set; }

        [DataMember(Name = "CardName")]
        public string CardName { get; set; }


        [DataMember(Name = "ValidFrom")]
        public DateTime? ValidFrom { get; set; }


        [DataMember(Name = "ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }


        [DataMember(Name = "TotalPurchase")]
        public string Status { get; set; }

        public CardSummary()
           : base("CardSummary")
        {
            Initialize();
        }

        public CardSummary(string className)
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

    [DataContract]
    public class MembershipSummary : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }


        [DataMember(Name = "AccountSummaries")]
        public ICollection<AccountSummary> AccountSummaries { get; set; }

        [IgnoreDataMember]
        //[DataMember(Name = "ChanceSummaries")]
        public ICollection<object> ChanceSummaries { get; set; }

        [DataMember(Name = "SpendingSummaries")]
        public ICollection<SpendingSummary> SpendingSummaries { get; set; }

        [DataMember(Name = "CardSummaries")]
        public ICollection<CardSummary> CardSummaries { get; set; }

        [IgnoreDataMember]
        //[DataMember(Name = "PromotionSummaries")]
        public ICollection<object> PromotionSummaries { get; set; }

        [DataMember(Name = "BenefitSummaries")]
        public ICollection<BenefitSummary> BenefitSummaries { get; set; }


        [DataMember(Name = "PointBucketSummaries")]
        public ICollection<PointBucketSummary> PointBucketSummaries { get; set; }

        [IgnoreDataMember]
        //[DataMember(Name = "ChanceBucketSummaries")]
        public ICollection<object> ChanceBucketSummaries { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Tier")]
        public string Tier { get; set; }

        [DataMember(Name = "MemberNo")]
        public string MemberNo { get; set; }

        [DataMember(Name = "ValidFrom")]
        public DateTime? ValidFrom { get; set; }

        [DataMember(Name = "ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [DataMember(Name = "Status")]
        public string Status { get; set; }

        [DataMember(Name = "JoinLocationCode")]
        public string JoinLocationCode { get; set; }

        [DataMember(Name = "JoinLocationName")]
        public string JoinLocationName { get; set; }

        [DataMember(Name = "LastPeriodStartDate")]
        public DateTime? LastPeriodStartDate { get; set; }

        public MembershipSummary()
            : base("MembershipSummary")
        {
            Initialize();
        }

        public MembershipSummary(string className)
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
            this.AccountSummaries = new List<AccountSummary>();
            this.ChanceSummaries = new List<object>();
            this.SpendingSummaries = new List<SpendingSummary>();
            this.CardSummaries = new List<CardSummary>();
            this.PromotionSummaries = new List<object>();
            this.BenefitSummaries = new List<BenefitSummary>();
            this.PointBucketSummaries = new List<PointBucketSummary>();
            this.ChanceBucketSummaries = new List<object>();

        }
    }

    [DataContract]
    public class MembershipSummaryDetails : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "MembershipSummaries")]
        public ICollection<MembershipSummary> MembershipSummaries { get; set; }

        [DataMember(Name = "CustomerNumber")]
        public string CustomerNumber { get; set; }

        [DataMember(Name = "CardNo")]
        public string CardNo { get; set; }

        [DataMember(Name = "Salutation")]
        public string Title { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        public string LastName { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "IC")]
        public string IC { get; set; }

        [DataMember(Name = "DOB")]
        public DateTime DOB { get; set; }

        [DataMember(Name = "JoinDate")]
        public DateTime JoinDate { get; set; }

        [DataMember(Name = "MobileNo")]
        public string Mobile { get; set; }

        [DataMember(Name = "MobileCountryCode")]
        public object MobileCountryCode { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "Remark")]
        public object Remark { get; set; }

        [DataMember(Name = "Gender")]
        public string Gender { get; set; }

        [DataMember(Name = "Nationality")]
        public string NationalityCode { get; set; }

        [DataMember(Name = "WTR_MemberCardTemplate")]
        public WTR_MemberCardTemplate WTR_MemberCardTemplate { get; set; }

        //[DataMember(Name = "DollarsToPointRatio")] //N-1 change
        //public string DollarsToPointRatio { get; set; }

        [DataMember(Name = "MembershipStatus")]
        public string MembershipStatus { get; set; }

        [DataMember(Name = "MembershipType")]
        public string MembershipType { get; set; }

        [DataMember(Name = "MembershipTier")]
        public string MembershipTier { get; set; }

        public MembershipSummaryDetails()
           : base("MembershipSummaryDetails")
        {
            Initialize();
        }

        public MembershipSummaryDetails(string className)
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
            this.MembershipSummaries = new List<MembershipSummary>();
            this.WTR_MemberCardTemplate = new WTR_MemberCardTemplate();

        }
    }
}
