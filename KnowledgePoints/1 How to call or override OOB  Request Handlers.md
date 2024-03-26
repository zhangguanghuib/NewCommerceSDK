## How to generate custom receipt and print it?

1.HQ Configuration:
  * Receipt format
  * Receipt designer(no screenshot)
  * Receipt Profile
  <img width="839" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/96969aae-dab2-48e5-8ec4-bf25ddf8f64f">

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
