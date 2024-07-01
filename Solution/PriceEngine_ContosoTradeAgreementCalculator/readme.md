# This solution demonstrate how to build a customer trade agreement calculator
0. The official document is :  https://learn.microsoft.com/en-us/dynamics365/commerce/pricing-extensions#csu-and-store-commerce<br/>
1. F&O Side<br/>
  <img width="1082" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9500d27f-1242-4a3e-a195-11ae39afddc9">

The X++ extension is like as below<br/>

```cs
using CommerceRunTimeDataModel = Microsoft.Dynamics.Commerce.Runtime.DataModel;
using CrtPriceAndDiscountCalculationParameters = Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine.PriceAndDiscountCalculationParameters;
using Contoso.PricingEngine.TradeAgreementCalculator;
using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;
using System.Collections.Generic;
using System.Reflection;

[ExtensionOf(classStr(RetailOrderCalculator))]
final class RetailOrderCalculator_ApplicationSuiteExt_Extension
{
    protected void calculatePricesForOrder()
    {
        //Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine.PricingEngineExtensionRepositor::RegisterPriceTradeAgreementCalculator(new ContosoTradeAgreementCalculator());
        //Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine::RegisterPriceTradeAgreementCalculator(new ContosoTradeAgreementCalculator()); 
        
        PricingEngineExtensionRepository::RegisterPriceTradeAgreementCalculator(new ContosoTradeAgreementCalculator());
        next calculatePricesForOrder();
    }

}
```
2.  C# code to create the custom calculator:<br/>
<img width="1227" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b940c571-1b37-4cd3-8481-6032747dbad1">

And then register the customer calculator: <br/>
```cs
  public class PricingServiceSampleCustomization : IRequestHandlerAsync
  {
      /// <summary>
      /// Gets the collection of supported request types by this handler.
      /// </summary>
      public IEnumerable<Type> SupportedRequestTypes
      {
          get
          {
              return new[]
              {
                  typeof(CalculatePricesServiceRequest),
                  typeof(CalculateDiscountsServiceRequest),
                  typeof(GetIndependentPriceDiscountServiceRequest),
              };
          }
      }

      /// <summary>
      /// Implements customized solutions for pricing services.
      /// </summary>
      /// <param name="request">The request object.</param>
      /// <returns>The response object.</returns>
      public async Task<Response> Execute(Request request)
      {
          ThrowIf.Null(request, nameof(request));

          Response response;
          switch (request)
          {
              case CalculatePricesServiceRequest calculatePricesServiceRequest:
                  response = await this.CalculatePricesAsync(calculatePricesServiceRequest).ConfigureAwait(false);
                  break;
              case CalculateDiscountsServiceRequest calculateDiscountsServiceRequest:
                  response = await this.CalculateDiscountAsync(calculateDiscountsServiceRequest).ConfigureAwait(false);
                  break;
              case GetIndependentPriceDiscountServiceRequest getIndependentPriceDiscountServiceRequest:
                  response = await this.CalculateIndependentPriceAndDiscountAsync(getIndependentPriceDiscountServiceRequest).ConfigureAwait(false);
                  break;
              default:
                  throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
          }

          return response;
      }

      private async Task<GetPriceServiceResponse> CalculatePricesAsync(CalculatePricesServiceRequest request)
      {
          ThrowIf.Null(request, nameof(request));
          ThrowIf.Null(request.RequestContext, "request.RequestContext");
          ThrowIf.Null(request.Transaction, "request.Transaction");

          PricingEngineExtensionRegister.RegisterPricingEngineExtensions();

          var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

          return response;
      }

      private async Task<GetPriceServiceResponse> CalculateDiscountAsync(CalculateDiscountsServiceRequest request)
      {
          ThrowIf.Null(request, nameof(request));
          ThrowIf.Null(request.RequestContext, "request.RequestContext");
          ThrowIf.Null(request.Transaction, "request.Transaction");

          // Uncomment to register amount cap discount.
          // PE.IDiscountPackage package = new DiscountPackageAmountCap(new ChannelDataAccessorDiscountAmountCap(request.RequestContext));
          // PE.PricingEngineExtensionRepository.RegisterDiscountPackage(package);
          PricingEngineExtensionRegister.RegisterPricingEngineExtensions();

          var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

          return response;
      }

      private async Task<GetPriceServiceResponse> CalculateIndependentPriceAndDiscountAsync(
          GetIndependentPriceDiscountServiceRequest request)
      {
          ThrowIf.Null(request, nameof(request));
          ThrowIf.Null(request.RequestContext, "request.RequestContext");
          ThrowIf.Null(request.Transaction, "request.Transaction");

          PricingEngineExtensionRegister.RegisterPricingEngineExtensions();

          var response = await request.RequestContext.Runtime.ExecuteNextAsync<GetPriceServiceResponse>(this, request, request.RequestContext, skipRequestTriggers: false).ConfigureAwait(false);

          return response;
      }
  }
```
```cs
  using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;
  using Contoso.PricingEngine.TradeAgreementCalculator;

  /// <summary>
  /// Pricing engine initializer.
  /// </summary>
  /// <remarks>The sample code of multiple ways of initializing pricing engine. In production code, you need one only.</remarks>
  public static class PricingEngineExtensionRegister
  {
      /// <summary>
      /// Initializes pricing engine extensions.
      /// </summary>
      public static void RegisterPricingEngineExtensions()
      {
          PricingEngineExtensionRepository.RegisterPriceTradeAgreementCalculator(new ContosoTradeAgreementCalculator());
      }
  }
```
3. Then put the CommerceRuntime.dll will be put into this folder and also in
4. K:\AosService\WebRoot\bin
  <img width="804" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e7ae9b2d-f0d1-4826-bf2b-25d96d3228bf">

5. When we you debug you can see the custome calculator will be called: <br/>

6. Finally,  you will find it works:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f9b73705-cc50-4fbc-b6f9-cdec2f765694)

   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9328629c-0b41-4b85-a004-b877fa6b62a5)




   
   


