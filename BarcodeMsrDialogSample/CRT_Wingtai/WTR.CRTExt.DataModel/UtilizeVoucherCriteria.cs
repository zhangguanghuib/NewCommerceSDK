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
    public class UtilizeVoucherCriteria : CommerceEntitySearch
    {
        private const string IdColumn = "RECID";

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id { get; set; }

        [DataMember]
        public string VoucherCode { get; set; }

        [DataMember]
        public string VoucherNumber { get; set; }

        [DataMember]
        public string VoucherName { get; set; }

        [DataMember]
        public string TransactDate { get; set; }

        [DataMember]
        public string TransactTime { get; set; }

        [DataMember]
        public string MemberNumber { get; set; }
    }
}
