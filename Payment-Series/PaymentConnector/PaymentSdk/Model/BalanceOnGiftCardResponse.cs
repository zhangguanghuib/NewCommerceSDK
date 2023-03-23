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
        using System.Collections.Generic;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Response that contains gift card balance.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class BalanceOnGiftCardResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BalanceOnGiftCardResponse" /> class.
            /// Contains attributes associated with a Balance on Gift Card response.
            /// Also contains a helper method to convert <see cref="BalanceOnGiftCardResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="locale">The locale value derived from the request.</param>
            /// <param name="serviceAccountId">Service Account ID associated with the Merchant Account derived from the request.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal BalanceOnGiftCardResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            ///  Gets or sets the card type associated with the Balance On Gift Card request.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Balance On Gift Card request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            ///  Gets or sets the Provider Transaction ID for this request/response.
            /// </summary>
            internal string ProviderTransactionId { get; set; }

            /// <summary>
            /// Gets or sets the available balance on the gift card.
            /// </summary>
            internal decimal? AvailableBalance { get; set; }

            /// <summary>
            /// Gets or sets the result of request after it has been processed.
            /// </summary>
            internal string BalanceOnGiftCardResult { get; set; }

            /// <summary>
            /// Will convert the passed-in <see cref="BalanceOnGiftCardResponse"/> object into a <see cref="Response"/> object.
            /// </summary>
            /// <param name="bogcResponse"><see cref="BalanceOnGiftCardResponse"/> object to be parsed and converted into a <see cref="Response"/> object.</param>
            /// <returns><see cref="Response"/> object derived from the parsing of the passed-in <see cref="BalanceOnGiftCardResponse"/> object.</returns>
            internal static Response ConvertTo(BalanceOnGiftCardResponse bogcResponse)
            {
                Response response = new Response();
                bogcResponse.WriteBaseProperties(response);

                List<PaymentProperty> properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                List<PaymentProperty> bogcResponseProperties = new List<PaymentProperty>();
                PaymentUtilities.AddPropertyIfPresent(bogcResponseProperties, GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.CardType, bogcResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(bogcResponseProperties, GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.BalanceOnGiftCardResult, bogcResponse.BalanceOnGiftCardResult);
                PaymentUtilities.AddPropertyIfPresent(bogcResponseProperties, GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.AvailableBalance, bogcResponse.AvailableBalance);
                PaymentUtilities.AddPropertyIfPresent(bogcResponseProperties, GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.Last4Digits, bogcResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(bogcResponseProperties, GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.ProviderTransactionId, bogcResponse.ProviderTransactionId);
                properties.Add(new PaymentProperty(GenericNamespace.BalanceOnGiftCardResponse, BalanceOnGiftCardProperties.Properties, bogcResponseProperties.ToArray()));

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
