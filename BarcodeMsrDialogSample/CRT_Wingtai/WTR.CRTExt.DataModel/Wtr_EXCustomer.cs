namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using System.Collections.Generic;


    [DataContract]
    public class Wtr_EXCustomer : CommerceEntity
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

        [Column("AccountNumber")]
        [DataMember]
        public string AccountNumber { get; set; }

        [Column("CustomerTypeValue")]
        [DataMember]
        public int CustomerTypeValue { get; set; }

        [Column("Email")]
        [DataMember]
        public string Email { get; set; }

        [Column("FullAddress")]
        [DataMember]
        public string FullAddress { get; set; }

        [Column("FullName")]
        [DataMember]
        public string FullName { get; set; }

        [Column("FirstName")]
        [DataMember]
        public string FirstName { get; set; }

        [Column("LastName")]
        [DataMember]
        public string LastName { get; set; }

        [Column("Images")]
        [DataMember]
        public IEnumerable<MediaLocation> Images { get; set; }

        [Column("IsAsyncCustomer")]
        [DataMember]
        public bool IsAsyncCustomer { get; set; }

        [Column("OfflineImage")]
        [DataMember]
        public string OfflineImage { get; set; }

        [Column("PartyNumber")]
        [DataMember]
        public string PartyNumber { get; set; }

        [Column("Phone")]
        [DataMember]
        public string Phone { get; set; }

        [Column("RecordId")]
        [DataMember]
        [Key]
        public long RecordId { get; set; }

        [Column("MemCustomerNumber")]
        [DataMember]
        public string MemCustomerNumber { get; set; }


        [Column("MemMemberId")]
        [DataMember]
        public string MemMemberId { get; set; }

        [Column("NRIC")]
        [DataMember]
        public string NRIC { get; set; }


        [Column("MembershipStatus")]
        [DataMember]
        public string MembershipStatus { get; set; }

        [Column("MembershipType")]
        [DataMember]
        public string MembershipType { get; set; }

        [Column("MembershipTier")]
        [DataMember]
        public string MembershipTier { get; set; }

        [Column("DOB")]
        [DataMember]
        public bool DOB { get; set; }

        [Column("GenderCode")]
        [DataMember]
        public string GenderCode { get; set; }

        [Column("NationalityCode")]
        [DataMember]
        public string NationalityCode { get; set; }

        [Column("HasActiveMembership ")]
        [DataMember]
        public bool HasActiveMembership { get; set; }


        public Wtr_EXCustomer()
            : base("Wtr_EXCustomer")
        {
            //Initialize();
        }

        public Wtr_EXCustomer(string className)
        : base(className)
        {
            //Initialize();
        }

    }
}
