using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class CustomSaveCartTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SaveCartRequest) };
            }
        }

        public  Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        public async Task OnExecuting(Request request)
        {
            ThrowIf.Null(request, "request");
            Type requestedType = request.GetType();
            string result = null;
            if (requestedType == typeof(SaveCartRequest))
            {
                string staffId = request.RequestContext.GetPrincipal().StaffId;
                GetUserDefinedSecretStringValueServiceRequest keyVaultRequest = new GetUserDefinedSecretStringValueServiceRequest(staffId);
                GetUserDefinedSecretStringValueServiceResponse keyVaultResponse = await request.RequestContext.ExecuteAsync<GetUserDefinedSecretStringValueServiceResponse>(keyVaultRequest).ConfigureAwait(false);
                result = keyVaultResponse.SecretStringValue;
            }
        }
    }
}
