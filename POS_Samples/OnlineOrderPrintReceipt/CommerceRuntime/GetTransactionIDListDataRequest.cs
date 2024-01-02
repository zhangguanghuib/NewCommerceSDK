

namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class GetTransactionIDListDataRequest: Request
    {
        public GetTransactionIDListDataRequest(List<string> transactionIDList, QueryResultSettings settings)
        {
            TransactionIDList = transactionIDList;
            this.QueryResultSettings = settings;
        }

        [DataMember]
        public List<string> TransactionIDList { get; private set; }
    }
}
