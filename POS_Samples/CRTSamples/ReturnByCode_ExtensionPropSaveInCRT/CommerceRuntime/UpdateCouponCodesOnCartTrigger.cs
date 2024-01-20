namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
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

            if ((request is UpdateCouponCodesOnCartServiceRequest) && (response is UpdateCartServiceResponse))
            {
                UpdateCartServiceResponse updateCartServiceResponse = (UpdateCartServiceResponse)response;

                GetCartServiceRequest getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(updateCartServiceResponse.Cart.Id), QueryResultSettings.SingleRecord);
                GetCartServiceResponse getCartServiceResponse = await request.RequestContext.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
                SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();

                CommerceProperty couponCommercePropMinAmount = new CommerceProperty();
                couponCommercePropMinAmount.Key = "MinAmount";
                couponCommercePropMinAmount.Value = new CommercePropertyValue();
                couponCommercePropMinAmount.Value.IntegerValue = 10;

                CommerceProperty couponCommercePropVoucherCode = new CommerceProperty();
                couponCommercePropVoucherCode.Key = "VoucherCode";
                couponCommercePropVoucherCode.Value = new CommercePropertyValue();
                couponCommercePropVoucherCode.Value.StringValue = "VoucherCode";
                if (transaction.Coupons.Count > 0)
                {
                    Coupon coupon = transaction.Coupons[transaction.Coupons.Count - 1];
                    coupon.ExtensionProperties = new List<CommerceProperty>
                    {
                        couponCommercePropMinAmount,
                        couponCommercePropVoucherCode
                    };

                    transaction.LongVersion = (await request.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(transaction)).ConfigureAwait(false)).Entity;

                    ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(transaction);
                    Cart cart = (await request.RequestContext.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;

                }
            }
        }

        public  Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }
    }
}


