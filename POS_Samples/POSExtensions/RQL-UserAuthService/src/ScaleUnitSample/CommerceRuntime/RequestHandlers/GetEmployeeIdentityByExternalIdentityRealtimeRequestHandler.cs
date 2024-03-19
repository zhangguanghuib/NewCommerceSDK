using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class GetEmployeeIdentityByExternalIdentityRealtimeRequestHandler : SingleAsyncRequestHandler<GetEmployeeIdentityByExternalIdentityRealtimeRequest>
    {
        public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities
            = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();


        protected override async Task<Response> Process(GetEmployeeIdentityByExternalIdentityRealtimeRequest request)
        {
            ThrowIf.Null(request, "request");

            GetEmployeeIdentityByExternalIdentityRealtimeResponse val;

            try
            {
                if (request == null)
                {
                    var exception = new ArgumentNullException("request");
                   
                    throw exception;
                }
                else if (request is GetEmployeeIdentityByExternalIdentityRealtimeRequest)
                {
                    GetEmployeeIdentityByExternalIdentityRealtimeRequest employeeRequest = request as GetEmployeeIdentityByExternalIdentityRealtimeRequest;

                    if (foundIdentities.TryGetValue(employeeRequest.ExternalIdentityId, out val))
                    {    
                        return val;
                    }
                    else
                    {
                        var response = await this.ExecuteNextAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request).ConfigureAwait(false);
                        foundIdentities.GetOrAdd(employeeRequest.ExternalIdentityId, response);
                        return response;
                    }
                }
            }
            finally
            {
                //logger.StopOperation(operation);
            }

            // Do no checks.
            return NullResponse.Instance;
        }
    }
}
