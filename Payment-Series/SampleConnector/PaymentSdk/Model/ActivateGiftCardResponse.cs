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
        /// Response to activate gift card.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class ActivateGiftCardResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActivateGiftCardResponse" /> class.
            /// Contains attributes associated with a Activate Gift Card response.
            /// Also contains a helper method to convert <see cref="ActivateGiftCardResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="locale">The locale value derived from the request.</param>
            /// <param name="serviceAccountId">Service Account ID associated with the Merchant Account derived from the request.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal ActivateGiftCardResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            ///  Gets or sets the available balance on the gift card.
            /// </summary>
            internal decimal? AvailableBalance { get; set; }

            /// <summary>
            ///  Gets or sets the card type associated with the Activate Gift Card request. It should be <see cref="CardType.GiftCard"/>.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Activate Gift Card request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            ///  Gets or sets the Provider Transaction ID for this request/response.
            /// </summary>
            internal string ProviderTransactionId { get; set; }

            /// <summary>
            ///  Gets or sets the approval code associated with this request/response.
            /// </summary>
            internal string ApprovalCode { get; set; }

            /// <summary>
            ///  Gets or sets the result of the activation gift card attempt.
            /// </summary>
            internal string ActivateGiftCardResult { get; set; }

            /// <summary>
            /// Gets or sets the gift card number.
            /// </summary>
            internal string GiftCardNumber { get; set; }

            /// <summary>
            /// Gets or sets the expiration year.
            /// </summary>
            internal decimal? ExpirationYear { get; set; }

            /// <summary>
            /// Gets or sets the expiration month.
            /// </summary>
            internal decimal? ExpirationMonth { get; set; }

            /// <summary>
            /// Will convert the passed-in <see cref="ActivateGiftCardResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="activateGiftCardResponse"><see cref="ActivateGiftCardResponse"/> object to be parsed and converted into a <see cref="Response"/> object.</param>
            /// <returns><see cref="Response"/> object derived from the parsing of the passed-in <see cref="ActivateGiftCardResponse"/> object.</returns>
            internal static Response ConvertTo(ActivateGiftCardResponse activateGiftCardResponse)
            {
                var response = new Response();
                activateGiftCardResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                var activateGiftCardResponseProperties = new List<PaymentProperty>();
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.CardType, activateGiftCardResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.Last4Digits, activateGiftCardResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.GiftCardNumber, activateGiftCardResponse.GiftCardNumber);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.ProviderTransactionId, activateGiftCardResponse.ProviderTransactionId);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.ApprovalCode, activateGiftCardResponse.ApprovalCode);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.AvailableBalance, activateGiftCardResponse.AvailableBalance);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.ActivateGiftCardResult, activateGiftCardResponse.ActivateGiftCardResult);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.ExpirationYear, activateGiftCardResponse.ExpirationYear);
                PaymentUtilities.AddPropertyIfPresent(activateGiftCardResponseProperties, GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.ExpirationMonth, activateGiftCardResponse.ExpirationMonth);
                properties.Add(new PaymentProperty(GenericNamespace.ActivateGiftCardResponse, ActivateGiftCardResponseProperties.Properties, activateGiftCardResponseProperties.ToArray()));

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
