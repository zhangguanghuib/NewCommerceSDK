## Customer Order (Call Center Order) In-Store Pickup Notification

1. **Background:**

   As you know, D365 Commerce Solution supports Customer order (Call Center Order) In-Store Pick-up. Some Store Managers want to get notifications to show new orders created that need pickup from the current store. The final function is like this:
   
   ![Customer Order Notification](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4148ff19-0324-4160-9cf6-2846852eb428)<br/>

   The video can be found:
   https://microsoftapc-my.sharepoint.com/personal/guazha_microsoft_com/_layouts/15/stream.aspx?id=%2Fpersonal%2Fguazha%5Fmicrosoft%5Fcom%2FDocuments%2FRecordings%2FCustomer%20Order%20In%2DStore%20Pickup%20Notification%2D20240417%5F225004%2DMeeting%20Recording%2Emp4&referrer=StreamWebApp%2EWeb&referrerScenario=AddressBarCopied%2Eview&ga=1<br/>
   https://www.youtube.com/watch?v=YoEazXIJhB0

3. **Implementation details:**
   
   - Add Custom Control on Screen Layout Designer
     ![Image 1](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/68f80dad-79cd-42a3-8ece-1149f8e7e587)
     ![Image 2](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8d92e1cd-2c70-48dc-b279-b0e728afe1a1)

4. **Install Scale Unit Extension Package**

5. **Install Store Commerce Extension Package**

6. **Technical Point:**
   
   - Periodically check if there are new customer orders that need pickup from the current store.
     ```typescript
     public init(state: ICartViewCustomControlState): void {
        this._state = state;

        this.context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
            .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                return response.data.result;
            }).then((deviceConfiguration: ProxyEntities.DeviceConfiguration)
                : Promise<void> => {
                this.currentChannelId = deviceConfiguration.ChannelId;
                return Promise.resolve();
            });

        this.intervalId = setInterval((): void => {
            let request: StoreOperations.GetPickupOrdersCreatedFromOtherStoreRequest<StoreOperations.GetPickupOrdersCreatedFromOtherStoreResponse>
                = new StoreOperations.GetPickupOrdersCreatedFromOtherStoreRequest(this.currentChannelId);

            this.context.runtime.executeAsync(request).then((getPickupOrdersCreatedFromOtherStoreResponse: ClientEntities.ICancelableDataResult<StoreOperations.GetPickupOrdersCreatedFromOtherStoreResponse>):
                Promise<string> => {
                if (getPickupOrdersCreatedFromOtherStoreResponse.canceled) {
                    return Promise.resolve("");
                } else {
                    if (getPickupOrdersCreatedFromOtherStoreResponse.data.result.length <= 0) {
                        return Promise.resolve("");
                    } else {
                        let orderNoStr: string = '';
                        getPickupOrdersCreatedFromOtherStoreResponse.data.result.forEach((order: ProxyEntities.SalesOrder) => {
                            orderNoStr += order.SalesId + " ";
                        });
                        console.log(orderNoStr);
                        return Promise.resolve(orderNoStr);
                    }
                }
            }).then((orderlist: string) => {
                this.orderNoList(orderlist);
                let divRedDots: NodeListOf<Element> = document.querySelectorAll('.reddot1');

                if (orderlist.length > 0) {
                    if (!this._isLoaderVisible()) {
                        this._isLoaderVisible(true);
                    }

                    divRedDots.forEach((element: Element) => {
                        let divRedDot: HTMLDivElement = element as HTMLDivElement;
                        divRedDot.style.display = "block";

                    });
                } else {
                    divRedDots.forEach((element: Element) => {
                        let divRedDot: HTMLDivElement = element as HTMLDivElement;
                        divRedDot.style.display = "none";
                    });
                }

                setTimeout((): void => {
                    this._isLoaderVisible(false);
                }, 4000);
            });
        }, 10000);
     }
     ```

   - Scale Unit API:
     ```csharp
     public partial class NonBindableOperationCustomController : IController
     {
            ThrowIf.Null(settings, nameof(settings));

            // Only get the orders created last 10 minutes
            OrderSearchCriteria criteria = new OrderSearchCriteria();
            criteria.FulfillmentTypes.Add(FulfillmentOperationType.Pickup);
            criteria.StartDateTime = DateTimeOffset.UtcNow.AddDays(-1);
            criteria.EndDateTime = DateTimeOffset.UtcNow;

            var request = new SearchOrdersServiceRequest(criteria, settings);
            var response = await context.ExecuteAsync<SearchOrdersServiceResponse>(request).ConfigureAwait(false);

            var pagedOrders = response.Orders;
            //Only take the orders created from other store:
            //var pagedOrders = response.Orders.Where(o => o.ChannelId != currentChannelId).AsPagedResult<SalesOrder>();

            return pagedOrders;
     }
     ```
