namespace Contoso.GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

   [RoutePrefix("Carts")]
   [BindEntity(typeof(Cart))]
    public partial class CartsController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<List<Coupon>> GetAppliedCoupons(IEndpointContext context, string cartId, QueryResultSettings queryResultSettings)
        {
            GetCartServiceRequest getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
            GetCartServiceResponse getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();
            return this.GetAppliedCouponsFromSalesTransaction(transaction);
        }

        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<Cart> RemoveAppliedCouponsFromCart(IEndpointContext context, string cartId, IEnumerable<string> couponCodes)
        {
            UpdateCouponCodesOnCartServiceRequest request = new UpdateCouponCodesOnCartServiceRequest(cartId, couponCodes, CouponCodesOperation.Remove);
            UpdateCartServiceResponse response = await context.ExecuteAsync<UpdateCartServiceResponse>(request).ConfigureAwait(false);

            return response.Cart;
        }


        private List<Coupon> GetAppliedCouponsFromSalesTransaction(SalesTransaction transaction)
        {
            List<Coupon> appliedCoupons = new List<Coupon>();
            foreach (SalesLine salesLine in transaction.SalesLines)
            {
                if (!salesLine.IsVoided && !salesLine.IsReturnByReceipt && salesLine.DiscountAmount > 0)
                {
                    foreach (DiscountLine discountLine in salesLine.DiscountLines)
                    {
                        if (!string.IsNullOrWhiteSpace(discountLine.DiscountCode)
                            && transaction.Coupons.Any(c => c.Code.Equals(discountLine.DiscountCode, StringComparison.OrdinalIgnoreCase) && c.DiscountOfferId.Equals(discountLine.OfferId, StringComparison.OrdinalIgnoreCase))
                            && !appliedCoupons.Any(c => c.Code.Equals(discountLine.DiscountCode, StringComparison.OrdinalIgnoreCase) && c.DiscountOfferId.Equals(discountLine.OfferId, StringComparison.OrdinalIgnoreCase)))
                        {
                            appliedCoupons.Add(transaction.Coupons.Single(c => c.Code.Equals(discountLine.DiscountCode, StringComparison.OrdinalIgnoreCase) && c.DiscountOfferId.Equals(discountLine.OfferId, StringComparison.OrdinalIgnoreCase)));
                        }
                    }
                }
            }

            return appliedCoupons;
        }
    }
}
