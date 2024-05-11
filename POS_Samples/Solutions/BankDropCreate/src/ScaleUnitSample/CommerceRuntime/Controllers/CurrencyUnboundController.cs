namespace Contoso.CommerceRuntime.Controllers
{
    using System.Runtime;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class TenderCountingUnboundController : IController
    {
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<CurrencyAmount>> GetCurrenciesAmounExt(IEndpointContext context, string currencyCode, decimal amount, QueryResultSettings queryResultSetting)
        {
            ThrowIf.Null(queryResultSetting, nameof(queryResultSetting));

            var request = new GetChannelCurrencyAmountRequest
            {
                CurrenciesToConvert = new[] { new CurrencyRequest { AmountToConvert = amount, CurrencyCode = currencyCode } },
                IsTotalToBeCalculated = false,
                QueryResultSettings = queryResultSetting,
            };

            var response = await context.ExecuteAsync<GetChannelCurrencyAmountResponse>(request).ConfigureAwait(false);

            return response.CurrencyAmounts;
        }


        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public  async Task<PagedResult<CashDeclaration>> GetCashDeclarationsExt(IEndpointContext context, long currentChannelId, QueryResultSettings queryResultSetting)
        {
            ThrowIf.Null(queryResultSetting, nameof(queryResultSetting));

            var request = new GetChannelCashDeclarationDataRequest(currentChannelId, queryResultSetting);
            var response = await context.ExecuteAsync<EntityDataServiceResponse<CashDeclaration>>(request).ConfigureAwait(false);
            return response.PagedEntityCollection;
        }

        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<DropAndDeclareTransaction> CreateDropAndDeclareTransactionExt(IEndpointContext context, DropAndDeclareTransaction transaction)
        {
            ThrowIf.Null(transaction, nameof(transaction));

            ThrowIf.Null(transaction, nameof(transaction));

            var serviceRequest = new SaveDropAndDeclareServiceRequest()
            {
                TransactionId = transaction.Id,
                BeginDateTime = transaction.BeginDateTime,
                Description = transaction.Description,
                TenderDetails = transaction.TenderDetails,
                ExtensibleTransactionType = transaction.ExtensibleTransactionType,
                ShiftId = transaction.ShiftId,
                ShiftTerminalId = transaction.ShiftTerminalId,
                ReasonCodeLines = transaction.ReasonCodeLines,
                TransactionSatus = transaction.TransactionStatus,
                ToSafe = transaction.ToSafe,
                FromSafe = transaction.FromSafe,
                FromShiftTerminalId = transaction.FromShiftTerminalId,
                ToShiftTerminalId = transaction.ToShiftTerminalId,
                FromShiftId = transaction.FromShiftId,
                ToShiftId = transaction.ToShiftId,
                TransactionSourceContextType = transaction.TransactionSourceContextType,
                TransactionDestinationContextType = transaction.TransactionDestinationContextType,
            };

            var serviceResponse = await context.ExecuteAsync<SaveDropAndDeclareServiceResponse>(serviceRequest).ConfigureAwait(false);
            return serviceResponse.TenderDropAndDeclareOperation;
        }
    }
}
