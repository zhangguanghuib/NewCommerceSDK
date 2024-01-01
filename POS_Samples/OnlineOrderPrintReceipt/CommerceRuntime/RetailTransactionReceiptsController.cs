
namespace Contoso.GasStationSample.CommerceRuntime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
            GetTransactionIDListDataRequest getTransactionIDListRequest = new GetTransactionIDListDataRequest(string.Join(",", transactionIDListOrig), QueryResultSettings.AllRecords);
            var getTransactionIDListDataResponse = await context.ExecuteAsync<GetTransactionIDListDataResponse>(getTransactionIDListRequest).ConfigureAwait(false);

            // Get all transaction ids with Receipt Printed 
            List<string> transactionIDListPrinted = getTransactionIDListDataResponse.TransactionIDList.ToList<string>();

            // Get all transaction ids with Receipt Not Printed 
            List<string> unPrintedTransactionIDList = transactionIDListOrig.Where(t1 => !transactionIDListPrinted.Any(t2 => t2.Equals(t1))).ToList();

            IEnumerable <Transaction> transactions = response.Transactions.Where<Transaction>(t =>
            {
                return unPrintedTransactionIDList.Any(s => t.Id.Equals(s));
            });

            PagedResult<Transaction> filteredTransactions =
                new PagedResult<Transaction>(new System.Collections.ObjectModel.ReadOnlyCollection<Transaction>((IList<Transaction>)transactions));

            return filteredTransactions;
        }
    }
}
