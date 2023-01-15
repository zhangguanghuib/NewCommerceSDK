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
    public class WTR_MemberDiscountTotals : CommerceEntity
    {
        private const string IdColumn = "RECID";
        public WTR_MemberDiscountTotals() : base("WTR_MemberDiscountTotals")
        {
            Initialize();
        }

        public WTR_MemberDiscountTotals(string className) : base(className)
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
            this.CouponRedemptionDiscountTotal = 0;
            this.PointRedemptionDiscountTotal = 0;
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

        [DataMember(Name = "CouponRedemptionDiscountTotal")]
        public decimal CouponRedemptionDiscountTotal { get; set; }

        [DataMember(Name = "PointRedemptionDiscountTotal")]
        public decimal PointRedemptionDiscountTotal { get; set; }

    }
}
