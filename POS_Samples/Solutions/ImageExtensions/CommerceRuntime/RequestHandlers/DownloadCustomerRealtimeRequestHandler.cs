
namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    public sealed class DownloadCustomerRealtimeRequestHandler : SingleAsyncRequestHandler<DownloadCustomerRealtimeRequest>
    {
        protected override Task<Response> Process(DownloadCustomerRealtimeRequest request)
        {
            return Task.FromResult<Response>(NullResponse.Instance);
        }
    }
}
