namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading.Tasks;
    using Contoso.GasStationSample.CommerceRuntime.Messages;
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
                    typeof(GetTransactionListDataRequest),
                    typeof(SetTransactionPrintedDataRequest)
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
            if (reqType == typeof(GetTransactionListDataRequest))
            {
                return await this.GetTransactionList((GetTransactionListDataRequest)request).ConfigureAwait(false);
            }
            else if (reqType == typeof(SetTransactionPrintedDataRequest))
            {
                return await this.SetTransactionPrinted((SetTransactionPrintedDataRequest)request).ConfigureAwait(false);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                Console.WriteLine(message);
                throw new NotSupportedException(message);
            }
        }

        private async Task<Response> GetTransactionList(GetTransactionListDataRequest request)
        {
            ThrowIf.Null(request, "request");

            using (DataTable transIdTable = new DataTable("STRINGIDTABLETYPE")) 
            {
                using (SqlServerDatabaseContext databaseContext = new SqlServerDatabaseContext(request.RequestContext))
                {
                    transIdTable.Columns.Add("STRINGID", typeof(string));
                    foreach (string transId in request.TransactionList)
                    {
                        transIdTable.Rows.Add(transId);
                    }

                    ParameterSet parameters = new ParameterSet();
                    parameters["@AllTransIds"] = transIdTable;
                    parameters["@MinDate"] = DateTimeOffset.Now;
                    parameters["@MaxDate"] = DateTimeOffset.Now;

                    PagedResult<Transaction> PrintedTransactions = (await databaseContext.ExecuteStoredProcedureAsync<Transaction>("[ext].GETPRINTEDTRANSACTIONS", parameters, QueryResultSettings.AllRecords).ConfigureAwait(false)).Item2;

                    return new GetTransactionListDataResponse(PrintedTransactions);
                }
            }
        }

        private async Task<Response> SetTransactionPrinted(SetTransactionPrintedDataRequest request)
        {
            ThrowIf.Null(request, "request");

            using (var sqlDatanaseContext = new SqlServerDatabaseContext(request.RequestContext))
            {
                ParameterSet parameters = new ParameterSet();
                parameters["@s_TRANSACTIONID"] = request.Transaction.Id;
                parameters["@s_STORE"] = request.Transaction.StoreId;
                parameters["@bi_CHANNEL"] = request.RequestContext.GetChannel().RecordId;
                parameters["@s_TERMINAL"] = request.Transaction.TerminalId;
                parameters["@s_DATAAREAID"] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                parameters["@i_ISRECEIPTPRIENTED"] = request.IsReceiptPrinted ? 1: 0;
                int ret = await sqlDatanaseContext.ExecuteStoredProcedureNonQueryAsync("[ext].[SETTRANSACTIONPRINTED]", parameters, resultSettings: null).ConfigureAwait(false);
                return new SetTransactionPrintedDataResponse(ret);
            }
        }
    }
}
