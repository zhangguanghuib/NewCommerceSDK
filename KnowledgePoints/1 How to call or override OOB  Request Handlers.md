## How to generate custom receipt and print it?

1.HQ Configuration:
  * Receipt format
  * Receipt designer(no screenshot)
  * Receipt Profile
  <img width="839" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/96969aae-dab2-48e5-8ec4-bf25ddf8f64f">

2.  POS front-end code<br/>
```ts
public runPingTest(): Promise<void> {
    let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
        = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>("HOUSTON-HOUSTON-42-1711458887804", ProxyEntities.SearchLocation.Local);

    return this._context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): Promise<ProxyEntities.Receipt[]> => {
        let salesOrder: ProxyEntities.SalesOrder = res.data.result;

        return Promise.all([
            this._context.runtime.executeAsync(new GetHardwareProfileClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>): ProxyEntities.HardwareProfile => {
                    return response.data.result;
                }),
            this._context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                    return response.data.result;
                })])
            .then((results: any[]): Promise<ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>> => {
                let hardwareProfile: ProxyEntities.HardwareProfile = results[0];
                let deviceConfiguration: ProxyEntities.DeviceConfiguration = results[1];

                let criteria: ProxyEntities.ReceiptRetrievalCriteria = {
                    IsCopy: true,
                    IsRemoteTransaction: salesOrder.StoreId !== deviceConfiguration.StoreNumber,
                    IsPreview: false,
                    QueryBySalesId: true,
                    ReceiptTypeValue: ProxyEntities.ReceiptType.CustomReceipt6,
                    HardwareProfileId: hardwareProfile.ProfileId
                };

                let getReceiptsRequest: GetReceiptsClientRequest<GetReceiptsClientResponse> = new GetReceiptsClientRequest(salesOrder.SalesId ? salesOrder.SalesId : salesOrder.Id, criteria);
                return this._context.runtime.executeAsync(getReceiptsRequest);
            })
            .then((response: ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>): ProxyEntities.Receipt[] => {
                return response.data.result;
            });
    }).then((recreatedReceipts: ProxyEntities.Receipt[]): Promise<ClientEntities.ICancelableDataResult<PrinterPrintResponse>> => {
        let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
        return this._context.runtime.executeAsync(printRequest);
    }).then((): Promise<void> => {
        return Promise.resolve();
    }).catch((reason: any) => {
        console.error(reason);
    });
}
```
3. In the beginning, how the GetCustomReceiptsRequest will be called?<br/>
<img width="939" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7537e82d-2178-4ff9-a5d9-8e4aa57ad1c6">

4. Commerce runtime code:<br/>
  * Call GetCustomReceiptsRequest<br/>
  <img width="1094" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/7e91061b-5a07-44f3-8c3f-94e8b9ea6bf1">
  * GetReceiptServiceRequest
  <img width="971" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/947828fb-dbad-4ae1-99fc-3cda50f9dd17">
  * Call overriden request handler for GetReceiptServiceRequest to build the customer receipt:
    <img width="761" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/1686e094-7fd7-4965-b6e5-e32845d53aba"><br/>
  * Call OOB handler for GetReceiptServiceRequest to build the custom receipt:<br/>
  
  ```cs
    var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
    GetReceiptServiceResponse originalReceiptsResponse = await request.RequestContext.Runtime.ExecuteAsync<GetReceiptServiceResponse>(request, request.RequestContext, requestHandler, skipRequestTriggers: false).ConfigureAwait(false);
  ```
  <br/>
  <img width="1190" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8856114f-98f6-4993-abe0-5f45abf15546">
5. How the OOB Receipt Service build the custom receipt:<br/>
 *  In the GetFormattedReceipt, no condition will be met until go to default:
<img width="1135" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/ecdcafd0-532b-48bc-95e4-5f680a04be62">

6. Finally it goes to GetReceiptFromTransaction to build the receipt:<br/>
   <img width="1263" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/db77555b-7318-434a-9bb5-7e54ea4c4433">


