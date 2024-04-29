## Complex POS UI Development Sample (Like Bank Drop)

1. **Overall Background:**

   <p>As you know in the age of Retail SDK, MPOS or Cloud CPOS, the POS extension is totally integrated with the Out-of-Box CPOS or MPOS, so when develop engineers develop  POS View, POS dialog,  they even can just utilize the OOB POS Control in their customization code, it is working fine.</p>
   <p>But moving forward to Commerce SDK,  when develop Store Commerce Extensions, there are a lot of new limitations, the major one is a lot PosUI Control is no longer usable, one typical sampel the Numpad. In Commerce SDK  we have to implement Numpad in a totally different way instead of just putting the Numpad Control on the POS view. More official document can be found:<br/>
      
   [Use POS controls in extensions](https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/pos-extension/controls-pos-extension)
   
   <p> From the above offical doc, you can see only these POS Controls are supported by PosApi:</p>
     
   | Control | Interfaces | Description |
   |---------|------------|-------------|
   | Data list | IDataList, IPaginatedDataList | A responsive list control that is used throughout POS to show rows of information. |
   | Date picker  | IDatePicker | The date picker control that is used in POS. |
   | Menu | IMenu | The menu control that is used in POS to show contextual information. |
   | Number Pad | IAlphanumericNumPad, ICurrencyNumPad, INumericNumPad, ITransactionNumPad | <p>Number pads that are used throughout POS. Different types of number pads have different behaviors and input formatting:</p><ul><li>**Alphanumeric numpad** – This type of number pad accepts alphanumeric input.</li><li>**Currency numpad** – This type of number pad accepts monetary values.</li><li>**Numeric numpad** – This type of number pad accepts only numeric values.</li><li>**Transaction numpad** – This type of number pad accepts item identifiers or quantities. It's typically used in transaction scenarios.</li></ul> |
   | Time Picker | ITimePicker | The time picker control that is used in POS. |
   | Toggle | IToggle | The toggle switch control that is used in POS. |


2. **Commerce SDK's technical limitations**
   - The first challenge: <br/>
   <p>The big limitation of the datalist control is its columns only support *string*, the normal data list code is like</p>
   
   ```Javascript
    let dataListOptions: IDataListOptions<Entities.ExampleEntity> = {
        interactionMode: DataListInteractionMode.SingleSelect,
        data: this.viewModel.loadedData,
        columns: [
            {
                title: this.context.resources.getString("string_1001"), // Int data
                ratio: 40, collapseOrder: 1, minWidth: 100,
                computeValue: (data: Entities.ExampleEntity): string => data.IntData.toString()
            },
            {
                title: this.context.resources.getString("string_1002"), // String data
                ratio: 60, collapseOrder: 2, minWidth: 100,
                computeValue: (data: Entities.ExampleEntity): string => data.StringData
            }
        ]
    };
   ```
   That means that each column of the data list can only be a simple string, if user want to go to different view or open different dialog, from Commerce Product Group,  that is impossible.<br/>
   - The second challenge: <br/>
      Currently POS api only supports the simple dialog:<br/>
     ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/41b044f5-9b63-4e60-853e-5d37764b5f63)<br/>
     If you want to a little bit complex dialog, even including a Numeric Numpad on the dialog,  you may need extend the Templated Dialog to build the dialog like you build a normal POS View, that is also a challenge.<br/>
     
   - The 3rd challenge: <br/>
     There is another challenge is that in Retail SDK,  developers can user ApplicationContext to get the cached global variables like Store Information,  Store Tender Types, Store Default Customers, some parnters heavily depends on these variables,  but in Commerce SDK,  these unavailable. 
     

3. **This sample's goal is to make Bank-Drop Like Function totally depends on POS  Api instead of copying the OOB POS Controls since that is not supported:**
   - Finally the function looks like this
   - Open the custom "Bank Drop" View,  it looks like:
   - <img width="1443" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/294d4e36-538b-4423-8c54-eef9c8f52c5e">


5. **Install Scale Unit Extension Package**

6. **Install Store Commerce Extension Package**

7. **Technical Point:**
   
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
8. **All Source code can be found <br/>**
[Source code](https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/Notification-Sample)
