namespace Microsoft.Dynamics
{
    namespace Commerce.Runtime.Services.PricingEngine
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;

        /// <summary>
        /// Represents an AX unit of measure conversion.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Since the PricingEngine is used on both AX and CRT, we must take into account that unit of measure conversions are performed in different ways in each place (even though they yield the same result).
        /// In order to avoid duplicating the code to lookup which unit of measure conversion exists, the pricing engine AX wrapper uses the <c>UnitOfMeasureConverter</c> AX class,
        /// but this class uses a somewhat different modeling to handle unit of measure conversions.
        /// Instead of using the <see cref="UnitOfMeasureConversion.IsBackward"/>, it actually stores a <see cref="FactorDenominator"/> that can be used for "backwards" calculation.
        /// </para>
        /// <para>
        /// Forwards calculation means:
        /// - Item sales unit is pieces
        /// - A conversion between boxes and pieces exists
        /// - A sales order is created using boxes.
        /// </para>
        /// <para>
        /// Backwards calculation means:
        /// - Item sales unit is pieces
        /// - A conversion between pieces and boxes exists
        /// - A sales order is created using boxes.
        /// </para>
        /// </remarks>
        public class AxUnitOfMeasureConversion : UnitOfMeasureConversion
        {
            private const string FactorDenominatorColumn = "FACTORDENOMINATOR";

            /// <summary>
            /// Gets or sets the factor denominator.
            /// </summary>
            /// <value>
            /// The factor denominator.
            /// </value>
            [DataMember]
            [ReadOnly(FactorDenominatorColumn)]
            [Column(FactorDenominatorColumn)]
            public decimal FactorDenominator
            {
                get { return (this[FactorDenominatorColumn] == null) ? 0M : (decimal)this[FactorDenominatorColumn]; }
                set { this[FactorDenominatorColumn] = value; }
            }
        }
    }
}
