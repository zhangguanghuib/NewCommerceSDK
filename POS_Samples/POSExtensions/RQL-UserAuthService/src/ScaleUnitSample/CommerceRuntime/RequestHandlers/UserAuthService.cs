using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class UserAuthService : IRequestHandlerAsync
    {
        public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities 
            = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new Type[]
                {
                        //typeof(CheckAccessServiceRequest),
                        //typeof(CheckAccessToCartServiceRequest),
                        //typeof(CheckAccessToCustomerAccountServiceRequest),
                        typeof(GetEmployeeIdentityByExternalIdentityRealtimeRequest)
                };
            }
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        public async Task<Response> Execute(Request request)
        {
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
                        //return foundIdentities[employeeRequest.ExternalIdentityId];
                        return val;
                    }
                    else
                    {
                        var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
                        var response = await request.RequestContext.Runtime.ExecuteAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);

                        foundIdentities.GetOrAdd(employeeRequest.ExternalIdentityId, response);
                        return response;
                    }
                }
            }
            finally
            {
                Console.WriteLine("Finally");
            }

            // Do no checks.
            return NullResponse.Instance;
        }
    }
}

