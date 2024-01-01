namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetTransactionIDListDataResponse: Response
    {
        public GetTransactionIDListDataResponse(PagedResult<string> transactionIDList)
        {
            TransactionIDList = transactionIDList;
        }

        [DataMember]
        public PagedResult<string> TransactionIDList { get; private set; }
    }
}
