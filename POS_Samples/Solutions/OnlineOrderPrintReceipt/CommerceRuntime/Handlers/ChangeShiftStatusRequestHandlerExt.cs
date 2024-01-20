using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime.Handlers
{
    public sealed class ChangeShiftStatusRequestHandlerExt : SingleAsyncRequestHandler<ChangeShiftStatusRequest>
    {
        protected override async Task<Response> Process(ChangeShiftStatusRequest request)
        {
            ThrowIf.Null(request, "request");

            if (request.ToStatus == ShiftStatus.Closed)
            {
                string hardwareProfileId = request.RequestContext.GetDeviceConfiguration().HardwareProfile;

                GetHardwareProfileDataRequest getHardwareProfileDataRequest = new GetHardwareProfileDataRequest(hardwareProfileId, QueryResultSettings.SingleRecord);
                HardwareProfile hardwareProfile =
                    (await request.RequestContext.Runtime.ExecuteAsync<SingleEntityDataServiceResponse<HardwareProfile>>(getHardwareProfileDataRequest, request.RequestContext).ConfigureAwait(false)).Entity;

                bool noneCashDrawer = hardwareProfile.CashDrawers.All<HardwareProfileCashDrawer>(cashDrawer => cashDrawer.DeviceType == DeviceType.None);
                if (noneCashDrawer)
                {
                    request.CanForceClose = true;
                }
            }

            var response = await this.ExecuteNextAsync<ChangeShiftStatusResponse>(request).ConfigureAwait(false);

            return response;
        }
    }
}
