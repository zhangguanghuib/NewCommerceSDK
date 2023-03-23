/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using System;
        using System.Collections.Generic;

        /// <summary>
        /// Represents level3 data.
        /// </summary>
        internal class Level3Data
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Level3Data"/> class.
            /// </summary>
            internal Level3Data()
            {
            }

            /// <summary>
            /// Gets or sets the sequence number.
            /// </summary>
            /// <value>
            /// The sequence number.
            /// </value>
            internal string SequenceNumber { get; set; }

            /// <summary>
            /// Gets or sets the commodity code.
            /// </summary>
            /// <value>
            /// The commodity code.
            /// </value>
            internal string CommodityCode { get; set; }

            /// <summary>
            /// Gets or sets the product code.
            /// </summary>
            /// <value>
            /// The product code.
            /// </value>
            internal string ProductCode { get; set; }

            /// <summary>
            /// Gets or sets the name of the product.
            /// </summary>
            /// <value>
            /// The name of the product.
            /// </value>
            internal string ProductName { get; set; }

            /// <summary>
            /// Gets or sets the product sku.
            /// </summary>
            /// <value>
            /// The product sku.
            /// </value>
            internal string ProductSKU { get; set; }

            /// <summary>
            /// Gets or sets the descriptor.
            /// </summary>
            /// <value>
            /// The descriptor.
            /// </value>
            internal string Descriptor { get; set; }

            /// <summary>
            /// Gets or sets the unit of measure.
            /// </summary>
            /// <value>
            /// The unit of measure.
            /// </value>
            internal string UnitOfMeasure { get; set; }

            /// <summary>
            /// Gets or sets the unit price.
            /// </summary>
            /// <value>
            /// The unit price.
            /// </value>
            internal decimal? UnitPrice { get; set; }

            /// <summary>
            /// Gets or sets the discount.
            /// </summary>
            /// <value>
            /// The discount.
            /// </value>
            internal decimal? Discount { get; set; }

            /// <summary>
            /// Gets or sets the discount rate.
            /// </summary>
            /// <value>
            /// The discount rate.
            /// </value>
            internal decimal? DiscountRate { get; set; }

            /// <summary>
            /// Gets or sets the quantity.
            /// </summary>
            /// <value>
            /// The quantity.
            /// </value>
            internal decimal? Quantity { get; set; }

            /// <summary>
            /// Gets or sets the misc charge.
            /// </summary>
            /// <value>
            /// The misc charge.
            /// </value>
            internal decimal? MiscCharge { get; set; }

            /// <summary>
            /// Gets or sets the net total.
            /// </summary>
            /// <value>
            /// The net total.
            /// </value>
            internal decimal? NetTotal { get; set; }

            /// <summary>
            /// Gets or sets the tax amount.
            /// </summary>
            /// <value>
            /// The tax amount.
            /// </value>
            internal decimal? TaxAmount { get; set; }

            /// <summary>
            /// Gets or sets the tax rate.
            /// </summary>
            /// <value>
            /// The tax rate.
            /// </value>
            internal decimal? TaxRate { get; set; }

            /// <summary>
            /// Gets or sets the total amount.
            /// </summary>
            /// <value>
            /// The total amount.
            /// </value>
            internal decimal? TotalAmount { get; set; }

            /// <summary>
            /// Gets or sets the cost center.
            /// </summary>
            /// <value>
            /// The cost center.
            /// </value>
            internal string CostCenter { get; set; }

            /// <summary>
            /// Gets or sets the freight amount.
            /// </summary>
            /// <value>
            /// The freight amount.
            /// </value>
            internal decimal? FreightAmount { get; set; }

            /// <summary>
            /// Gets or sets the handling amount.
            /// </summary>
            /// <value>
            /// The handling amount.
            /// </value>
            internal decimal? HandlingAmount { get; set; }

            /// <summary>
            /// Gets or sets the carrier tracking number.
            /// </summary>
            /// <value>
            /// The carrier tracking number.
            /// </value>
            internal string CarrierTrackingNumber { get; set; }

            /// <summary>
            /// Gets or sets the merchant tax identifier.
            /// </summary>
            /// <value>
            /// The merchant tax identifier.
            /// </value>
            internal string MerchantTaxID { get; set; }

            /// <summary>
            /// Gets or sets the merchant catalog number.
            /// </summary>
            /// <value>
            /// The merchant catalog number.
            /// </value>
            internal string MerchantCatalogNumber { get; set; }

            /// <summary>
            /// Gets or sets the tax category applied.
            /// </summary>
            /// <value>
            /// The tax category applied.
            /// </value>
            internal string TaxCategoryApplied { get; set; }

            /// <summary>
            /// Gets or sets the pickup address.
            /// </summary>
            /// <value>
            /// The pickup address.
            /// </value>
            internal string PickupAddress { get; set; }

            /// <summary>
            /// Gets or sets the pickup city.
            /// </summary>
            /// <value>
            /// The pickup city.
            /// </value>
            internal string PickupCity { get; set; }

            /// <summary>
            /// Gets or sets the state of the pickup.
            /// </summary>
            /// <value>
            /// The state of the pickup.
            /// </value>
            internal string PickupState { get; set; }

            /// <summary>
            /// Gets or sets the pickup county.
            /// </summary>
            /// <value>
            /// The pickup county.
            /// </value>
            internal string PickupCounty { get; set; }

            /// <summary>
            /// Gets or sets the pickup zip.
            /// </summary>
            /// <value>
            /// The pickup zip.
            /// </value>
            internal string PickupZip { get; set; }

            /// <summary>
            /// Gets or sets the pickup country.
            /// </summary>
            /// <value>
            /// The pickup country.
            /// </value>
            internal string PickupCountry { get; set; }

            /// <summary>
            /// Gets or sets the pickup date time.
            /// </summary>
            /// <value>
            /// The pickup date time.
            /// </value>
            internal DateTime? PickupDateTime { get; set; }

            /// <summary>
            /// Gets or sets the pickup record number.
            /// </summary>
            /// <value>
            /// The pickup record number.
            /// </value>
            internal string PickupRecordNumber { get; set; }

            /// <summary>
            /// Gets or sets the carrier shipment number.
            /// </summary>
            /// <value>
            /// The carrier shipment number.
            /// </value>
            internal string CarrierShipmentNumber { get; set; }

            /// <summary>
            /// Gets or sets the UNSPSC code.
            /// </summary>
            /// <value>
            /// The UNSPSC code.
            /// </value>
            internal string UNSPSCCode { get; set; }

            /// <summary>
            /// Gets or sets the tax details.
            /// </summary>
            /// <value>
            /// The tax details.
            /// </value>
            internal IEnumerable<TaxDetail> TaxDetails { get; set; }

            /// <summary>
            /// Gets or sets the miscellaneous charges.
            /// </summary>
            /// <value>
            /// The miscellaneous charges.
            /// </value>
            internal IEnumerable<MiscellaneousCharge> MiscellaneousCharges { get; set; }
        }
    }
}
