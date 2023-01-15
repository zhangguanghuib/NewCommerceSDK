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
    public class WTR_ConversionRate : CommerceEntity
    {

        private const string IdColumn = "RECID";
        public WTR_ConversionRate() : base("WTR_ConversionRate")
        {
            Initialize();
        }

        public WTR_ConversionRate(string className) : base(className)
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
            this.FromRate = 0;
            this.ToRate = 0;
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

        [DataMember(Name = "FromCurrency")]
        public string FromCurrency { get; set; }

        [DataMember(Name = "FromRate")]
        public decimal FromRate { get; set; }

        [DataMember(Name = "ToCurrency")]
        public string ToCurrency { get; set; }

        [DataMember(Name = "ToRate")]
        public decimal ToRate { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }
    }
}
