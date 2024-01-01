

namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class GetTransactionIDListDataRequest: Request
    {
        public GetTransactionIDListDataRequest(string transactionIDList, QueryResultSettings settings)
        {
            TransactionIDList = transactionIDList;
            this.QueryResultSettings = settings;
        }

        [DataMember]
        public string TransactionIDList { get; private set; }
    }
}
