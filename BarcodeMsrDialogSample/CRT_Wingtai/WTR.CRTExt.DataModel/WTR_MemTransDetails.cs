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
    public class WTR_MemTransDetails : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        //[DataMember(Name = "MembershipSummaries")]
        //public ICollection<WTR_MemTransDetails> WTR_MemTransSummary { get; set; }

        [DataMember(Name = "Date")]
        public string Date { get; set; }

        [DataMember(Name = "ReceiptNo")]
        public string ReceiptNo { get; set; }

        [DataMember(Name = "LocationCode")]
        public string LocationCode { get; set; }

        [DataMember(Name = "Location")]
        public string Location { get; set; }

        [DataMember(Name = "PaymentMode")]
        public string PaymentMode { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "PromotionCodes")]
        public object PromotionCodes { get; set; }

        [DataMember(Name = "Rating")]
        public int Rating { get; set; }

        [DataMember(Name = "Points")]
        public List<Point> Points { get; set; }

        [DataMember(Name = "Purchases")]
        public List<Purchase> Purchases { get; set; }

        [DataMember(Name = "Chances")]
        public List<object> Chances { get; set; }

        [DataMember(Name = "PaymentModeString")]
        public string PaymentModeString { get; set; }

        public WTR_MemTransDetails()
                : base("WTR_MemTransDetails")
        {
            Initialize();
        }

        public WTR_MemTransDetails(string className)
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
            this.Points = new List<Point>();
            this.Purchases = new List<Purchase>();
            this.Chances = new List<object>();
        }

    }

    [DataContract]
    public class Point : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "ProductType")]
        public string ProductType { get; set; }

        [DataMember(Name = "ProductName")]
        public string ProductName { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "ExpiryDate")]
        public string ExpiryDate { get; set; }

        [DataMember(Name = "ValidFrom")]
        public string ValidFrom { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "EntryUser")]
        public string EntryUser { get; set; }

        [DataMember(Name = "SalesPerson")]
        public object SalesPerson { get; set; }

        [DataMember(Name = "Registrator")]
        public string Registrator { get; set; }

        [DataMember(Name = "Identifier")]
        public object Identifier { get; set; }

        [DataMember(Name = "HasHistory")]
        public object HasHistory { get; set; }

        public Point()
                : base("Point")
        {
            Initialize();
        }

        public Point(string className)
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
    public class Purchase : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Identifier")]
        public string Identifier { get; set; }

        [DataMember(Name = "ProductNo")]
        public string ProductNo { get; set; }

        [DataMember(Name = "ProductType")]
        public string ProductType { get; set; }

        [DataMember(Name = "ProductName")]
        public string ProductName { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "ValidFrom")]
        public string ValidFrom { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "EntryUser")]
        public string EntryUser { get; set; }

        [DataMember(Name = "SalesPerson")]
        public string SalesPerson { get; set; }

        [DataMember(Name = "Registrator")]
        public string Registrator { get; set; }

        [DataMember(Name = "HasHistory")]
        public bool HasHistory { get; set; }

        public Purchase()
                : base("Purchase")
        {
            Initialize();
        }

        public Purchase(string className)
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

}
