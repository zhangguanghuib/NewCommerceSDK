using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System.Collections.Concurrent;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class GetEmployeeIdentityByExternalIdentityRealtimeRequestHandlerV2
        : SingleAsyncRequestHandler<GetEmployeeIdentityByExternalIdentityRealtimeRequest>
    {
        public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();

        protected async override Task<Response> Process(GetEmployeeIdentityByExternalIdentityRealtimeRequest request)
        {
            ThrowIf.Null(request, "request");

            // The extension should do nothing If fiscal registration is enabled and legacy extension were used to run registration process.
            if (!string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
            {
                return NotHandledResponse.Instance;
            }

            GetEmployeeIdentityByExternalIdentityRealtimeResponse val;

            if (foundIdentities.TryGetValue(request.ExternalIdentityId, out val))
            {
                return val;
            }
            else
            {
                // Execute original logic.
                var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
                var response = await request.RequestContext.Runtime.ExecuteAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);

                foundIdentities.GetOrAdd(request.ExternalIdentityId, response);

                return response;
            }
        }
    }
}
