## Complex POS UI Development Sample (Like Bank Drop)

1. **Overall Background:**

   As you know in the age of Retail SDK, MPOS or Cloud CPOS, the POS extension is totally integrated with the Out-of-Box CPOS or MPOS, so when develop engineers develop  POS View, POS dialog,  they even can just utilize the OOB POS Control in their customization code, it is working fine.
   But moving forward to Commerce SDK,  when develop Store Commerce Extensions, there are a lot of new limitations, the major one is a lot PosUI Control is no longer usable, one typical sampel the Numpad. In Commerce SDK  we have to implement Numpad in a totally different way instead of just putting the Numpad Control on the POS view. More official document can be found  [Use POS controls in extensions](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-extension/controls-pos-extension)
   From the above offical doc, you can see only these POS Controls are supported by PosApi:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/fc6df3dd-c2d7-429e-abed-a9f7f623ced0)

## Video Links:

 - [Customer Order In-Store Pickup Notification](https://microsoftapc-my.sharepoint.com/personal/guazha_microsoft_com/_layouts/15/stream.aspx?id=%2Fpersonal%2Fguazha%5Fmicrosoft%5Fcom%2FDocuments%2FRecordings%2FCustomer%20Order%20In%2DStore%20Pickup%20Notification%2D20240417%5F225004%2DMeeting%20Recording%2Emp4&referrer=StreamWebApp%2EWeb&referrerScenario=AddressBarCopied%2Eview&ga=1)

- [YouTube Video](https://www.youtube.com/watch?v=YoEazXIJhB0)

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
7. **All Source code can be found <br/>**
[Source code](https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/Notification-Sample)
