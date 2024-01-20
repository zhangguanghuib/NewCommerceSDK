
namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contoso.GasStationSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class RetailTransactionReceiptsController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<Transaction>> SearchJournalTransactionsWithUnPrintReceipt(IEndpointContext context, TransactionSearchCriteria searchCriteria, QueryResultSettings queryResultSettings)
        {
            ThrowIf.Null(searchCriteria, nameof(searchCriteria));

            if (searchCriteria.SearchLocationType == SearchLocation.None)
            {
                searchCriteria.SearchLocationType = SearchLocation.Local;
            }

            // By Search Criteria to find all Transactions:
            var request = new SearchJournalTransactionsServiceRequest(searchCriteria, queryResultSettings);
            var response = await context.ExecuteAsync<SearchJournalTransactionsServiceResponse>(request).ConfigureAwait(false);

            //IEnumerable<Transaction> transactions = response.Transactions.Where<Transaction>(t =>
            //{
            //    return !(t.ExtensionProperties.Any(p => p.Key.Equals("ISRECIPTPRINTED"))) || t.ExtensionProperties.Any(p => p.Key.Equals("ISRECIPTPRINTED") && p.Value.IntegerValue.Equals(0));
            //});

            // Get All Transaction IDs whose Receipt has already printed:
            List<string> transactionIDListOrig = response.Transactions.Select(transaction => transaction.Id).ToList<string>();
            GetTransactionListDataRequest getTransactionIDListRequest = new GetTransactionListDataRequest(transactionIDListOrig, QueryResultSettings.AllRecords);
            var getTransactionIDListDataResponse = await context.ExecuteAsync<GetTransactionListDataResponse>(getTransactionIDListRequest).ConfigureAwait(false);
            List<string> printedTransList = getTransactionIDListDataResponse.TransactionList.Select(transaction => transaction.Id).ToList<string>();
            List<string> unPrintedTransactionIDList = transactionIDListOrig.Except(printedTransList).ToList();


            //// Get all transaction ids with Receipt Printed 
            //List<string> transactionIDListPrinted = getTransactionIDListDataResponse.TransactionIDList.ToList<string>();

            //// Get all transaction ids with Receipt Not Printed 
            //List<string> unPrintedTransactionIDList = transactionIDListOrig.Where(t1 => !transactionIDListPrinted.Any(t2 => t2.Equals(t1))).ToList();

            IEnumerable<Transaction> transactions = response.Transactions.Where<Transaction>(t =>
            {
                return unPrintedTransactionIDList.Any(s => t.Id.Equals(s));
            });

            List<Transaction> transactionList = new List<Transaction>();
            foreach( Transaction transaction in transactions)
            {
                transactionList.Add(transaction);
            }

            PagedResult<Transaction> filteredTransactions =
                new PagedResult<Transaction>(new System.Collections.ObjectModel.ReadOnlyCollection<Transaction>(transactionList));

            return filteredTransactions;
        }

        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<int> SetTransactionPrinted(IEndpointContext context, Transaction transaction)
        {
            var request = new SetTransactionPrintedDataRequest(transaction, true);
            var response = await context.ExecuteAsync<SetTransactionPrintedDataResponse>(request).ConfigureAwait(false);
            return response.Result;
        }
    }
}
