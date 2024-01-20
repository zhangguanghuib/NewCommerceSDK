namespace Contoso.GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class NonBindableOperationController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous)]
        public async Task<string> RetrieveStaffPwd(IEndpointContext context, string staffId)
        {
            GetUserDefinedSecretStringValueServiceRequest keyVaultRequest = new GetUserDefinedSecretStringValueServiceRequest(staffId);
            GetUserDefinedSecretStringValueServiceResponse keyVaultResponse =
                await context.ExecuteAsync<GetUserDefinedSecretStringValueServiceResponse>(keyVaultRequest).ConfigureAwait(false);
            
            return keyVaultResponse.SecretStringValue;
        }

    }
}
