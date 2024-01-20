namespace Contoso.GasStationSample.CommerceRuntime.Messages
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
        public SetTransactionPrintedDataRequest(Transaction transaction,
            bool isReceiptPrinted)
        {
            this.Transaction = transaction;
            this.IsReceiptPrinted = isReceiptPrinted;
        }

        [DataMember]
        public Transaction Transaction { get; set; }

        [DataMember]
        public bool IsReceiptPrinted { get; set; }
    }

}
