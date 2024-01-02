namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetTransactionIDListDataResponse: Response
    {
        public GetTransactionIDListDataResponse(PagedResult<Transaction> transactionList)
        {
            TransactionList = transactionList;
        }

        [DataMember]
        public PagedResult<Transaction> TransactionList { get; private set; }
    }
}
