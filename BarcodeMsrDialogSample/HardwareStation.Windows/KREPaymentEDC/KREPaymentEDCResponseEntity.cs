namespace KREPaymentEDC.HardwareStation
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    public class KREPaymentEDCResponseEntity : CommerceEntity
    {
        private const string IsSucceedColumn = "ISSUCCEED";
        private const string ResponApprovalColumn = "RESPONAPPROVAL";

        public KREPaymentEDCResponseEntity() : base("KREPaymentEDCResponseEntity")
        {
        }

        [DataMember]
        [Column(IsSucceedColumn)]
        public string IsSucceed
        {
            get { return (string)this[IsSucceedColumn]; }
            set { this[IsSucceedColumn] = value; }
        }

        [DataMember]
        [Column(ResponApprovalColumn)]
        public string ResponApproval
        {
            get { return (string)this[ResponApprovalColumn]; }
            set { this[ResponApprovalColumn] = value; }
        }
    }
}
