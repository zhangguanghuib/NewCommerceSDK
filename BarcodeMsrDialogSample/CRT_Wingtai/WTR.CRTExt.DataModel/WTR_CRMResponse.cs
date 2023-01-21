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
    public class WTR_CRMResponse: CommerceEntity
    {
        private const string IdColumn = "RECID";
        public WTR_CRMResponse(string entityName) : base("WTR_CRMResponse")
        {
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
        public int ReturnStatus { get; set; }
        [DataMember]
        public string ReturnMessage { get; set; }
        [DataMember]
        public string CardNo { get; set; }
        [DataMember]
        public string MemberID { get; set; }
    }
}
