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
        /// Response for <see cref="AuthorizeRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class AuthorizeResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AuthorizeResponse"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal AuthorizeResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            /// Gets or sets the type of the card.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            /// Gets or sets whether the card was swiped.
            /// </summary>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            /// Gets or sets the last 4 digits.
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
            /// Gets or sets the voice authorization code.
            /// </summary>
            /// <value>
            /// The voice authorization code.
            /// </value>
            internal string VoiceAuthorizationCode { get; set; }

            /// <summary>
            /// Gets or sets the cash back amount.
            /// </summary>
            /// <value>
            /// The cash back amount.
            /// </value>
            internal decimal? CashBackAmount { get; set; }

            /// <summary>
            /// Gets or sets the type of the account.
            /// </summary>
            /// <value>
            /// The type of the account.
            /// </value>
            internal string AccountType { get; set; }

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
            /// Gets or sets the approved amount.
            /// </summary>
            /// <value>
            /// The approved amount.
            /// </value>
            internal decimal? ApprovedAmount { get; set; }

            /// <summary>
            /// Gets or sets the currency code.
            /// </summary>
            /// <value>
            /// The currency code.
            /// </value>
            internal string CurrencyCode { get; set; }

            /// <summary>
            /// Gets or sets the authorization result.
            /// </summary>
            /// <value>
            /// The authorization result.
            /// </value>
            internal string AuthorizationResult { get; set; }

            /// <summary>
            /// Gets or sets the provider message.
            /// </summary>
            /// <value>
            /// The provider message.
            /// </value>
            internal string ProviderMessage { get; set; }

            /// <summary>
            /// Gets or sets the AVS result.
            /// </summary>
            /// <value>
            /// The avs result.
            /// </value>
            internal string AVSResult { get; set; }

            /// <summary>
            /// Gets or sets the AVS detail.
            /// </summary>
            /// <value>
            /// The avs detail.
            /// </value>
            internal string AVSDetail { get; set; }

            /// <summary>
            /// Gets or sets the CVV2 result.
            /// </summary>
            /// <value>
            /// The cv v2 result.
            /// </value>
            internal string CVV2Result { get; set; }

            /// <summary>
            /// Gets or sets the available balance.
            /// </summary>
            /// <value>
            /// The available balance.
            /// </value>
            internal decimal? AvailableBalance { get; set; }

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
            /// Converts to <see cref="AuthorizeResponse"/> to <see cref="Response"/>.
            /// </summary>
            /// <param name="authorizeResponse">The authorize response.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response ConvertTo(AuthorizeResponse authorizeResponse)
            {
                var response = new Response();
                authorizeResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                var authorizationResponseProperties = new List<PaymentProperty>();
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CardType, authorizeResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.IsSwiped, authorizeResponse.IsSwipe);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Last4Digits, authorizeResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CardToken, authorizeResponse.CardToken);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.UniqueCardId, authorizeResponse.UniqueCardId);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.VoiceAuthorizationCode, authorizeResponse.VoiceAuthorizationCode);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CashBackAmount, authorizeResponse.CashBackAmount);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AccountType, authorizeResponse.AccountType);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ProviderTransactionId, authorizeResponse.ProviderTransactionId);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ApprovalCode, authorizeResponse.ApprovalCode);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ResponseCode, authorizeResponse.ResponseCode);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ApprovedAmount, authorizeResponse.ApprovedAmount);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CurrencyCode, authorizeResponse.CurrencyCode);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AuthorizationResult, authorizeResponse.AuthorizationResult);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.ProviderMessage, authorizeResponse.ProviderMessage);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AVSResult, authorizeResponse.AVSResult);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AVSDetail, authorizeResponse.AVSDetail);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.CVV2Result, authorizeResponse.CVV2Result);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.AvailableBalance, authorizeResponse.AvailableBalance);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.TransactionType, authorizeResponse.TransactionType);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.TransactionDateTime, authorizeResponse.TransactionDateTime);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.PaymentTrackingId, authorizeResponse.PaymentTrackingId);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.SupportsMultipleCaptures, "True");
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.PaymentMethodName, authorizeResponse.PaymentMethodName);
                PaymentUtilities.AddPropertyIfPresent(authorizationResponseProperties, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.PaymentReference, authorizeResponse.PaymentReference);
                properties.Add(new PaymentProperty(GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Properties, authorizationResponseProperties.ToArray()));

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
