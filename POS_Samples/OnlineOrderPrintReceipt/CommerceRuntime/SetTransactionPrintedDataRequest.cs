

namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class SetTransactionPrintedDataRequest : Request
    {
        [DataMember]
        public string TransactionId { get; set; }

        public string Store { get;set }
    }

}
