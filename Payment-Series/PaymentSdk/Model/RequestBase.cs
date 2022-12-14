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
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Base request class.
        /// </summary>
        internal abstract class RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RequestBase"/> class.
            /// </summary>
            internal RequestBase()
                : base()
            {
            }

            /// <summary>
            /// Gets or sets the locale.
            /// </summary>
            /// <value>
            /// The locale.
            /// </value>
            internal string Locale { get; set; }

            /// <summary>
            /// Gets or sets the name of the assembly.
            /// </summary>
            /// <value>
            /// The name of the assembly.
            /// </value>
            internal string AssemblyName { get; set; }

            /// <summary>
            /// Gets or sets the service account identifier.
            /// </summary>
            /// <value>
            /// The service account identifier.
            /// </value>
            internal string ServiceAccountId { get; set; }

            /// <summary>
            /// Gets or sets the merchant identifier.
            /// </summary>
            /// <value>
            /// The merchant identifier.
            /// </value>
            internal string MerchantId { get; set; }

            /// <summary>
            /// Gets or sets the provider identifier.
            /// </summary>
            /// <value>
            /// The provider identifier.
            /// </value>
            internal string ProviderId { get; set; }

            /// <summary>
            /// Gets or sets the environment.
            /// </summary>
            /// <value>
            /// The environment.
            /// </value>
            internal string Environment { get; set; }

            /// <summary>
            /// Gets or sets the test string.
            /// </summary>
            /// <value>
            /// The test string.
            /// </value>
            internal string TestString { get; set; }

            /// <summary>
            /// Gets or sets the test decimal.
            /// </summary>
            /// <value>
            /// The test decimal.
            /// </value>
            internal decimal? TestDecimal { get; set; }

            /// <summary>
            /// Gets or sets the test date.
            /// </summary>
            /// <value>
            /// The test date.
            /// </value>
            internal DateTime? TestDate { get; set; }

            /// <summary>
            /// Gets or sets the supported currencies.
            /// </summary>
            /// <value>
            /// The supported currencies.
            /// </value>
            internal string SupportedCurrencies { get; set; }

            /// <summary>
            /// Gets or sets the supported tender types.
            /// </summary>
            /// <value>
            /// The supported tender types.
            /// </value>
            internal string SupportedTenderTypes { get; set; }

            /// <summary>
            /// Gets or sets the type of the industry.
            /// </summary>
            /// <value>
            /// The type of the industry.
            /// </value>
            internal string IndustryType { get; set; }

            /// <summary>
            /// Gets or sets the is test mode.
            /// </summary>
            /// <value>
            /// The is test mode.
            /// </value>
            internal bool? IsTestMode { get; set; }

            /// <summary>
            /// Reads the base properties.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="errors">The errors.</param>
            /// <exception cref="SampleException">
            /// Request is null.
            /// or
            /// Locale is null or whitespaces.
            /// or
            /// Request properties is null or empty.
            /// </exception>
            protected void ReadBaseProperties(Request request, List<PaymentError> errors)
            {
                if (request == null)
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Request is null.");
                }

                if (string.IsNullOrWhiteSpace(request.Locale))
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Locale is null or whitespaces.");
                }
                else
                {
                    this.Locale = request.Locale;
                }

                if (request.Properties == null || request.Properties.Length == 0)
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Request properties is null or empty.");
                }

                Hashtable properties = PaymentProperty.ConvertToHashtable(request.Properties);
                this.AssemblyName = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.AssemblyName,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.ServiceAccountId = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.ServiceAccountId,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.MerchantId = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.MerchantId,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.ProviderId = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.ProviderId,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.Environment = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.Environment);
                this.TestString = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestString,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.TestDecimal = PaymentUtilities.GetPropertyDecimalValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestDecimal,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.TestDate = PaymentUtilities.GetPropertyDateTimeValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    SampleMerchantAccountProperty.TestDate,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.SupportedCurrencies = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.SupportedCurrencies,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.SupportedTenderTypes = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.MerchantAccount,
                    MerchantAccountProperties.SupportedTenderTypes,
                    errors,
                    ErrorCode.InvalidMerchantProperty);
                this.IndustryType = PaymentUtilities.GetPropertyStringValue(
                    properties,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.IndustryType);
                this.IsTestMode = PaymentUtilities.GetPropertyBooleanValue(
                    properties,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.IsTestMode);
            }
        }
    }
}
