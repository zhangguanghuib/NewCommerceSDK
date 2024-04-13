namespace Contoso.CommerceRuntime.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    public class CurrencyUnboundController : IController
    {
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<CurrencyAmount>> GetCurrenciesAmounExt(IEndpointContext context, string currencyCode, decimal amount, QueryResultSettings queryResultSettingstring)
        {
            ThrowIf.Null(queryResultSettingstring, nameof(queryResultSettingstring));

            var request = new GetChannelCurrencyAmountRequest
            {
                CurrenciesToConvert = new[] { new CurrencyRequest { AmountToConvert = amount, CurrencyCode = currencyCode } },
                IsTotalToBeCalculated = false,
                QueryResultSettings = queryResultSettingstring,
            };

            var response = await context.ExecuteAsync<GetChannelCurrencyAmountResponse>(request).ConfigureAwait(false);

            return response.CurrencyAmounts;
        }
    }
}
