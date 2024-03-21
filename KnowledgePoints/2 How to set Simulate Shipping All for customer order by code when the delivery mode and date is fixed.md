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
. Create customer order:
 <img width="121" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/cb809bfd-bee8-4551-95be-2b138069a359">
. Click Pay Cash/Card and Checkout:
 <img width="244" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/49ecc7d2-0d31-4815-83fb-8eaa3e864b38">
. They expected the Shipping Address, Delivery Mode and Delivery Date will be automatically set

2. The idea to fix this issue:
   # By code set the transaction level delivery specification and line level specification:
```cs
var requestHandler = new UserAuthenticationTransactionService();
var response = await request.RequestContext.Runtime.ExecuteAsync<RS.GetEmployeeIdentityByExternalIdentityRealtimeResponse>(request, request.RequestContext, requestHandler).ConfigureAwait(false);
```
So in this way we will provide some samples how to override the OOB  handler, or get the OOB  handler and explicitly use it when send request:

2.  Official document is https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/commerce-runtime-extensibility
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
