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
        using System.Linq;
        using System.Text;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Utility methods for payment processing.
        /// </summary>
        internal static class PaymentUtilities
        {
            private const string OperationCompleted = "completed";

            /// <summary>
            /// Gets the property string value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="errors">The errors.</param>
            /// <param name="errorCode">The error code.</param>
            /// <returns>Property value.</returns>
            internal static string GetPropertyStringValue(Hashtable properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                string value;
                if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
                {
                    value = null;
                    if (errors != null)
                    {
                        errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                    }
                }

                return value;
            }

            /// <summary>
            /// Gets the property string value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns>Property value.</returns>
            internal static string GetPropertyStringValue(Hashtable properties, string propertyNamespace, string propertyName)
            {
                return GetPropertyStringValue(properties, propertyNamespace, propertyName, errors: null, ErrorCode.InvalidRequest);
            }

            /// <summary>
            /// Gets the property decimal value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="errors">The errors.</param>
            /// <param name="errorCode">The error code.</param>
            /// <returns>Nullable <see cref="decimal"/>.</returns>
            internal static decimal? GetPropertyDecimalValue(Hashtable properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                decimal value;
                decimal? result = null;
                if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
                {
                    if (errors != null)
                    {
                        errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                    }
                }
                else
                {
                    result = value;
                }

                return result;
            }

            /// <summary>
            /// Gets the property decimal value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns>Nullable <see cref="decimal"/>.</returns>
            internal static decimal? GetPropertyDecimalValue(Hashtable properties, string propertyNamespace, string propertyName)
            {
                return GetPropertyDecimalValue(properties, propertyNamespace, propertyName, errors: null, ErrorCode.InvalidRequest);
            }

            /// <summary>
            /// Gets the property boolean value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="errors">The errors.</param>
            /// <param name="errorCode">The error code.</param>
            /// <returns>Nullable <see cref="bool"/>.</returns>
            internal static bool? GetPropertyBooleanValue(Hashtable properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                bool value;
                bool? result = null;
                if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
                {
                    if (errors != null)
                    {
                        errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                    }
                }
                else
                {
                    result = value;
                }

                return result;
            }

            /// <summary>
            /// Gets the property boolean value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns>Nullable <see cref="bool"/>.</returns>
            internal static bool? GetPropertyBooleanValue(Hashtable properties, string propertyNamespace, string propertyName)
            {
                return GetPropertyBooleanValue(properties, propertyNamespace, propertyName, errors: null, ErrorCode.InvalidRequest);
            }

            /// <summary>
            /// Gets the property date time value.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="errors">The errors.</param>
            /// <param name="errorCode">The error code.</param>
            /// <returns>Nullable <see cref="DateTime"/>.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static DateTime? GetPropertyDateTimeValue(Hashtable properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                DateTime value;
                DateTime? result = null;
                if (!PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out value))
                {
                    if (errors != null)
                    {
                        errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                    }
                }
                else
                {
                    result = value;
                }

                return result;
            }

            /// <summary>
            /// Gets the value of date time property.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <returns>Nullable <see cref="DateTime"/>.</returns>
            internal static DateTime? GetPropertyDateTimeValue(Hashtable properties, string propertyNamespace, string propertyName)
            {
                return GetPropertyDateTimeValue(properties, propertyNamespace, propertyName, errors: null, ErrorCode.InvalidRequest);
            }

            /// <summary>
            /// Gets the level2 data.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <returns>Collection of <see cref="Level2Data"/>.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static Level2Data GetLevel2Data(Hashtable properties)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                Level2Data level2Data = null;
                PaymentProperty[] level2DataPropertyArray;
                if (PaymentProperty.GetPropertyValue(properties, GenericNamespace.PurchaseLevelData, PurchaseLevelDataProperties.L2Data, out level2DataPropertyArray))
                {
                    Hashtable level2DataProperties = PaymentProperty.ConvertToHashtable(level2DataPropertyArray);

                    level2Data = new Level2Data();
                    level2Data.OrderDateTime = PaymentUtilities.GetPropertyDateTimeValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.OrderDateTime);
                    level2Data.OrderNumber = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.OrderNumber);
                    level2Data.InvoiceDateTime = PaymentUtilities.GetPropertyDateTimeValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.InvoiceDateTime);
                    level2Data.InvoiceNumber = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.InvoiceNumber);
                    level2Data.OrderDescription = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.OrderDescription);
                    level2Data.SummaryCommodityCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.SummaryCommodityCode);
                    level2Data.MerchantContact = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantContact);
                    level2Data.MerchantTaxId = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantTaxId);
                    level2Data.MerchantType = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantType);
                    level2Data.PurchaserId = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.PurchaserId);
                    level2Data.PurchaserTaxId = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.PurchaserTaxId);
                    level2Data.ShipToCity = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipToCity);
                    level2Data.ShipToCounty = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipToCounty);
                    level2Data.ShipToState_ProvinceCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipToState_ProvinceCode);
                    level2Data.ShipToPostalCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipToPostalCode);
                    level2Data.ShipToCountryCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipToCountryCode);
                    level2Data.ShipFromCity = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipFromCity);
                    level2Data.ShipFromCounty = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipFromCounty);
                    level2Data.ShipFromState_ProvinceCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipFromState_ProvinceCode);
                    level2Data.ShipFromPostalCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipFromPostalCode);
                    level2Data.ShipFromCountryCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.ShipFromCountryCode);
                    level2Data.DiscountAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.DiscountAmount);
                    level2Data.MiscCharge = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MiscCharge);
                    level2Data.DutyAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.DutyAmount);
                    level2Data.FreightAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.FreightAmount);
                    level2Data.IsTaxable = PaymentUtilities.GetPropertyBooleanValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.IsTaxable);
                    level2Data.TotalTaxAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TotalTaxAmount);
                    level2Data.TotalTaxRate = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TotalTaxRate);
                    level2Data.MerchantName = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantName);
                    level2Data.MerchantStreet = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantStreet);
                    level2Data.MerchantCity = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantCity);
                    level2Data.MerchantState = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantState);
                    level2Data.MerchantCounty = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantCounty);
                    level2Data.MerchantCountryCode = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantCountryCode);
                    level2Data.MerchantZip = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MerchantZip);
                    level2Data.TaxRate = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TaxRate);
                    level2Data.TaxAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TaxAmount);
                    level2Data.TaxDescription = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TaxDescription);
                    level2Data.TaxTypeIdentifier = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TaxTypeIdentifier);
                    level2Data.RequesterName = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.RequesterName);
                    level2Data.TotalAmount = PaymentUtilities.GetPropertyDecimalValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TotalAmount);
                    level2Data.PurchaseCardType = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.PurchaseCardType);
                    level2Data.AmexLegacyDescription1 = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.AmexLegacyDescription1);
                    level2Data.AmexLegacyDescription2 = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.AmexLegacyDescription2);
                    level2Data.AmexLegacyDescription3 = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.AmexLegacyDescription3);
                    level2Data.AmexLegacyDescription4 = PaymentUtilities.GetPropertyStringValue(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.AmexLegacyDescription4);

                    level2Data.TaxDetails = PaymentUtilities.GetTaxDetails(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.TaxDetails);
                    level2Data.MiscellaneousCharges = PaymentUtilities.GetMiscellaneousCharges(level2DataProperties, GenericNamespace.L2Data, L2DataProperties.MiscellaneousCharges);
                }

                return level2Data;
            }

            /// <summary>
            /// Gets the level3 data.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <returns>Collection of <see cref="Level3Data"/>.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static IEnumerable<Level3Data> GetLevel3Data(Hashtable properties)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                IEnumerable<Level3Data> level3Data = null;
                PaymentProperty[] level3DataPropertyArray;
                if (PaymentProperty.GetPropertyValue(properties, GenericNamespace.PurchaseLevelData, PurchaseLevelDataProperties.L3Data, out level3DataPropertyArray))
                {
                    if (level3DataPropertyArray.Length > 0)
                    {
                        level3Data = new List<Level3Data>();
                    }

                    foreach (var level3DataProperty in level3DataPropertyArray)
                    {
                        var level3DataItems = new Level3Data();
                        Hashtable level3DataItemsProperties = PaymentProperty.ConvertToHashtable(level3DataProperty.PropertyList);

                        level3DataItems.SequenceNumber = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.SequenceNumber);
                        level3DataItems.CommodityCode = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.CommodityCode);
                        level3DataItems.ProductCode = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.ProductCode);
                        level3DataItems.ProductName = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.ProductName);
                        level3DataItems.ProductSKU = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.ProductSKU);
                        level3DataItems.Descriptor = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.Descriptor);
                        level3DataItems.UnitOfMeasure = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.UnitOfMeasure);
                        level3DataItems.UnitPrice = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.UnitPrice);
                        level3DataItems.Discount = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.Discount);
                        level3DataItems.DiscountRate = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.DiscountRate);
                        level3DataItems.Quantity = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.Quantity);
                        level3DataItems.MiscCharge = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.MiscCharge);
                        level3DataItems.NetTotal = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.NetTotal);
                        level3DataItems.TaxAmount = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.TaxAmount);
                        level3DataItems.TaxRate = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.TaxRate);
                        level3DataItems.TotalAmount = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.TotalAmount);
                        level3DataItems.CostCenter = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.CostCenter);
                        level3DataItems.FreightAmount = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.FreightAmount);
                        level3DataItems.HandlingAmount = PaymentUtilities.GetPropertyDecimalValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.HandlingAmount);
                        level3DataItems.CarrierTrackingNumber = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.CarrierTrackingNumber);
                        level3DataItems.MerchantTaxID = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.MerchantTaxID);
                        level3DataItems.MerchantCatalogNumber = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.MerchantCatalogNumber);
                        level3DataItems.TaxCategoryApplied = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.TaxCategoryApplied);
                        level3DataItems.PickupAddress = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupAddress);
                        level3DataItems.PickupCity = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupCity);
                        level3DataItems.PickupState = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupState);
                        level3DataItems.PickupCounty = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupCounty);
                        level3DataItems.PickupZip = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupZip);
                        level3DataItems.PickupCountry = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupCountry);
                        level3DataItems.PickupDateTime = PaymentUtilities.GetPropertyDateTimeValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupDateTime);
                        level3DataItems.PickupRecordNumber = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.PickupRecordNumber);
                        level3DataItems.CarrierShipmentNumber = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.CarrierShipmentNumber);
                        level3DataItems.UNSPSCCode = PaymentUtilities.GetPropertyStringValue(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.UNSPSCCode);

                        level3DataItems.TaxDetails = PaymentUtilities.GetTaxDetails(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.TaxDetails);
                        level3DataItems.MiscellaneousCharges = PaymentUtilities.GetMiscellaneousCharges(level3DataItemsProperties, GenericNamespace.L3Data, L3DataProperties.MiscellaneousCharges);

                        (level3Data as List<Level3Data>).Add(level3DataItems);
                    }
                }

                return level3Data;
            }

            /// <summary>
            /// Adds the property if present.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="value">The value.</param>
            /// <returns>Added property or <c>null</c> if not present.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, string value)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                if (value != null)
                {
                    PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value);
                    properties.Add(property);
                    return property;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Adds the property if present.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="value">The value.</param>
            /// <returns>Added property or <c>null</c> if not present.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, decimal? value)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                if (value.HasValue)
                {
                    PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value.Value);
                    properties.Add(property);
                    return property;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Adds the property if present.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="value">The value.</param>
            /// <returns>Added property or <c>null</c> if not present.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, DateTime? value)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                if (value.HasValue)
                {
                    PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value.Value);
                    properties.Add(property);
                    return property;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Adds the property if present.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="propertyNamespace">The property namespace.</param>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="value">The value.</param>
            /// <returns>Added property or <c>null</c> if not present.</returns>
            /// <exception cref="ArgumentNullException">properties.</exception>
            internal static PaymentProperty AddPropertyIfPresent(IList<PaymentProperty> properties, string propertyNamespace, string propertyName, bool? value)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                if (value.HasValue)
                {
                    PaymentProperty property = new PaymentProperty(propertyNamespace, propertyName, value.Value.ToString());
                    properties.Add(property);
                    return property;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Creates the and log response for return.
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            /// <param name="connectorName">Name of the connector.</param>
            /// <param name="platform">The platform.</param>
            /// <param name="locale">The locale.</param>
            /// <param name="errors">The errors.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response CreateAndLogResponseForReturn(string methodName, string connectorName, string platform, string locale, IList<PaymentError> errors)
            {
                return CreateAndLogResponseForReturn(methodName, connectorName, platform, locale, properties: null, errors);
            }

            /// <summary>
            /// Creates the and log response for return.
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            /// <param name="connectorName">Name of the connector.</param>
            /// <param name="platform">The platform.</param>
            /// <param name="locale">The locale.</param>
            /// <param name="properties">The properties.</param>
            /// <param name="errors">The errors.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response CreateAndLogResponseForReturn(string methodName, string connectorName, string platform, string locale, IList<PaymentProperty> properties, IList<PaymentError> errors)
            {
                var response = new Response();
                response.Locale = locale;

                if (properties != null && properties.Count > 0)
                {
                    response.Properties = new PaymentProperty[properties.Count];
                    properties.CopyTo(response.Properties, 0);
                }

                if (errors != null && errors.Count > 0)
                {
                    response.Errors = new PaymentError[errors.Count];
                    errors.CopyTo(response.Errors, 0);
                }

                LogBeforeReturn(methodName, connectorName, platform, response.Errors);
                return response;
            }

            /// <summary>
            /// Logs the operation result.
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            /// <param name="connectorName">Name of the connector.</param>
            /// <param name="platform">The platform.</param>
            /// <param name="request">The request.</param>
            /// <param name="response">The response.</param>
            internal static void LogOperationResult(string methodName, string connectorName, string platform, Request request, Response response)
            {
                /*
                 IMPORTANT!!!
                 THIS IS SAMPLE CODE ONLY!
                 THE CODE MIGHT LOG TOO MUCH INFORMATION WHICH IS INAPPROPRIATE FOR PRODUCT ENVIRONMENT.
                */
                if (request != null && request.Properties != null && response != null && response.Properties != null)
                {
                    var requestProperties = PaymentProperty.ConvertToHashtable(request.Properties);
                    var responseProperties = PaymentProperty.ConvertToHashtable(response.Properties);

                    StringBuilder result = new StringBuilder();
                    string requestCardToken = PaymentUtilities.GetPropertyStringValue(requestProperties, GenericNamespace.PaymentCard, PaymentCardProperties.CardToken);
                    string responseCardToken = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.PaymentCard, PaymentCardProperties.CardToken);
                    if (string.IsNullOrWhiteSpace(requestCardToken) && !string.IsNullOrWhiteSpace(responseCardToken))
                    {
                        string last4Digit = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.PaymentCard, PaymentCardProperties.Last4Digits);
                        result.Append(string.Format("Card {0}, Tokenization Success. ", last4Digit));
                    }

                    string authResult = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AuthorizationResult);
                    if (!string.IsNullOrWhiteSpace(authResult))
                    {
                        string last4Digit = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Last4Digits);
                        decimal? approvedAmount = PaymentUtilities.GetPropertyDecimalValue(responseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ApprovedAmount);
                        string currency = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CurrencyCode);
                        result.Append(string.Format("Card {0}, Authorization {1}, Approved amount {2} {3}. ", last4Digit, authResult, approvedAmount, currency));
                    }

                    string captureResult = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CaptureResult);
                    if (!string.IsNullOrWhiteSpace(captureResult))
                    {
                        string last4Digit = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.Last4Digits);
                        decimal? requestedAmount = PaymentUtilities.GetPropertyDecimalValue(requestProperties, GenericNamespace.TransactionData, TransactionDataProperties.Amount);
                        string currency = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CurrencyCode);
                        result.Append(string.Format("Card {0}, Capture {1}, Requested amount {2} {3}. ", last4Digit, captureResult, requestedAmount, currency));
                    }

                    string voidResult = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.VoidResult);
                    if (!string.IsNullOrWhiteSpace(voidResult))
                    {
                        string last4Digit = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.Last4Digits);
                        decimal? requestedAmount = PaymentUtilities.GetPropertyDecimalValue(requestProperties, GenericNamespace.TransactionData, TransactionDataProperties.Amount);
                        string currency = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CurrencyCode);
                        result.Append(string.Format("Card {0}, Void {1}, Requested amount {2} {3}. ", last4Digit, voidResult, requestedAmount, currency));
                    }

                    string refundResult = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.RefundResult);
                    if (!string.IsNullOrWhiteSpace(refundResult))
                    {
                        string last4Digit = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.Last4Digits);
                        decimal? approvedAmount = PaymentUtilities.GetPropertyDecimalValue(responseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.ApprovedAmount);
                        string currency = PaymentUtilities.GetPropertyStringValue(responseProperties, GenericNamespace.RefundResponse, RefundResponseProperties.CurrencyCode);
                        result.Append(string.Format("Card {0}, Refund {1}, Approved amount {2} {3}. ", last4Digit, refundResult, approvedAmount, currency));
                    }

                    string step = "result: ";
                    if (result.Length > 0)
                    {
                        step += result.ToString();
                    }
                    else
                    {
                        step += "No result. ";
                    }

                    RetailLogger.Log.PaymentConnectorLogOperation(methodName, step, connectorName, platform);
                }
            }

            /// <summary>
            /// Logs the response before return.
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            /// <param name="connectorName">Name of the connector.</param>
            /// <param name="platform">The platform.</param>
            /// <param name="response">The response.</param>
            /// <exception cref="ArgumentNullException">response.</exception>
            internal static void LogResponseBeforeReturn(string methodName, string connectorName, string platform, Response response)
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                LogBeforeReturn(methodName, connectorName, platform, response.Errors);
            }

            /// <summary>
            /// Validates the type of the card.
            /// </summary>
            /// <param name="supportedCardTypes">The supported card types.</param>
            /// <param name="cardType">Type of the card.</param>
            /// <returns><c>true</c> if card type is valid; <c>false</c> otherwise.</returns>
            internal static bool ValidateCardType(string supportedCardTypes, string cardType)
            {
                if (string.IsNullOrWhiteSpace(supportedCardTypes) || string.IsNullOrWhiteSpace(cardType))
                {
                    return false;
                }

                string[] cardTypes = supportedCardTypes.Split(';');
                return cardTypes.Any(t => t.Equals(cardType, StringComparison.OrdinalIgnoreCase));
            }

            /// <summary>
            /// Validates the currency code.
            /// </summary>
            /// <param name="supportedCurrencies">The supported currencies.</param>
            /// <param name="currencyCode">The currency code.</param>
            /// <returns><c>true</c> if currency code is valid; <c>false</c> otherwise.</returns>
            internal static bool ValidateCurrencyCode(string supportedCurrencies, string currencyCode)
            {
                if (string.IsNullOrWhiteSpace(supportedCurrencies) || string.IsNullOrWhiteSpace(currencyCode))
                {
                    return false;
                }

                string[] currencies = supportedCurrencies.Split(';');
                return currencies.Any(c => c.Equals(currencyCode, StringComparison.OrdinalIgnoreCase));
            }

            /// <summary>
            /// Validates the expiration date.
            /// </summary>
            /// <param name="expirationYear">The expiration year.</param>
            /// <param name="expirationMonth">The expiration month.</param>
            /// <returns><c>true</c> if expiration date is valid; <c>false</c> otherwise.</returns>
            internal static bool ValidateExpirationDate(decimal expirationYear, decimal expirationMonth)
            {
                DateTime now = DateTime.Now;
                return expirationYear > now.Year
                    || (expirationYear == now.Year && expirationMonth >= now.Month);
            }

            /// <summary>
            /// Parses the track1 for card number.
            /// </summary>
            /// <param name="track1">The track1.</param>
            /// <returns>Track1 string.</returns>
            internal static string ParseTrack1ForCardNumber(string track1)
            {
                if (string.IsNullOrWhiteSpace(track1))
                {
                    return null;
                }

                // Normally track 1 starts with %B but some hardware just send B as the start sentinel
                string symbolBeforeCardNumber = "B";
                string symbolAfterCardNumber = "^";
                int index1 = track1.IndexOf(symbolBeforeCardNumber, StringComparison.OrdinalIgnoreCase);
                int index2 = track1.IndexOf(symbolAfterCardNumber, StringComparison.OrdinalIgnoreCase);

                if (index1 < 0 || index2 < 0)
                {
                    return null;
                }

                int begin = index1 + symbolBeforeCardNumber.Length;
                int end = index2 - 1;

                if (begin > end)
                {
                    return null;
                }
                else
                {
                    return track1.Substring(begin, end - begin + 1);
                }
            }

            /// <summary>
            /// Parses the track2 for card number.
            /// </summary>
            /// <param name="track2">The track2.</param>
            /// <returns>Track2 data.</returns>
            internal static string ParseTrack2ForCardNumber(string track2)
            {
                if (string.IsNullOrWhiteSpace(track2))
                {
                    return null;
                }

                string symbolBeforeCardNumber = ";";
                string symbolAfterCardNumber = "=";
                int index1 = track2.IndexOf(symbolBeforeCardNumber, StringComparison.OrdinalIgnoreCase);
                int index2 = track2.IndexOf(symbolAfterCardNumber, StringComparison.OrdinalIgnoreCase);

                // Normally track 2 starts with ; but some hardware remove the start sentinel, so ignore not finding it
                if (index2 < 0)
                {
                    return null;
                }

                int begin = index1 + symbolBeforeCardNumber.Length;
                int end = index2 - 1;

                if (begin > end)
                {
                    return null;
                }
                else
                {
                    return track2.Substring(begin, end - begin + 1);
                }
            }

            /// <summary>
            /// Determines whether purchase level is level 2 or higher.
            /// </summary>
            /// <param name="purchaseLevel">The purchase level.</param>
            /// <returns>
            ///   <c>true</c> if purchase level is level 2 or higher; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsLevel2OrHigher(string purchaseLevel)
            {
                return PurchaseLevel.Level2.ToString().Equals(purchaseLevel, StringComparison.OrdinalIgnoreCase) || IsLevel3(purchaseLevel);
            }

            /// <summary>
            /// Determines whether purchase level is level 3 or higher.
            /// </summary>
            /// <param name="purchaseLevel">The purchase level.</param>
            /// <returns>
            ///   <c>true</c> if purchase level is level 3 or higher; otherwise, <c>false</c>.
            /// </returns>
            internal static bool IsLevel3(string purchaseLevel)
            {
                return PurchaseLevel.Level3.ToString().Equals(purchaseLevel, StringComparison.OrdinalIgnoreCase);
            }

            private static IEnumerable<TaxDetail> GetTaxDetails(Hashtable properties, string propertyNamespace, string propertyName)
            {
                IEnumerable<TaxDetail> taxDetails = null;
                PaymentProperty[] taxDetailsPropertyArray;
                if (PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out taxDetailsPropertyArray))
                {
                    if (taxDetailsPropertyArray.Length > 0)
                    {
                        taxDetails = new List<TaxDetail>();
                        foreach (var taxDetailsProperty in taxDetailsPropertyArray)
                        {
                            PaymentProperty[] taxDetailPropertyArray = taxDetailsProperty.PropertyList;
                            if (taxDetailPropertyArray != null)
                            {
                                Hashtable taxDetailProperties = PaymentProperty.ConvertToHashtable(taxDetailPropertyArray);
                                var taxDetail = new TaxDetail();
                                taxDetail.TaxTypeIdentifier = PaymentUtilities.GetPropertyStringValue(taxDetailProperties, GenericNamespace.TaxDetail, TaxDetailProperties.TaxTypeIdentifier);
                                taxDetail.TaxRate = PaymentUtilities.GetPropertyDecimalValue(taxDetailProperties, GenericNamespace.TaxDetail, TaxDetailProperties.TaxRate);
                                taxDetail.TaxDescription = PaymentUtilities.GetPropertyStringValue(taxDetailProperties, GenericNamespace.TaxDetail, TaxDetailProperties.TaxDescription);
                                taxDetail.TaxAmount = PaymentUtilities.GetPropertyDecimalValue(taxDetailProperties, GenericNamespace.TaxDetail, TaxDetailProperties.TaxAmount);
                                (taxDetails as List<TaxDetail>).Add(taxDetail);
                            }
                        }
                    }
                }

                return taxDetails;
            }

            private static IEnumerable<MiscellaneousCharge> GetMiscellaneousCharges(Hashtable properties, string propertyNamespace, string propertyName)
            {
                IEnumerable<MiscellaneousCharge> miscellaneousCharges = null;
                PaymentProperty[] miscellaneousChargesPropertyArray;
                if (PaymentProperty.GetPropertyValue(properties, propertyNamespace, propertyName, out miscellaneousChargesPropertyArray))
                {
                    if (miscellaneousChargesPropertyArray.Length > 0)
                    {
                        miscellaneousCharges = new List<MiscellaneousCharge>();
                        foreach (var miscellaneousChargesProperty in miscellaneousChargesPropertyArray)
                        {
                            PaymentProperty[] miscellaneousChargePropertyArray = miscellaneousChargesProperty.PropertyList;
                            if (miscellaneousChargePropertyArray != null)
                            {
                                Hashtable miscellaneousChargeProperties = PaymentProperty.ConvertToHashtable(miscellaneousChargePropertyArray);
                                var miscellaneousCharge = new MiscellaneousCharge();
                                miscellaneousCharge.ChargeType = PaymentUtilities.GetPropertyStringValue(miscellaneousChargeProperties, GenericNamespace.MiscellaneousCharge, MiscellaneousChargeProperties.ChargeType);
                                miscellaneousCharge.ChargeAmount = PaymentUtilities.GetPropertyDecimalValue(miscellaneousChargeProperties, GenericNamespace.MiscellaneousCharge, MiscellaneousChargeProperties.ChargeAmount);
                                (miscellaneousCharges as List<MiscellaneousCharge>).Add(miscellaneousCharge);
                            }
                        }
                    }
                }

                return miscellaneousCharges;
            }

            private static void LogBeforeReturn(string methodName, string connectorName, string platform, IList<PaymentError> errors)
            {
                if (errors != null && errors.Count > 0)
                {
                    RetailLogger.Log.PaymentConnectorLogErrors("methodName", connectorName, platform, PaymentError.GetErrorsAsTraceString(errors));
                }

                RetailLogger.Log.PaymentConnectorLogOperation(methodName, OperationCompleted, connectorName, platform);
            }

            private static string MissingPropertyMessage(string propertyNamespace, string propertyName)
            {
                return string.Format("Missing {0}:{1}.", propertyNamespace, propertyName);
            }
        }
    }
}
