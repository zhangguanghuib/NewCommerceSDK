namespace Contoso.GasStationSample.CommerceRuntime.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Contoso.GasStationSample.CommerceRuntime.Messages;

    public class BarcodeController : IController
    {
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public async Task<string> GetBarcodeBase64String(IEndpointContext context, string barCodeString)
        {
            var getBarCodeImageRequest = new GetBarCodeImageRequest(barCodeString);
            GetBarCodeImageResponse response = await context.ExecuteAsync<GetBarCodeImageResponse>(getBarCodeImageRequest).ConfigureAwait(false);
            return response.EncodedBarCode;
        }
    }
}
