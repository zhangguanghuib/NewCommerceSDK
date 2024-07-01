# This solution demonstrate how to build a customer trade agreement calculator
0. The official document is :<br/>  https://learn.microsoft.com/en-us/dynamics365/commerce/pricing-extensions#csu-and-store-commerce<br/>
1. F&O Side extension<br/>
  ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4cf6c1de-21cb-45a1-b31a-670294fefdf7)

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

And the X++ project referenced 3 C# assembly, those Target Framework should be .Net Standard 2.0<br/>

<img width="656" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/896947ea-0b58-47c5-b7e6-c0c7f6d216c3">

2.  C# code to create the custom calculator:<br/>
<img width="1227" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/b940c571-1b37-4cd3-8481-6032747dbad1">

And then register the customer calculator: <br/>
Please notice the below code use this line of C# code to register custom Calculator<br/>
```cs
          PricingEngineExtensionRegister.RegisterPricingEngineExtensions();
```
The complete code class is: <br/>
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
3. Then put the CommerceRuntime.dll will be put into this folder and also in<br/>
   K:\AosService\PackagesLocalDirectory\ApplicationSuiteExt\bin <br/>
   And this folder<Br/>
   <img width="749" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/c85c921c-a17c-45c4-8f07-dc695decc39b">
   
5. K:\AosService\WebRoot\bin
  <img width="804" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e7ae9b2d-f0d1-4826-bf2b-25d96d3228bf">

6. When we you debug you can see the custome calculator will be called: <br/>

<img width="836" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/55690e1e-2e75-4436-97a3-90424d8e50b7">


7. Finally,  you will find it works:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f9b73705-cc50-4fbc-b6f9-cdec2f765694)

   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9328629c-0b41-4b85-a004-b877fa6b62a5)




   
   


