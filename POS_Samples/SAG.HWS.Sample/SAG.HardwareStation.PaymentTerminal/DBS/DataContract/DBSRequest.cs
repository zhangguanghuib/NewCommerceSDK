using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System.Runtime.Serialization;

namespace SAG.HardwareStation.PaymentTerminal.DBS.DataContract
{
    [DataContract]
    public class DBSRequest: Request
    {
        [DataMember(Name = "COMPort")]
        public string COMPort { get; set; }
        [DataMember(Name = "TransactionType")]
        public string TransactionType { get; set; }

        [DataMember(Name = "ECRRefNumber")]
        public string ECRRefNumber { get; set; }

        [DataMember(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "Tips")]
        public decimal Tips { get; set; }

        [DataMember(Name = "TraceNo")]
        public string TraceNo { get; set; }

        public DBSRequest()
        {
            COMPort = "";
            TransactionType = "";
            ECRRefNumber = "";
            Amount = 0;
            Tips = 0;
            TraceNo = "";
        }
    }
}
