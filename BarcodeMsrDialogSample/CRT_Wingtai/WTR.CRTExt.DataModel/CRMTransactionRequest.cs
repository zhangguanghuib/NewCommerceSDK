using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WTR.CRT.DataModel
{
    [DataContract]
    public sealed class CRMTransactionRequest : Request
    {
        [DataMember]
        public string WTR_MemberId { get; set; }

        [DataMember]
        public string WTR_CardId { get; set; }

        [DataMember]
        public decimal WTR_PointBalance { get; set; }

        [DataMember]
        public decimal WTR_EarnedPoints { get; set; }

        [DataMember]
        public decimal WTR_RedeemedPoints { get; set; }

        [DataMember]
        public decimal WTR_ExpiringPoints { get; set; }

        [DataMember]
        public string WTR_ExpiringDate { get; set; }

        [DataMember]
        public string WTR_MemberTier { get; set; }

        [DataMember]
        public decimal WTR_DollarsToNextTier { get; set; }

        public CRMTransactionRequest()
        {
            this.WTR_MemberId = string.Empty;
            this.WTR_CardId = string.Empty;
            this.WTR_PointBalance = 0;
            this.WTR_EarnedPoints = 0;
            this.WTR_ExpiringPoints = 0;
            this.WTR_ExpiringDate = string.Empty;
            this.WTR_RedeemedPoints = 0;
            this.WTR_MemberTier = string.Empty;
            this.WTR_DollarsToNextTier = 0;
        }

    }
}
