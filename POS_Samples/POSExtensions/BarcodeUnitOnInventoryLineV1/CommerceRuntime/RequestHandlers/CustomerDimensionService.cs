namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;

    public class CustomerDimensionService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetCustomerDimensionRequest)
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(GetCustomerDimensionRequest))
            {
                return this.GetCustomerDimension((GetCustomerDimensionRequest)request);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                throw new NotSupportedException(message);
            }
        }

        private async Task<Response> GetCustomerDimension(GetCustomerDimensionRequest request)
        {
            InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                 "avanadeGetCustomerFinancialDimension",
                 request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId,
                 request.AccountNum,
                 request.DimensionAttributeName
                );

            InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
            var results = response.Result;
            string dimensionValue = (string)results[0];
            return new GetCustomerDimensionResponse(dimensionValue);
        }
    }
}
