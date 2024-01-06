namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class SetTransactionPrintedDataResponse : Response
    {
        public SetTransactionPrintedDataResponse(int result)
        {
            Result = result;
        }

        [DataMember]
        public int Result { get; set; }
    }
}
