## Customer Order(Call Center Order) In-Store Pickup Notification

1. <ins>Background:</ins><br/>
As you know D365 Commerce Solution Support Customer order (Call Center Order) In-Store Pick-up, some Store Manager want to get notification to show new order created and thatneed pickup from the current store<br/>
The final function is like this:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4148ff19-0324-4160-9cf6-2846852eb428)

2. Implementation details
- Add Custom Control on Screen Layout Designer
- <img width="1482" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/68f80dad-79cd-42a3-8ece-1149f8e7e587">
- <img width="1424" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8d92e1cd-2c70-48dc-b279-b0e728afe1a1">

3. <ins>Install Scale Unit Extension Package</ins><br/>
4. <ins>Install Store Commerce Extension Package</ins><br>
6. Technical point.
   - Periodically check if there are new customer order need pickup from the current store
   ```ts
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
   
