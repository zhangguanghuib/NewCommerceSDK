namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetTransactionListDataResponse : Response
    {
        public GetTransactionListDataResponse(PagedResult<Transaction> transactionList)
        {
            TransactionList = transactionList;
        }

        [DataMember]
        public PagedResult<Transaction> TransactionList { get; private set; }
    }
}
