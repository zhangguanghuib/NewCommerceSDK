namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class GetTransactionListDataRequest : Request
    {
        public GetTransactionListDataRequest(List<string> transactionIDList, QueryResultSettings settings)
        {
            this.TransactionList = transactionIDList;
            this.QueryResultSettings = settings;
        }

        [DataMember]
        public List<string> TransactionList { get; private set; }
    }
}
