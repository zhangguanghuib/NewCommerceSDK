using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
using System.Threading.Tasks;

namespace SAG.HardwareStation.PaymentTerminal
{
    [RoutePrefix("SAGPAYMENTTERMINAL")]
    public class SAGPaymentTerminalController: IController
    {
        public async Task<bool> IsReady(IEndpointContext context)
        {
            bool ret = false;

            return await Task.FromResult(ret).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<PaymentTerminalResponse> ProcessDBSPaymentTerminal(DBS.DataContract.DBSRequest request, IEndpointContext context)
        {
            //DBS.DataContract.DBSResponse response = new DBS.DataContract.DBSResponse();
            PaymentTerminalResponse response;

            response = await context.ExecuteAsync<PaymentTerminalResponse>(request).ConfigureAwait(false);

            return await Task.FromResult(response).ConfigureAwait(false);
        }
        
    }
}
