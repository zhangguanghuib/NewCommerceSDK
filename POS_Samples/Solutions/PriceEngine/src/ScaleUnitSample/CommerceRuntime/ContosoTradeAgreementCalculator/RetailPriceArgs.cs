/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso.CommerceRuntime.PricingEngine
{
    using System.Collections.ObjectModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Arguments for a Retail price lookup operation.
    /// </summary>
    internal struct RetailPriceArgs
    {
        /// <summary>
        /// The optional customer account number to consider.
        /// </summary>
        internal string CustomerId;

        /// <summary>
        /// The price group Ids to search by.
        /// </summary>
        internal ReadOnlyCollection<string> PriceGroups;

        /// <summary>
        /// The currency code to filter by.
        /// </summary>
        internal string CurrencyCode;

        /// <summary>
        /// The quantity of the item or total cost to consider.
        /// </summary>
        internal decimal Quantity;

        /// <summary>
        /// The item Id to find a price for.
        /// </summary>
        internal string ItemId;

        /// <summary>
        /// The barcode for this line. Deprecated.
        /// </summary>
        internal string Barcode;

        /// <summary>
        /// The default sales unit of measure.
        /// </summary>
        internal string DefaultSalesUnitOfMeasure;

        /// <summary>
        /// The sales unit of measure of the item. Used to search by unit.
        /// </summary>
        internal string SalesUOM;

        /// <summary>
        /// The quantity of the item or total cost to consider in default quantity.
        /// </summary>
        internal decimal QuantityOnDefaultSOM;

        /// <summary>
        /// The class which manages unit conversion for this item.
        /// </summary>
        internal UnitOfMeasureConversion UnitOfMeasureConversion;

        /// <summary>
        /// Optional parameter which specifies the product variant dimensions to consider for price search.
        /// </summary>
        internal ProductVariant Dimensions;

        /// <summary>
        /// Convert to <see cref="PriceAgreementArgs"/> using the Sale UOM.
        /// </summary>
        /// <returns>Sales price agreement arguments.</returns>
        internal PriceAgreementArgs AgreementArgsForSale()
        {
            return new PriceAgreementArgs()
            {
                CurrencyCode = this.CurrencyCode,
                CustomerId = this.CustomerId,
                Dimensions = this.Dimensions,
                ItemId = this.ItemId,
                PriceGroups = this.PriceGroups,
                Quantity = this.Quantity,
                UnitOfMeasure = this.SalesUOM,
            };
        }

        /// <summary>
        /// Convert to <see cref="PriceAgreementArgs"/> using the Inventory UOM.
        /// </summary>
        /// <returns>The inventory price agreement arguments.</returns>
        internal PriceAgreementArgs AgreementArgsForDefaultSales()
        {
            return new PriceAgreementArgs()
            {
                CurrencyCode = this.CurrencyCode,
                CustomerId = this.CustomerId,
                Dimensions = this.Dimensions,
                ItemId = this.ItemId,
                PriceGroups = this.PriceGroups,
                Quantity = this.QuantityOnDefaultSOM,
                UnitOfMeasure = this.DefaultSalesUnitOfMeasure,
            };
        }
    }
}
