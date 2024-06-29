namespace Microsoft.Dynamics
{
    namespace Commerce.Runtime.Services.PricingEngine
    {
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// This class is responsible for choosing which unit of measure strategy is going to be used, depending on the environment that the pricing engine is running.
        /// If the pricing engine is running inside CRT, then <see cref="UnitOfMeasureConversionExtension"/> is invoked.
        /// If the pricing engine is running inside AX, then a trivial equation is solved in order to obtain the converted value.
        /// </summary>
        /// <remarks>
        /// Ideally the implementation of this class should've followed the same pattern that we use for <see cref="ICurrencyOperations"/>, providing one implementation per execution environment.
        /// But introducing a <c>IUnitOfMeasureOperations</c> would've meant changing lots of methods inside <see cref="PricingEngine"/> and <see cref="CommerceRuntimePriceAndDiscount"/>,
        /// and doing that without introducing a breaking change would be very challenging.
        /// </remarks>
        internal static class UnitOfMeasureOperations
        {
            /// <summary>
            /// Performs unit of measure conversions the CRT way.
            /// </summary>
            /// <param name="unitOfMeasureConversion">The unit of measure conversion.</param>
            /// <param name="fromQuantity">The quantity to be converted.</param>
            /// <returns>The converted quantity.</returns>
            /// <remarks>
            /// In CRT, we figure out the available conversions using a Request/Response pair and then invoke this method to do the actual calculation.
            /// </remarks>
            internal static decimal Convert(UnitOfMeasureConversion unitOfMeasureConversion, decimal fromQuantity)
            {
                return unitOfMeasureConversion.Convert(fromQuantity);
            }

            /// <summary>
            /// Performs unit of measure conversions the AX way.
            /// </summary>
            /// <param name="axUoMConversion">The unit of measure conversion.</param>
            /// <param name="fromQuantity">The quantity to be converted.</param>
            /// <returns>The converted quantity.</returns>
            /// <remarks>
            /// In AX, we use the <c>UnitOfMeasureConverter</c> X++ class to figure out the available conversions, then we use the <c>UnitOfMeasureConversionCache</c> returned by it to instantiate a <see cref="AxUnitOfMeasureConversion"/>
            /// that can be used by the pricing engine. See <c>RetailPricingUnitOfMeasureConversionHelper</c> X++ class for more details.
            /// The equation used below is inspired on the equation used at <c>performValueConversion</c> method of the <c>UnitOfMeasureConverter</c> X++ class.
            /// </remarks>
            internal static decimal Convert(AxUnitOfMeasureConversion axUoMConversion, decimal fromQuantity)
            {
                return (axUoMConversion.Factor * (axUoMConversion.Numerator * (fromQuantity + axUoMConversion.InnerOffset) / axUoMConversion.Denominator) / axUoMConversion.FactorDenominator) + axUoMConversion.OuterOffset;
            }

            /// <summary>
            /// Gets the factor for quantity.
            /// </summary>
            /// <param name="unitOfMeasureConversion">The unit of measure conversion.</param>
            /// <param name="quantity">The quantity.</param>
            /// <returns>The factor.</returns>
            internal static decimal GetFactorForQuantity(UnitOfMeasureConversion unitOfMeasureConversion, decimal quantity)
            {
                AxUnitOfMeasureConversion axUoMConversion = unitOfMeasureConversion as AxUnitOfMeasureConversion;

                if (axUoMConversion != null)
                {
                    if (quantity == 0)
                    {
                        // No conversion to do if the quantity is zero.
                        return 0;
                    }

                    return Convert(axUoMConversion, quantity) / quantity;
                }
                else
                {
                    return unitOfMeasureConversion.GetFactorForQuantity(quantity);
                }
            }
        }
    }
}
