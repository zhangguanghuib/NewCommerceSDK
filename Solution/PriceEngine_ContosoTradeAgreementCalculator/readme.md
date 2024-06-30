# This solution demonstrate how to build a customer trade agreement calculator
1. F&O Side<br/>
  <img width="1082" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9500d27f-1242-4a3e-a195-11ae39afddc9">

The X++ extension is like as below<br/>

```cs
using CommerceRunTimeDataModel = Microsoft.Dynamics.Commerce.Runtime.DataModel;
using CrtPriceAndDiscountCalculationParameters = Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine.PriceAndDiscountCalculationParameters;
using PricingEngine.ContosoTradeAgreementCalculator;
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

3. Finally only the CommerceRuntime.dll will be put into this folder:
  <img width="804" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/e7ae9b2d-f0d1-4826-bf2b-25d96d3228bf">

4. Finally, we you debug you can see the custome calculator will be called: <br/>

5. Finally,  you will find it works:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f9b73705-cc50-4fbc-b6f9-cdec2f765694)

   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9328629c-0b41-4b85-a004-b877fa6b62a5)




   
   


