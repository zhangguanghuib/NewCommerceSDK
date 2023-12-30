
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

            var request = new SearchJournalTransactionsServiceRequest(searchCriteria, queryResultSettings);
            var response = await context.ExecuteAsync<SearchJournalTransactionsServiceResponse>(request).ConfigureAwait(false);

            //IEnumerable<Transaction> transactions = 
            //response.Transactions.Where<Transaction>(t => !(t.ExtensionProperties.Where<CommerceProperty>(p => p.Key.Equals("ISRECIPT")).Any()) && t.ExtensionProperties.Where<CommerceProperty>(p => p.Key.Equals("ISRECIPT") && p.Value.IntegerValue.Equals(0)).Any());

            //SearchJournalTransactionsServiceResponse filteredResponse = new SearchJournalTransactionsServiceResponse((PagedResult<Transaction>)transactions);
            //return response.Transactions;

            IEnumerable<Transaction> transactions = response.Transactions.Where<Transaction>(t => 
            {
                return !(t.ExtensionProperties.Any(p => p.Key.Equals("ISRECIPTPRINTED"))) || t.ExtensionProperties.Any(p => p.Key.Equals("ISRECIPTPRINTED") && p.Value.IntegerValue.Equals(0));
            });

            return response.Transactions;

            //return (PagedResult<Transaction>)transactions;
        }
    }
}
