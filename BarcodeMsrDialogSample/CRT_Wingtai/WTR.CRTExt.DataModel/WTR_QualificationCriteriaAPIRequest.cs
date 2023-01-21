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
    using System.Collections.Generic;

    [DataContract]
    public class WTR_QualificationCriteriaAPIRequest : CommerceEntity
    {
        private const string IdColumn = "RECID";

        public WTR_QualificationCriteriaAPIRequest()
            : base("WTR_QualificationCriteriaAPIRequest")
        {
            Initialize();
        }

        public WTR_QualificationCriteriaAPIRequest(string className)
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

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }

        [DataMember(Name = "MemberNumber")]
        public string MemberNumber { get; set; }

        [DataMember(Name = "TargetMembership")]
        public string TargetMembership { get; set; }

        [DataMember(Name = "ActionType")]
        public string ActionType { get; set; }

        [DataMember(Name = "RequestDate")]
        public DateTime RequestDate { get; set; }
    }
}
