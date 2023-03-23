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
        /// Repesents level2 data.
        /// </summary>
        internal class Level2Data
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Level2Data"/> class.
            /// </summary>
            internal Level2Data()
            {
            }

            /// <summary>
            /// Gets or sets the order date time.
            /// </summary>
            /// <value>
            /// The order date time.
            /// </value>
            internal DateTime? OrderDateTime { get; set; }

            /// <summary>
            /// Gets or sets the order number.
            /// </summary>
            /// <value>
            /// The order number.
            /// </value>
            internal string OrderNumber { get; set; }

            /// <summary>
            /// Gets or sets the invoice date time.
            /// </summary>
            /// <value>
            /// The invoice date time.
            /// </value>
            internal DateTime? InvoiceDateTime { get; set; }

            /// <summary>
            /// Gets or sets the invoice number.
            /// </summary>
            /// <value>
            /// The invoice number.
            /// </value>
            internal string InvoiceNumber { get; set; }

            /// <summary>
            /// Gets or sets the order description.
            /// </summary>
            /// <value>
            /// The order description.
            /// </value>
            internal string OrderDescription { get; set; }

            /// <summary>
            /// Gets or sets the summary commodity code.
            /// </summary>
            /// <value>
            /// The summary commodity code.
            /// </value>
            internal string SummaryCommodityCode { get; set; }

            /// <summary>
            /// Gets or sets the merchant contact.
            /// </summary>
            /// <value>
            /// The merchant contact.
            /// </value>
            internal string MerchantContact { get; set; }

            /// <summary>
            /// Gets or sets the merchant tax identifier.
            /// </summary>
            /// <value>
            /// The merchant tax identifier.
            /// </value>
            internal string MerchantTaxId { get; set; }

            /// <summary>
            /// Gets or sets the type of the merchant.
            /// </summary>
            /// <value>
            /// The type of the merchant.
            /// </value>
            internal string MerchantType { get; set; }

            /// <summary>
            /// Gets or sets the purchaser identifier.
            /// </summary>
            /// <value>
            /// The purchaser identifier.
            /// </value>
            internal string PurchaserId { get; set; }

            /// <summary>
            /// Gets or sets the purchaser tax identifier.
            /// </summary>
            /// <value>
            /// The purchaser tax identifier.
            /// </value>
            internal string PurchaserTaxId { get; set; }

            /// <summary>
            /// Gets or sets the ship to city.
            /// </summary>
            /// <value>
            /// The ship to city.
            /// </value>
            internal string ShipToCity { get; set; }

            /// <summary>
            /// Gets or sets the ship to county.
            /// </summary>
            /// <value>
            /// The ship to county.
            /// </value>
            internal string ShipToCounty { get; set; }

            /// <summary>
            /// Gets or sets the ship to state province code.
            /// </summary>
            /// <value>
            /// The ship to state province code.
            /// </value>
            internal string ShipToState_ProvinceCode { get; set; }

            /// <summary>
            /// Gets or sets the ship to postal code.
            /// </summary>
            /// <value>
            /// The ship to postal code.
            /// </value>
            internal string ShipToPostalCode { get; set; }

            /// <summary>
            /// Gets or sets the ship to country code.
            /// </summary>
            /// <value>
            /// The ship to country code.
            /// </value>
            internal string ShipToCountryCode { get; set; }

            /// <summary>
            /// Gets or sets the ship from city.
            /// </summary>
            /// <value>
            /// The ship from city.
            /// </value>
            internal string ShipFromCity { get; set; }

            /// <summary>
            /// Gets or sets the ship from county.
            /// </summary>
            /// <value>
            /// The ship from county.
            /// </value>
            internal string ShipFromCounty { get; set; }

            /// <summary>
            /// Gets or sets the ship from state province code.
            /// </summary>
            /// <value>
            /// The ship from state province code.
            /// </value>
            internal string ShipFromState_ProvinceCode { get; set; }

            /// <summary>
            /// Gets or sets the ship from postal code.
            /// </summary>
            /// <value>
            /// The ship from postal code.
            /// </value>
            internal string ShipFromPostalCode { get; set; }

            /// <summary>
            /// Gets or sets the ship from country code.
            /// </summary>
            /// <value>
            /// The ship from country code.
            /// </value>
            internal string ShipFromCountryCode { get; set; }

            /// <summary>
            /// Gets or sets the discount amount.
            /// </summary>
            /// <value>
            /// The discount amount.
            /// </value>
            internal decimal? DiscountAmount { get; set; }

            /// <summary>
            /// Gets or sets the misc charge.
            /// </summary>
            /// <value>
            /// The misc charge.
            /// </value>
            internal decimal? MiscCharge { get; set; }

            /// <summary>
            /// Gets or sets the duty amount.
            /// </summary>
            /// <value>
            /// The duty amount.
            /// </value>
            internal decimal? DutyAmount { get; set; }

            /// <summary>
            /// Gets or sets the freight amount.
            /// </summary>
            /// <value>
            /// The freight amount.
            /// </value>
            internal decimal? FreightAmount { get; set; }

            /// <summary>
            /// Gets or sets the is taxable.
            /// </summary>
            /// <value>
            /// The is taxable.
            /// </value>
            internal bool? IsTaxable { get; set; }

            /// <summary>
            /// Gets or sets the total tax amount.
            /// </summary>
            /// <value>
            /// The total tax amount.
            /// </value>
            internal decimal? TotalTaxAmount { get; set; }

            /// <summary>
            /// Gets or sets the total tax rate.
            /// </summary>
            /// <value>
            /// The total tax rate.
            /// </value>
            internal decimal? TotalTaxRate { get; set; }

            /// <summary>
            /// Gets or sets the name of the merchant.
            /// </summary>
            /// <value>
            /// The name of the merchant.
            /// </value>
            internal string MerchantName { get; set; }

            /// <summary>
            /// Gets or sets the merchant street.
            /// </summary>
            /// <value>
            /// The merchant street.
            /// </value>
            internal string MerchantStreet { get; set; }

            /// <summary>
            /// Gets or sets the merchant city.
            /// </summary>
            /// <value>
            /// The merchant city.
            /// </value>
            internal string MerchantCity { get; set; }

            /// <summary>
            /// Gets or sets the state of the merchant.
            /// </summary>
            /// <value>
            /// The state of the merchant.
            /// </value>
            internal string MerchantState { get; set; }

            /// <summary>
            /// Gets or sets the merchant county.
            /// </summary>
            /// <value>
            /// The merchant county.
            /// </value>
            internal string MerchantCounty { get; set; }

            /// <summary>
            /// Gets or sets the merchant country code.
            /// </summary>
            /// <value>
            /// The merchant country code.
            /// </value>
            internal string MerchantCountryCode { get; set; }

            /// <summary>
            /// Gets or sets the merchant zip.
            /// </summary>
            /// <value>
            /// The merchant zip.
            /// </value>
            internal string MerchantZip { get; set; }

            /// <summary>
            /// Gets or sets the tax rate.
            /// </summary>
            /// <value>
            /// The tax rate.
            /// </value>
            internal decimal? TaxRate { get; set; }

            /// <summary>
            /// Gets or sets the tax amount.
            /// </summary>
            /// <value>
            /// The tax amount.
            /// </value>
            internal decimal? TaxAmount { get; set; }

            /// <summary>
            /// Gets or sets the tax description.
            /// </summary>
            /// <value>
            /// The tax description.
            /// </value>
            internal string TaxDescription { get; set; }

            /// <summary>
            /// Gets or sets the tax type identifier.
            /// </summary>
            /// <value>
            /// The tax type identifier.
            /// </value>
            internal string TaxTypeIdentifier { get; set; }

            /// <summary>
            /// Gets or sets the name of the requester.
            /// </summary>
            /// <value>
            /// The name of the requester.
            /// </value>
            internal string RequesterName { get; set; }

            /// <summary>
            /// Gets or sets the total amount.
            /// </summary>
            /// <value>
            /// The total amount.
            /// </value>
            internal decimal? TotalAmount { get; set; }

            /// <summary>
            /// Gets or sets the type of the purchase card.
            /// </summary>
            /// <value>
            /// The type of the purchase card.
            /// </value>
            internal string PurchaseCardType { get; set; }

            /// <summary>
            /// Gets or sets the amex legacy description1.
            /// </summary>
            /// <value>
            /// The amex legacy description1.
            /// </value>
            internal string AmexLegacyDescription1 { get; set; }

            /// <summary>
            /// Gets or sets the amex legacy description2.
            /// </summary>
            /// <value>
            /// The amex legacy description2.
            /// </value>
            internal string AmexLegacyDescription2 { get; set; }

            /// <summary>
            /// Gets or sets the amex legacy description3.
            /// </summary>
            /// <value>
            /// The amex legacy description3.
            /// </value>
            internal string AmexLegacyDescription3 { get; set; }

            /// <summary>
            /// Gets or sets the amex legacy description4.
            /// </summary>
            /// <value>
            /// The amex legacy description4.
            /// </value>
            internal string AmexLegacyDescription4 { get; set; }

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
