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
    public class UtilizeVoucher : CommerceEntity
    {
        private const string IdColumn = "RECID";
        public UtilizeVoucher() : base("UtilizeVoucher")
        {
            Initialize();
        }

        public UtilizeVoucher(string className) : base(className)
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

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }

        [DataMember]
        [Column("VoucherCode")]
        public string VoucherCode
        {
            get { return (string)this["VoucherCode"]; }
            set { this["VoucherCode"] = value; }
        }

        [DataMember]
        [Column("VoucherNumber")]
        public string VoucherNumber
        {
            get { return (string)this["VoucherNumber"]; }
            set { this["VoucherNumber"] = value; }
        }

        [DataMember]
        [Column("VoucherName")]
        public string VoucherName
        {
            get { return (string)this["VoucherName"]; }
            set { this["VoucherName"] = value; }
        }

        [DataMember]
        [Column("UtilizeDate")]
        public DateTime UtilizeDate
        {
            get { return (DateTime)this["UtilizeDate"]; }
            set { this["UtilizeDate"] = value; }
        }

        [DataMember]
        [Column("MemberNumber")]
        public string MemberNumber
        {
            get { return (string)this["MemberNumber"]; }
            set { this["MemberNumber"] = value; }
        }
    }
}
