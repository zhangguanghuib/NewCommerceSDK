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
        /// Response for <see cref="CaptureRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class CaptureResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CaptureResponse"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal CaptureResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            /// Gets or sets the type of the card.
            /// </summary>
            /// <value>
            /// The type of the card.
            /// </value>
            internal string CardType { get; set; }

            /// <summary>
            /// Gets or sets the last4 digit.
            /// </summary>
            /// <value>
            /// The last4 digit.
            /// </value>
            internal string Last4Digit { get; set; }

            /// <summary>
            /// Gets or sets the card token.
            /// </summary>
            /// <value>
            /// The card token.
            /// </value>
            internal string CardToken { get; set; }

            /// <summary>
            /// Gets or sets the unique card identifier.
            /// </summary>
            /// <value>
            /// The unique card identifier.
            /// </value>
            internal string UniqueCardId { get; set; }

            /// <summary>
            /// Gets or sets the provider transaction identifier.
            /// </summary>
            /// <value>
            /// The provider transaction identifier.
            /// </value>
            internal string ProviderTransactionId { get; set; }

            /// <summary>
            /// Gets or sets the approval code.
            /// </summary>
            /// <value>
            /// The approval code.
            /// </value>
            internal string ApprovalCode { get; set; }

            /// <summary>
            /// Gets or sets the response code.
            /// </summary>
            /// <value>
            /// The response code.
            /// </value>
            internal string ResponseCode { get; set; }

            /// <summary>
            /// Gets or sets the currency code.
            /// </summary>
            /// <value>
            /// The currency code.
            /// </value>
            internal string CurrencyCode { get; set; }

            /// <summary>
            /// Gets or sets the capture result.
            /// </summary>
            /// <value>
            /// The capture result.
            /// </value>
            internal string CaptureResult { get; set; }

            /// <summary>
            /// Gets or sets the provider message.
            /// </summary>
            /// <value>
            /// The provider message.
            /// </value>
            internal string ProviderMessage { get; set; }

            /// <summary>
            /// Gets or sets the type of the transaction.
            /// </summary>
            /// <value>
            /// The type of the transaction.
            /// </value>
            internal string TransactionType { get; set; }

            /// <summary>
            /// Gets or sets the transaction date time.
            /// </summary>
            /// <value>
            /// The transaction date time.
            /// </value>
            internal DateTime? TransactionDateTime { get; set; }

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
            /// Converts to <see cref="Response"/>.
            /// </summary>
            /// <param name="captureResponse">The capture response.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response ConvertTo(CaptureResponse captureResponse)
            {
                var response = new Response();
                captureResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                var captureResponseProperties = new List<PaymentProperty>();
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CardType, captureResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.Last4Digits, captureResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CardToken, captureResponse.CardToken);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.UniqueCardId, captureResponse.UniqueCardId);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ProviderTransactionId, captureResponse.ProviderTransactionId);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ApprovalCode, captureResponse.ApprovalCode);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ResponseCode, captureResponse.ResponseCode);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CurrencyCode, captureResponse.CurrencyCode);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.CaptureResult, captureResponse.CaptureResult);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.ProviderMessage, captureResponse.ProviderMessage);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.TransactionType, captureResponse.TransactionType);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.TransactionDateTime, captureResponse.TransactionDateTime);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.CaptureResponse, CaptureResponseProperties.PaymentTrackingId, captureResponse.PaymentTrackingId);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.PaymentMethodName, captureResponse.PaymentMethodName);
                PaymentUtilities.AddPropertyIfPresent(captureResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.PaymentReference, captureResponse.PaymentReference);
                properties.Add(new PaymentProperty(GenericNamespace.CaptureResponse, CaptureResponseProperties.Properties, captureResponseProperties.ToArray()));

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
