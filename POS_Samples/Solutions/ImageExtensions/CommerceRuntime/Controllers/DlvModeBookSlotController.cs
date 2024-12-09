namespace Contoso.GasStationSample.CommerceRuntime.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Contoso.GasStationSample.CommerceRuntime.Entities;
    using Contoso.GasStationSample.CommerceRuntime.Messages;

    [RoutePrefix("DlvModeBookSlot")]
    [BindEntity(typeof(DlvModeBookSlot))]
    public class DlvModeBookSlotController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<DlvModeBookSlot>> GetDlvModeBookSlots(IEndpointContext context, string DlvModeCode, QueryResultSettings queryResultSettings)
        {
            var request = new GetDlvModeBookSlotsRequest(DlvModeCode) { QueryResultSettings = queryResultSettings};
            var response = await context.ExecuteAsync<GetDlvModeBookSlotsResponse>(request).ConfigureAwait(false);
            return response.DlvModeBookSlots;
        }
    }
}
