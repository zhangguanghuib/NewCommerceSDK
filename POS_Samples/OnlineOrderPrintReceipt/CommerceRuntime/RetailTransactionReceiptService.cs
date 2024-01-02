namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.Data.Types;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using static System.Net.Mime.MediaTypeNames;

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

            using (DataTable transIdTable = new DataTable("STRINGIDTABLETYPE")) 
            {
                using (SqlServerDatabaseContext databaseContext = new SqlServerDatabaseContext(request.RequestContext))
                {
                    transIdTable.Columns.Add("STRINGID", typeof(string));
                    foreach (string transId in request.TransactionIDList)
                    {
                        transIdTable.Rows.Add(transId);
                    }

                    ParameterSet parameters = new ParameterSet();
                    parameters["@AllTransIds"] = transIdTable;
                    parameters["@MinDate"] = DateTimeOffset.Now;
                    parameters["@MaxDate"] = DateTimeOffset.Now;

                    PagedResult<Transaction> PrintedTransactions = (await databaseContext.ExecuteStoredProcedureAsync<Transaction>("[ext].GETPRINTEDTRANSACTIONS", parameters, QueryResultSettings.AllRecords).ConfigureAwait(false)).Item2;

                    return new GetTransactionIDListDataResponse(PrintedTransactions);
                }
            }
        }
    }
}
