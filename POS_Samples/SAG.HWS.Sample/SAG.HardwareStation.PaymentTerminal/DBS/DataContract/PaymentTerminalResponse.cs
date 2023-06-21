using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Dynamics.Commerce.Runtime.Messages;

namespace SAG.HardwareStation.PaymentTerminal
{
    [DataContract]
    public class PaymentTerminalResponse : Response
    {
        [DataMember]
        public DBS.DataContract.DBSResponse PaymentTerminal { get; set; }

        public PaymentTerminalResponse(DBS.DataContract.DBSResponse _dbsResponse)
        {
            this.PaymentTerminal = _dbsResponse;
        }
        public PaymentTerminalResponse()
        {
            this.PaymentTerminal = new DBS.DataContract.DBSResponse();
        }
    }
}
