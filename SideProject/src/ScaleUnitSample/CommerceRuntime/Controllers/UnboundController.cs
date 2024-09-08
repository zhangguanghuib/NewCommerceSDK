namespace Contoso.CommerceRuntime.Controllers
{
    using System.IO;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Localization.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Bmp;
    using CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using System.Collections.Generic;
    using global::CommerceRuntime.Messages;

    //using static System.Net.Mime.MediaTypeNames;

    /// <summary>
    /// An extension controller to handle requests to the StoreHours entity set.
    /// </summary>
    public class UnboundController : IController
    {
        /// <summary>
        /// A simple GET endpoint to demonstrate GET endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public Task<bool> SimplePingGet()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// A simple POST endpoint to demonstrate POST endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public Task<bool> SimplePingPost()
        {
            return Task.FromResult(true);
        }

        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public async Task<string> GetQRCodeBase64String(IEndpointContext context, string qrcodeurl)
        {
            ContosoGetStoreLocationsDataRequest contosoGetStoreLocationsDataRequest = new ContosoGetStoreLocationsDataRequest(QueryResultSettings.AllRecords);
            IEnumerable<OrgUnitLocation> storeDetails = (await context.ExecuteAsync<EntityDataServiceResponse<OrgUnitLocation>>(contosoGetStoreLocationsDataRequest)
                .ConfigureAwait(false)).PagedEntityCollection.Results;

            var getQRCodeImageRequest = new GetQRCodeImageRequest(qrcodeurl);
            GetQRCodeImageResponse response = await context.ExecuteAsync<GetQRCodeImageResponse>(getQRCodeImageRequest).ConfigureAwait(false);
            return response.EncodedQrCode;
        }
    }
}
