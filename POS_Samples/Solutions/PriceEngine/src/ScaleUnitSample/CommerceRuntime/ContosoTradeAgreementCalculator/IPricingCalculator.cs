namespace Microsoft.Dynamics
{
    namespace Commerce.Runtime.Services.PricingEngine
    {
        using System.Collections.Generic;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// This interface defines a method which will calculate prices for all given
        ///  sales lines and return the price lines keyed by sales line Id.
        /// </summary>
        internal interface IPricingCalculator
        {
            /// <summary>
            /// This method will calculate the prices for given sales transaction,
            ///  and return the price lines for each item line, keyed by the item line Id.
            /// </summary>
            /// <param name="transaction">The transaction which need prices.</param>
            /// <param name="priceContext">The configuration of the overall pricing context for the calculation.</param>
            /// <param name="pricingDataManager">Instance of pricing data manager to access pricing data.</param>
            /// <returns>Sets of possible price lines keyed by item line Id.</returns>
            IDictionary<string, IEnumerable<PriceLine>> CalculatePriceLines(
                SalesTransaction transaction,
                PriceContext priceContext,
                IPricingDataAccessor pricingDataManager);
        }
    }
}
