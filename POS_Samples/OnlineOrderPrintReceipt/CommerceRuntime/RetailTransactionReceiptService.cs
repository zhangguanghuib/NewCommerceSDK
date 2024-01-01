namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class RetailTransactionReceiptService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetTransactionIDListDataRequest)
                };
            }
        }


        public async Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(GetTransactionIDListDataRequest))
            {
                return await this.GetTransactionList((GetTransactionIDListDataRequest)request).ConfigureAwait(false);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                Console.WriteLine(message);
                throw new NotSupportedException(message);
            }
        }

        private async Task<Response> GetTransactionList(GetTransactionIDListDataRequest request)
        {
            ThrowIf.Null(request, "request");

            using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
            {
                var query = new SqlPagedQuery(request.QueryResultSettings)
                {
                    DatabaseSchema = "ext",
                    Select = new ColumnSet("TRANSACTIONID"),
                    From = "CONTOSORETAILTRANSACTIONTABLE",
                    Where = "ISRECEIPTPRINTED = 1 AND TRANSACTIONID IN ({@TRANSACTIONIDS})",
                };

                query.Parameters["@TRANSACTIONIDS"] = request.TransactionIDList;
                ReadOnlyCollection<string> transactionIDs = await databaseContext.ExecuteScalarCollectionAsync<string>(query).ConfigureAwait(false);
                return new GetTransactionIDListDataResponse(new PagedResult<string>(transactionIDs));
            }
        }
    }
}
