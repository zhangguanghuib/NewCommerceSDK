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
    public class WTR_MemberCardTemplate : CommerceEntity
    {
        private const string IdColumn = "RECID";
        public WTR_MemberCardTemplate() : base("WTR_MemberCardTemplate")
        {
            Initialize();
        }

        public WTR_MemberCardTemplate(string className) : base(className)
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
            this.PointToNextTierUpgrade = 0;
            this.PointsBalance = 0;
            this.PointsEarned = 0;
            this.PointsRedeemed = 0;
            this.NumberOfVouchers = 0;
            this.PointsExpiringSoonest = 0;
            this.IsBirthDayMonth = "No";
            this.DollarToPointsRatio = "";
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

        [DataMember(Name = "Title")]
        public string Title
        {
            get { return (string)this["Title"]; }
            set { this["Title"] = value; }
        }

        [DataMember]
        [Column("Name")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [DataMember]
        [Column("BirthDay")]
        public DateTime? BirthDay
        {
            get { return (DateTime?)this["BirthDay"]; }
            set { this["BirthDay"] = value; }
        }


        [DataMember]
        [Column("Mobile")]
        public string Mobile
        {
            get { return (string)this["Mobile"]; }
            set { this["Mobile"] = value; }
        }

        [DataMember]
        [Column("MembershipNo")]
        public string MembershipNo
        {
            get { return (string)this["MembershipNo"]; }
            set { this["MembershipNo"] = value; }
        }

        [DataMember]
        [Column("CardNo")]
        public string CardNo
        {
            get { return (string)this["CardNo"]; }
            set { this["CardNo"] = value; }
        }

        [DataMember]
        [Column("MemberTier")]
        public string MemberTier
        {
            get { return (string)this["MemberTier"]; }
            set { this["MemberTier"] = value; }
        }

        [DataMember]
        [Column("DateJoined")]
        public DateTime? DateJoined
        {
            get { return (DateTime?)this["DateJoined"]; }
            set { this["DateJoined"] = value; }
        }

        [DataMember]
        [Column("DollarToPointsRatio")]
        public string DollarToPointsRatio
        {
            get { return (string)this["DollarToPointsRatio"]; }
            set { this["DollarToPointsRatio"] = value; }
        }

        [DataMember]
        [Column("PointToNextTierUpgrade")]
        public decimal? PointToNextTierUpgrade
        {
            get { return (decimal?)this["PointToNextTierUpgrade"]; }
            set { this["PointToNextTierUpgrade"] = value; }
        }

        [DataMember]
        [Column("MemberPrivilege")]
        public string MemberPrivilege
        {
            get { return (string)this["MemberPrivilege"]; }
            set { this["MemberPrivilege"] = value; }
        }

        [DataMember]
        [Column("PointsBalance")]
        public decimal? PointsBalance
        {
            get { return (decimal?)this["PointsBalance"]; }
            set { this["PointsBalance"] = value; }
        }

        [DataMember]
        [Column("PointsEarned")]
        public decimal? PointsEarned
        {
            get { return (decimal?)this["PointsEarned"]; }
            set { this["PointsEarned"] = value; }
        }

        // WTR_RAD After SIT2
        [DataMember]
        [Column("PointsExpiringSoonest")]
        public decimal? PointsExpiringSoonest
        {
            get { return (decimal?)this["PointsExpiringSoonest"]; }
            set { this["PointsExpiringSoonest"] = value; }
        }


        [DataMember]
        [Column("PointsExpiring")]
        public DateTime? PointsExpiring
        {
            get { return (DateTime?)this["PointsExpiring"]; }
            set { this["PointsExpiring"] = value; }
        }

        [DataMember]
        [Column("PointsExpiringLocaleDateString")]
        public string PointsExpiringLocaleDateString
        {
            get { return (string)this["PointsExpiringLocaleDateString"]; }
            set { this["PointsExpiringLocaleDateString"] = value; }
        }


        [DataMember]
        [Column("PointsRedeemed")]
        public decimal? PointsRedeemed
        {
            get { return (decimal?)this["PointsRedeemed"]; }
            set { this["PointsRedeemed"] = value; }
        }


        [DataMember]
        [Column("ExpiryDateString")]
        public string ExpiryDateString
        {
            get { return (string)this["ExpiryDateString"]; }
            set { this["ExpiryDateString"] = value; }
        }

        [DataMember]
        [Column("IsBirthDayMonth")]
        public string IsBirthDayMonth
        {
            get { return (string)this["IsBirthDayMonth"]; }
            set { this["IsBirthDayMonth"] = value; }
        }

        [DataMember]
        [Column("NumberOfVouchers")]
        public int NumberOfVouchers
        {
            get { return (int)this["NumberOfVouchers"]; }
            set { this["NumberOfVouchers"] = value; }
        }

    }
}
