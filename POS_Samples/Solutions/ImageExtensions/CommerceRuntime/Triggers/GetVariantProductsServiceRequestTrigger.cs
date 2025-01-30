namespace Contoso.GasStationSample.CommerceRuntime.Triggers
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetVariantProductsServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetVariantProductsServiceRequest) };
            }
        }
        /// <summary>
        /// Post trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");
            if ((request is GetVariantProductsServiceRequest) && (response is GetProductsServiceResponse))
            {
                var getVariantProductsServiceResponse = response as GetProductsServiceResponse;
                PagedResult<SimpleProduct> pagedResult = getVariantProductsServiceResponse.Products;
                foreach (SimpleProduct item in pagedResult)
                {
                    if (item.PrimaryImageUrl.Length >= 12)
                    {
                        //item.PrimaryImageUrl = "Products/" + item.ItemId + item.PrimaryImageUrl.Substring(item.PrimaryImageUrl.Length - 12);
                        string productMasterImageUrl = "Products/" + item.ItemId + item.PrimaryImageUrl.Substring(item.PrimaryImageUrl.Length - 12);
                        CommerceProperty commerceProperty = new CommerceProperty("ProductMasterImageUrl", productMasterImageUrl);
                        item.ExtensionProperties.Add(commerceProperty);
                    }
                }
            }
            await Task.CompletedTask.ConfigureAwait(false);
        }
        /// <summary>
        /// Pre trigger code
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
