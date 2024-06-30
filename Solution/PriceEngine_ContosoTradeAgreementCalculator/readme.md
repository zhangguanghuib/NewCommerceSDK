# This solution demonstrate how to build a customer trade agreement calculator
1. F&O Side<br/>
  <img width="1082" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/9500d27f-1242-4a3e-a195-11ae39afddc9">

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
