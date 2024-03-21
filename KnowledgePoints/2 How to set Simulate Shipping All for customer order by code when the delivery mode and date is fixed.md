##  How to simulate Shipping All customer order by code when the delivery mode and date is fixed to simplify the user operation?

1.Background:<br/>
For some retail customer,  they have only one fixed Shipping Method for customer like Standard, and the shipping date is also fixed like two days later, so they don't need click "Ship All" button, to choose Shipping Address because that is always customer's primary address, and nor choose Shipping Method and Shipping Date.<br/>
So these steps seems too complex for them and they really don't need that:<br/>
<img width="128" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c424688c-968d-4480-ab54-61f8c4cc5ae4"><br/>
<img width="523" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/be9cf250-876e-4099-aa0e-a700f768ab8a"><br/>
<img width="251" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/5620f2da-dfbd-459f-a554-752a58492765"><br/>

What customer want is:<br/>
. Add customer account
. Add product
. Create customer order:<br/>
 <img width="121" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/cb809bfd-bee8-4551-95be-2b138069a359">
. Click Pay Cash/Card and Checkout:<br/>
 <img width="244" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/49ecc7d2-0d31-4815-83fb-8eaa3e864b38"><br/>
. They expected the Shipping Address, Delivery Mode and Delivery Date will be automatically set.

2. The idea to fix this issue:<br/>
   . By code set the transaction level delivery specification and line level specification:<br/>
   ```cs
   public async Task<Cart> updateLinesDeliverySpecifications(IEndpointContext context, string cartId, SalesTransaction transaction, Address shippingAddress)
   {
       var lineDeliverySpecs = this.CreateLineDeliverySpecifications(transaction, shippingAddress);
       var updateDeliveryRequest = new UpdateDeliverySpecificationsRequest(cartId, lineDeliverySpecs);
 
       Cart cart = (await context.ExecuteAsync<UpdateDeliverySpecificationsResponse>(updateDeliveryRequest).ConfigureAwait(false)).Cart;
 
       return cart;
   }
 
   public  IEnumerable<LineDeliverySpecification> CreateLineDeliverySpecifications(SalesTransaction transaction, Address shipAddress)
   {
       var lineDeliverySpecifications = new Collection<LineDeliverySpecification>();
       foreach (var salesLine in transaction.SalesLines)
       {
           var deliverySpec = new LineDeliverySpecification()
           {
               LineId = salesLine.LineId,
               DeliverySpecification = new DeliverySpecification()
               {
                   DeliveryModeId = "99",
                   DeliveryAddress = shipAddress,
                   DeliveryPreferenceType = DeliveryPreferenceType.ShipToAddress,
                   RequestedDeliveryDate = System.DateTimeOffset.Now.AddDays(2),
               },
           };
           lineDeliverySpecifications.Add(deliverySpec);
       }
 
       return lineDeliverySpecifications;
   }
   ```
   . By this code set the header level delivery specification:<br/>
    ```cs
    public async Task<(Cart, SalesTransaction)> updateOrderDeliverySpecifications(IEndpointContext context, SalesTransaction salesTransaction, Address shippingAddress)
    {
        var newDeliveryInformation = new DeliverySpecification()
        {
            DeliveryModeId = "99",
            DeliveryAddress = shippingAddress,
            DeliveryPreferenceType = DeliveryPreferenceType.ShipToAddress,
            RequestedDeliveryDate = System.DateTimeOffset.Now.AddDays(2),
        };
 
        var updateDeliveryRequest = new UpdateDeliverySpecificationsRequest(salesTransaction, newDeliveryInformation);
 
        salesTransaction = (await context.ExecuteAsync<UpdateDeliverySpecificationsResponse>(updateDeliveryRequest).ConfigureAwait(false)).ExistingCart;
        salesTransaction.LongVersion = (await context.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(salesTransaction)).ConfigureAwait(false)).Entity;
 
        ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(salesTransaction);
        Cart cart = (await context.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;
        return (cart, salesTransaction);
    }
    ```
    .  Controller level API :<br/>
    ```cs
    [HttpGet]
    [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
    public async Task<Cart> SimplePingGet(IEndpointContext context, string cartId)
    {
        var getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
        GetCartServiceResponse getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
        SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();

        if(transaction.CartType == CartType.CustomerOrder && 
            (string.IsNullOrEmpty(transaction.DeliveryMode) || transaction.SalesLines.Any(sl => string.IsNullOrEmpty(sl.DeliveryMode))))
        {
            GetCustomersServiceRequest getCustomersServiceRequest = new GetCustomersServiceRequest(QueryResultSettings.SingleRecord, transaction.CustomerId, SearchLocation.Local);
            GetCustomersServiceResponse getCustomersServiceResponse = await context.ExecuteAsync<GetCustomersServiceResponse>(getCustomersServiceRequest).ConfigureAwait(false);
            Address shippingAddress = getCustomersServiceResponse.Customers.FirstOrDefault().GetPrimaryAddress();

            (Cart cart, transaction) = await this.updateOrderDeliverySpecifications(context, transaction, shippingAddress).ConfigureAwait(false);

            cart = await this.updateLinesDeliverySpecifications(context, cartId, transaction, shippingAddress).ConfigureAwait(false);
            return cart;
        }
        else
        {
           return getCartServiceResponse.Carts.FirstOrDefault();
        }
    }
    ```
2.  In POS extension, when click Pay Cash or Pay Card,  the above logic will be call to set the Delivery Specification for the customer order by code, the will simplify the user operation and improve user experience:
```ts
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/OperationTriggers"
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import * as Messages from "../DataService/DataServiceRequests.g";

export default class PreOperationTrigger extends Triggers.PreOperationTrigger {
    public execute(options: Triggers.IOperationTriggerOptions): Promise<Commerce.Client.Entities.ICancelable> {
        if (ObjectExtensions.isNullOrUndefined(options)) {
            // This will never happen, but is included to demonstrate how to return a rejected promise when validation fails.
            let error: ClientEntities.ExtensionError
                = new ClientEntities.ExtensionError("The options provided to the PreTenderPaymentTrigger were invalid. Please select a product and try again.");
            return Promise.reject(error);
        } 

        if (options.operationRequest.operationId == ProxyEntities.RetailOperation.PayCash
            || options.operationRequest.operationId == ProxyEntities.RetailOperation.PayCard) {

            let correlationId: string = this.context.logger.getNewCorrelationId();
            let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

            return this.context.runtime.executeAsync(getCurrentCartClientRequest).then((response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                return this.context.runtime.executeAsync(new Messages.StoreOperations.SimplePingGetRequest(response.data.result.Id));
            }).then(pingGetResponse => {
                let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest();
                return this.context.runtime.executeAsync(refreshCartClientRequest);
            }).then(() => {
                return Promise.resolve({ canceled: false });
            });
        }

        return Promise.resolve({ canceled: false });    
    }
}
```
4.  The complete project to test that is:
   https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/POSExtensions/CreateCustomerOrderByCode/src/ScaleUnitSample/CommerceRuntime/Controllers/UnboundController.cs

https://github.com/zhangguanghuib/NewCommerceSDK/blob/main/POS_Samples/POSExtensions/CreateCustomerOrderByCode/src/ScaleUnitSample/POS/TriggerHandlers/PreOperationTrigger.ts
