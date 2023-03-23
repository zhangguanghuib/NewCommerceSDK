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
        /// Response for <see cref="VoidRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class VoidResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VoidResponse" /> class.
            /// Contains attributes associated with a VoidResponse response.
            /// Also contains a helper method to convert <see cref="VoidResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="locale">The locale value derived from the request.</param>
            /// <param name="serviceAccountId">Service Account ID associated with the Merchant Account derived from the request.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal VoidResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            ///  Gets or sets the card type associated with the Void request.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Void request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            ///  Gets or sets the unique card ID associated with the Void request.
            /// </summary>
            internal string UniqueCardId { get; set; }

            /// <summary>
            ///  Gets or sets the provider transaction ID associated with the Void request.
            /// </summary>
            internal string ProviderTransactionId { get; set; }

            /// <summary>
            ///  Gets or sets the response code associated with the Void request.
            /// </summary>
            internal string ResponseCode { get; set; }

            /// <summary>
            ///  Gets or sets the currency code associated with the Void request.
            /// </summary>
            internal string CurrencyCode { get; set; }

            /// <summary>
            ///  Gets or sets the void result associated with the Void request.
            /// </summary>
            internal string VoidResult { get; set; }

            /// <summary>
            ///  Gets or sets the provider message associated with the Void request.
            /// </summary>
            internal string ProviderMessage { get; set; }

            /// <summary>
            ///  Gets or sets the transaction type associated with the Void request.
            /// </summary>
            internal string TransactionType { get; set; }

            /// <summary>
            ///  Gets or sets the transaction date and time associated with the Void request.
            /// </summary>
            internal DateTime? TransactionDateTime { get; set; }

            /// <summary>
            ///  Gets or sets the close amount associated with the Void request. This is only set if AuthorizationResponse is
            ///  missing from the original Void request.
            /// </summary>
            internal decimal? CloseAmount { get; set; }

            /// <summary>
            /// Gets or sets the payment tracking identifier.
            /// </summary>
            /// <value>
            /// The payment tracking identifier.
            /// </value>
            internal string PaymentTrackingId { get; set; }

            /// <summary>
            /// Gets or sets the name of the payment method.
            /// </summary>
            /// <value>
            /// The payment method name.
            /// </value>
            internal string PaymentMethodName { get; set; }

            /// <summary>
            /// Gets or sets the payment reference property.
            /// </summary>
            /// <value>
            /// The payment reference.
            /// </value>
            internal string PaymentReference { get; set; }

            /// <summary>
            /// Will convert the passed-in <see cref="VoidResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="voidResponse"><see cref="VoidResponse"/> object to be parsed and converted into a <see cref="Response"/> object.</param>
            /// <returns><see cref="Response"/> object derived from the parsing of the passed-in <see cref="VoidResponse"/> object.</returns>
            internal static Response ConvertTo(VoidResponse voidResponse)
            {
                var response = new Response();
                voidResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                var voidResponseProperties = new List<PaymentProperty>();
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CardType, voidResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.Last4Digits, voidResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.UniqueCardId, voidResponse.UniqueCardId);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ProviderTransactionId, voidResponse.ProviderTransactionId);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ResponseCode, voidResponse.ResponseCode);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CurrencyCode, voidResponse.CurrencyCode);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.VoidResult, voidResponse.VoidResult);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.ProviderMessage, voidResponse.ProviderMessage);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.TransactionType, voidResponse.TransactionType);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.TransactionDateTime, voidResponse.TransactionDateTime);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.CloseAmount, voidResponse.CloseAmount);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.PaymentTrackingId, voidResponse.PaymentTrackingId);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.PaymentMethodName, voidResponse.PaymentMethodName);
                PaymentUtilities.AddPropertyIfPresent(voidResponseProperties, GenericNamespace.VoidResponse, VoidResponseProperties.PaymentReference, voidResponse.PaymentReference);
                properties.Add(new PaymentProperty(GenericNamespace.VoidResponse, VoidResponseProperties.Properties, voidResponseProperties.ToArray()));

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
