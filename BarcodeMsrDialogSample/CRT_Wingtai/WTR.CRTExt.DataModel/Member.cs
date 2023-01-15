using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WTR.CRT.DataModel
{
    [DataContract]
    public class WTR_MemberIdentifier :CommerceEntity
    {
        private const string IdColumn = "RECID";

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }

        [DataMember]
        [Column("MemberID")]
        public string MemberID { get; set; }

        [DataMember]
        [Column("CardNo")]
        public string CardNo
        {
            get { return (string)this["CardNo"]; }
            set { this["CardNo"] = value; }
        }
        public WTR_MemberIdentifier(string className)
            : base(className)
        {
            this.Id = 0;
        }
    }
    [DataContract]
    public class Member : CommerceEntity
    {
        private const string MemberIdColumn = "MEMBERID";
        private const string FirstNameColumn = "FIRSTNAME";
        private const string LastNameColumn = "LASTNAME";
        private const string GenderColumn = "GENDER";
        private const string EmailColumn = "EMAIL";
        private const string CountryCodeColumn = "COUNTRYCODE";
        private const string MobileColumn = "MOBILE";
       
        private const string IdColumn = "RECID";

        

        [Key]
        [DataMember]
        [Column(MemberIdColumn)]
        public string MemberID
        {
            get { return (string)this[MemberIdColumn]; }
            set { this[MemberIdColumn] = value; }
        }

        [DataMember]
        [Column(MobileColumn)]
        public string Mobile
        {
            get { return (string)this[MobileColumn]; }
            set { this[MobileColumn] = value; }
        }
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [DataMember(Name = "FirstName")]
        [Column(FirstNameColumn)]
        public string FirstName
        {
            get { return (string)this[FirstNameColumn]; }
            set { this[FirstNameColumn] = value; }
        }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [DataMember(Name = "LastName")]
        [Column(LastNameColumn)]
        public string LastName
        {
            get { return (string)this[LastNameColumn]; }
            set { this[LastNameColumn] = value; }
        }

        /// <summary>
        /// Gets or sets the Gender
        /// </summary>
        [DataMember(Name = "Gender")]
        [Column(GenderColumn)]
        public string Gender
        {
            get { return (string)this[GenderColumn]; }
            set { this[GenderColumn] = value; }
        }

        [DataMember(Name = "CountryCode")]
        [Column(CountryCodeColumn)]
        public string CountryCode
        {
            get { return (string)this[CountryCodeColumn]; }
            set { this[CountryCodeColumn] = value; }
        }
        [DataMember(Name = "Email")]
        [Column(EmailColumn)]
        public string Email
        {
            get { return (string)this[EmailColumn]; }
            set { this[EmailColumn] = value; }
        }
        public Member()
            : base("Member")
        {
        }
        public Member(string className)
            : base(className)
        {
            //Initialize();
        }

    }
}
