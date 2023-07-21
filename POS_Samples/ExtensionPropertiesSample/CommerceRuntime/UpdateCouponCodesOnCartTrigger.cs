namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class UpdateCouponCodesOnCartTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(UpdateCouponCodesOnCartServiceRequest) };
            }
        }

        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            if((request is UpdateCouponCodesOnCartServiceRequest) && (response is UpdateCartServiceResponse))
            {
                UpdateCartServiceResponse updateCartServiceResponse = (UpdateCartServiceResponse) response;

                CommerceProperty couponCommercePropMinAmount = new CommerceProperty();
                couponCommercePropMinAmount.Key = "MinAmount";
                couponCommercePropMinAmount.Value = new CommercePropertyValue();
                couponCommercePropMinAmount.Value.IntegerValue = 10;

                CommerceProperty couponCommercePropVoucherCode = new CommerceProperty();
                couponCommercePropVoucherCode.Key = "VoucherCode";
                couponCommercePropVoucherCode.Value = new CommercePropertyValue();
                couponCommercePropVoucherCode.Value.StringValue = "VoucherCode";
                if (updateCartServiceResponse.Cart.Coupons.Count > 0)
                {
                    Coupon coupon = updateCartServiceResponse.Cart.Coupons[updateCartServiceResponse.Cart.Coupons.Count - 1];
                    coupon.ExtensionProperties.Add(couponCommercePropMinAmount);
                    coupon.ExtensionProperties.Add(couponCommercePropVoucherCode);
                    var updateCartRequest = new UpdateCartRequest(updateCartServiceResponse.Cart);
                    var saveCartResponse = await request.RequestContext.ExecuteAsync<SaveCartResponse>(updateCartRequest).ConfigureAwait(false);
                 }
            }
        }

        public  Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }
    }
}


