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
        /// Response for <see cref="GenerateCardTokenRequest"/>.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.ResponseBase" />
        internal class GenerateCardTokenResponse : ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GenerateCardTokenResponse"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal GenerateCardTokenResponse(string locale, string serviceAccountId, string connectorName)
                : base(locale, serviceAccountId, connectorName)
            {
            }

            /// <summary>
            /// Gets or sets the BankIdentificationNumberStart for the card.
            /// The value is the starting number of card BIN (Bank Identification Number).
            /// For example, it should be 400000 for Visa, 510000 for MasterCard, 340000 for Amex, and 601100 for Discover.
            /// </summary>
            internal string BankIdentificationNumberStart { get; set; }

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
            /// Gets or sets the expiration year.
            /// </summary>
            /// <value>
            /// The expiration year.
            /// </value>
            internal decimal? ExpirationYear { get; set; }

            /// <summary>
            /// Gets or sets the expiration month.
            /// </summary>
            /// <value>
            /// The expiration month.
            /// </value>
            internal decimal? ExpirationMonth { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            internal string Name { get; set; }

            /// <summary>
            /// Gets or sets the street address.
            /// </summary>
            /// <value>
            /// The street address.
            /// </value>
            internal string StreetAddress { get; set; }

            /// <summary>
            /// Gets or sets the street address2.
            /// </summary>
            /// <value>
            /// The street address2.
            /// </value>
            internal string StreetAddress2 { get; set; }

            /// <summary>
            /// Gets or sets the city.
            /// </summary>
            /// <value>
            /// The city.
            /// </value>
            internal string City { get; set; }

            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            /// <value>
            /// The state.
            /// </value>
            internal string State { get; set; }

            /// <summary>
            /// Gets or sets the postal code.
            /// </summary>
            /// <value>
            /// The postal code.
            /// </value>
            internal string PostalCode { get; set; }

            /// <summary>
            /// Gets or sets the country.
            /// </summary>
            /// <value>
            /// The country.
            /// </value>
            internal string Country { get; set; }

            /// <summary>
            /// Gets or sets the phone.
            /// </summary>
            /// <value>
            /// The phone.
            /// </value>
            internal string Phone { get; set; }

            /// <summary>
            /// Gets or sets the type of the account.
            /// </summary>
            /// <value>
            /// The type of the account.
            /// </value>
            internal string AccountType { get; set; }

            /// <summary>
            /// Gets or sets the other card properties.
            /// </summary>
            /// <value>
            /// The other card properties.
            /// </value>
            internal IList<PaymentProperty> OtherCardProperties { get; set; }

            /// <summary>
            /// Converts to <see cref="Response"/>.
            /// </summary>
            /// <param name="tokenizeResponse">The tokenize response.</param>
            /// <returns>An instance of <see cref="Response"/>.</returns>
            internal static Response ConvertTo(GenerateCardTokenResponse tokenizeResponse)
            {
                var response = new Response();
                tokenizeResponse.WriteBaseProperties(response);

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.BankIdentificationNumberStart, tokenizeResponse.BankIdentificationNumberStart);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.CardType, tokenizeResponse.CardType);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Last4Digits, tokenizeResponse.Last4Digit);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.CardToken, tokenizeResponse.CardToken);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.UniqueCardId, tokenizeResponse.UniqueCardId);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.ExpirationYear, tokenizeResponse.ExpirationYear);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.ExpirationMonth, tokenizeResponse.ExpirationMonth);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Name, tokenizeResponse.Name);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.StreetAddress, tokenizeResponse.StreetAddress);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.StreetAddress2, tokenizeResponse.StreetAddress2);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.City, tokenizeResponse.City);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.State, tokenizeResponse.State);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.PostalCode, tokenizeResponse.PostalCode);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Country, tokenizeResponse.Country);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.Phone, tokenizeResponse.Phone);
                PaymentUtilities.AddPropertyIfPresent(properties, GenericNamespace.PaymentCard, PaymentCardProperties.AccountType, tokenizeResponse.AccountType);
                properties.AddRange(tokenizeResponse.OtherCardProperties);

                response.Properties = properties.ToArray();
                return response;
            }
        }
    }
}
