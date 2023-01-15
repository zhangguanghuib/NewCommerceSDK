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
    public class WTR_QualificationCriteria : CommerceEntity
    {


        private const string IdColumn = "RECID";
        public WTR_QualificationCriteria() : base("WTR_QualificationCriteria")
        {
            Initialize();
        }

        public WTR_QualificationCriteria(string className) : base(className)
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
            this.AmountRequired = 0;
            this.SpendingRuleUnit = 0;
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

        [DataMember(Name = "TargetMembership")]
        public string TargetMembership { get; set; }

        [DataMember(Name = "ActionType")]
        public string ActionType { get; set; }

        [DataMember(Name = "AmountRequired")]
        public decimal AmountRequired { get; set; }

        [DataMember(Name = "RequestDate")]
        public DateTime? RequestDate { get; set; }

        [DataMember(Name = "SpendingRule")]
        public string SpendingRule { get; set; }

        [DataMember(Name = "SpendingRuleType")]
        public string SpendingRuleType { get; set; }

        [DataMember(Name = "SpendingRuleUnit")]
        public decimal SpendingRuleUnit { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

    }
}
