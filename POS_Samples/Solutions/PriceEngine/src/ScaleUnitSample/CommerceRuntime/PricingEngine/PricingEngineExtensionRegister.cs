namespace Contoso.CommerceRuntime.PricingEngine
{
    using global::PricingEngine.ContosoTradeAgreementCalculator;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;

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
}
