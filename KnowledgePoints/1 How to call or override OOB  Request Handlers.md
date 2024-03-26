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
3. Some code samples:<br/>
   .Way #1,  Override the OOB request Handler and then call the OOB request in the Process
   ```cs
   public class GetEmployeeIdentityByExternalIdentityRealtimeRequestHandler : SingleAsyncRequestHandler<GetEmployeeIdentityByExternalIdentityRealtimeRequest>
    {
        public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities
            = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();

        protected override async Task<Response> Process(GetEmployeeIdentityByExternalIdentityRealtimeRequest request)
        {
            ThrowIf.Null(request, "request");

            GetEmployeeIdentityByExternalIdentityRealtimeResponse val;

            try
            {
                if (request == null)
                {
                    var exception = new ArgumentNullException("request");
                   
                    throw exception;
                }
                else if (request is GetEmployeeIdentityByExternalIdentityRealtimeRequest)
                {
                    GetEmployeeIdentityByExternalIdentityRealtimeRequest employeeRequest = request as GetEmployeeIdentityByExternalIdentityRealtimeRequest;

                    if (foundIdentities.TryGetValue(employeeRequest.ExternalIdentityId, out val))
                    {    
                        return val;
                    }
                    else
                    {
                        var response = await this.ExecuteNextAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request).ConfigureAwait(false);
                        foundIdentities.GetOrAdd(employeeRequest.ExternalIdentityId, response);
                        return response;
                    }
                }
            }
            finally
            {
                //logger.StopOperation(operation);
            }
            // Do no checks.
            return NullResponse.Instance;
        }
    }
   ```
<br/>
   . Way#2, Implement SingleAsyncRequestHandler, get OOB  request Handler, and then when call request with the OOB  request handler:

  ```cs
  public class GetEmployeeIdentityByExternalIdentityRealtimeRequestHandlerV2 : SingleAsyncRequestHandler<GetEmployeeIdentityByExternalIdentityRealtimeRequest>
  {
      public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();
  
      protected async override Task<Response> Process(GetEmployeeIdentityByExternalIdentityRealtimeRequest request)
      {
          ThrowIf.Null(request, "request");
  
          // The extension should do nothing If fiscal registration is enabled and legacy extension were used to run registration process.
          if (!string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
          {
              return NotHandledResponse.Instance;
          }
  
          GetEmployeeIdentityByExternalIdentityRealtimeResponse val;
  
          if (foundIdentities.TryGetValue(request.ExternalIdentityId, out val))
          {
              return val;
          }
          else
          {
              // Execute original logic.
              var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
              var response = await request.RequestContext.Runtime.ExecuteAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);
  
              foundIdentities.GetOrAdd(request.ExternalIdentityId, response);
  
              return response;
          }
      }
  }
  ```
<br/>
. Way#3,Implement IRequestHandlerAsync,  then get OOB  request handler, and when send request using the base request handler:

 ```cs
 public class UserAuthService : IRequestHandlerAsync
 {
     public static ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse> foundIdentities 
         = new ConcurrentDictionary<string, GetEmployeeIdentityByExternalIdentityRealtimeResponse>();

     public IEnumerable<Type> SupportedRequestTypes
     {
         get
         {
             return new Type[]
             {
                 typeof(GetEmployeeIdentityByExternalIdentityRealtimeRequest)
             };
         }
     }

     /// <summary>
     /// Executes the request.
     /// </summary>
     /// <param name="request">The request.</param>
     /// <returns>The response.</returns>
     public async Task<Response> Execute(Request request)
     {
         GetEmployeeIdentityByExternalIdentityRealtimeResponse val;

         try
         {
             if (request == null)
             {
                 var exception = new ArgumentNullException("request");
                 throw exception;
             }
             else if (request is GetEmployeeIdentityByExternalIdentityRealtimeRequest)
             {
                 GetEmployeeIdentityByExternalIdentityRealtimeRequest employeeRequest = request as GetEmployeeIdentityByExternalIdentityRealtimeRequest;

                 if (foundIdentities.TryGetValue(employeeRequest.ExternalIdentityId, out val))
                 {
                     //return foundIdentities[employeeRequest.ExternalIdentityId];
                     return val;
                 }
                 else
                 {
                     var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
                     var response = await request.RequestContext.Runtime.ExecuteAsync<GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);

                     foundIdentities.GetOrAdd(employeeRequest.ExternalIdentityId, response);
                     return response;
                 }
             }
         }
         finally
         {
             Console.WriteLine("Finally");
         }

         // Do no checks.
         return NullResponse.Instance;
     }
 }
 ```

4.  The complete project to test that is:  https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/POSExtensions/RQL-UserAuthService/src/ScaleUnitSample/CommerceRuntime/RequestHandlers
