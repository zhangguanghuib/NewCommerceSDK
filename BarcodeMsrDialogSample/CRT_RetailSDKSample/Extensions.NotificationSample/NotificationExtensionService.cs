using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRT_RetailSDKSample.Extensions.NotificationSample
{
    public class NotificationExtensionService : SingleAsyncRequestHandler<GetNotificationsExtensionServiceRequest>
    {
        protected override async Task<Response> Process(GetNotificationsExtensionServiceRequest request)
        {
            ThrowIf.Null(request, nameof(request));

            NotificationDetailCollection details= new NotificationDetailCollection();
            DateTimeOffset lastNotificationDateTime = DateTimeOffset.Now;

            if(request.SubscribedOperation == RetailOperation.CreateCustomerOrder)
            {
                NotificationDetail detail = new NotificationDetail()
                {
                    DisplayText = "Add coupon",

                    ItemCount =1,
                    
                    LastUpdatedDateTime = lastNotificationDateTime,

                    IsSuccess= true,
                };

                details.Add(detail);
            }
            else if(request.SubscribedOperation == RetailOperation.VoidTransaction)
            {
                NotificationDetail detail = new NotificationDetail()
                {
                    DisplayText = "Remove coupon",
                    ItemCount = 1,
                    LastUpdatedDateTime = lastNotificationDateTime,
                    IsSuccess = true
                };

                details.Add(detail);

                NotificationDetail detail2 = new NotificationDetail()
                {
                    DisplayText = "Remove coupon",
                    ItemCount = 1,
                    LastUpdatedDateTime = lastNotificationDateTime,
                    IsSuccess = true
                };
                details.Add(detail2);
            }

            var serviceResponse = new GetNotificationsExtensionServiceResponse(details);
            return await Task.FromResult(serviceResponse).ConfigureAwait(false);
        }
    }
}
