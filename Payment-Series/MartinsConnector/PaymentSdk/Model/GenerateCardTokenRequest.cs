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
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Request to generate card token.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class GenerateCardTokenRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GenerateCardTokenRequest"/> class.
            /// </summary>
            internal GenerateCardTokenRequest() : base()
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
            /// Gets or sets a value indicating whether the card was swiped.
            /// </summary>
            /// <value>
            /// The is swipe.
            /// </value>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            /// Gets or sets the card number.
            /// </summary>
            /// <value>
            /// The card number.
            /// </value>
            internal string CardNumber { get; set; }

            /// <summary>
            /// Gets or sets the track1.
            /// </summary>
            /// <value>
            /// The track1.
            /// </value>
            internal string Track1 { get; set; }

            /// <summary>
            /// Gets or sets the track2.
            /// </summary>
            /// <value>
            /// The track2.
            /// </value>
            internal string Track2 { get; set; }

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
            /// Converts <paramref name="request"/> and <paramref name="requiredInteractionProperties"/> to <see cref="GenerateCardTokenRequest"/>.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="requiredInteractionProperties">The required interaction properties.</param>
            /// <returns>An instance of <see cref="GenerateCardTokenRequest"/>.</returns>
            internal static GenerateCardTokenRequest ConvertFrom(Request request, PaymentProperty[] requiredInteractionProperties)
            {
                var tokenizeRequest = new GenerateCardTokenRequest();
                var errors = new List<PaymentError>();
                tokenizeRequest.ReadBaseProperties(request, errors);

                PaymentProperty[] cardProperties = null;
                if (requiredInteractionProperties != null)
                {
                    // Get card data from interaction form
                    IExtensions handler = SDKExtensions.Extension;
                    if (handler != null)
                    {
                        // We have found the implementation for the form
                        Dictionary<string, PaymentProperty> interactionPropertyDictionary = null;
                        if (handler.GetCreditCardDetails(requiredInteractionProperties, out interactionPropertyDictionary))
                        {
                            cardProperties = interactionPropertyDictionary.Values.ToArray();
                        }
                    }
                    else
                    {
                        errors.Add(new PaymentError(ErrorCode.UserAborted, "User aborted data entry form."));
                    }
                }
                else
                {
                    // Get card data from request.
                    cardProperties = request.Properties;
                }

                // Read card data
                tokenizeRequest.OtherCardProperties = new List<PaymentProperty>();
                if (cardProperties != null)
                {
                    Hashtable hashtable = PaymentProperty.ConvertToHashtable(cardProperties);
                    tokenizeRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardType,
                        errors,
                        ErrorCode.InvalidRequest);
                    if (tokenizeRequest.CardType != null
                        && (!PaymentUtilities.ValidateCardType(tokenizeRequest.SupportedTenderTypes, tokenizeRequest.CardType)
                            || tokenizeRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.Debit.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format("Card type is not supported: {0}.", tokenizeRequest.CardType)));
                    }

                    tokenizeRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.IsSwipe);
                    if (tokenizeRequest.IsSwipe ?? false)
                    {
                        tokenizeRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Track1);
                        tokenizeRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Track2);

                        string cardNumber = PaymentUtilities.ParseTrack1ForCardNumber(tokenizeRequest.Track1);
                        if (cardNumber == null)
                        {
                            cardNumber = PaymentUtilities.ParseTrack2ForCardNumber(tokenizeRequest.Track2);
                        }

                        if (cardNumber == null)
                        {
                            errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                        }

                        tokenizeRequest.CardNumber = HelperUtilities.GetMaskedCardNumber(cardNumber);

                        decimal expirationYear, expirationMonth;
                        HelperUtilities.ParseTrackDataForExpirationDate(tokenizeRequest.Track1 ?? string.Empty, tokenizeRequest.Track2 ?? string.Empty, out expirationYear, out expirationMonth);
                        tokenizeRequest.ExpirationYear = expirationYear;
                        tokenizeRequest.ExpirationMonth = expirationMonth;
                    }
                    else
                    {
                        tokenizeRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardNumber,
                        errors,
                        ErrorCode.InvalidCardNumber);
                        if (tokenizeRequest.CardNumber != null
                            && !HelperUtilities.ValidateBankCardNumber(tokenizeRequest.CardNumber))
                        {
                            errors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "Invalid card number."));
                        }

                        tokenizeRequest.ExpirationYear = PaymentUtilities.GetPropertyDecimalValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.ExpirationYear,
                            errors,
                            ErrorCode.InvalidExpirationDate);
                        tokenizeRequest.ExpirationMonth = PaymentUtilities.GetPropertyDecimalValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.ExpirationMonth,
                            errors,
                            ErrorCode.InvalidExpirationDate);
                    }

                    if (tokenizeRequest.ExpirationYear.HasValue
                        && tokenizeRequest.ExpirationMonth.HasValue
                        && tokenizeRequest.ExpirationYear >= 0M
                        && tokenizeRequest.ExpirationMonth >= 0M
                        && !PaymentUtilities.ValidateExpirationDate(tokenizeRequest.ExpirationYear.Value, tokenizeRequest.ExpirationMonth.Value))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, "Invalid expiration date."));
                    }

                    tokenizeRequest.Name = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Name);
                    tokenizeRequest.StreetAddress = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.StreetAddress);
                    tokenizeRequest.StreetAddress2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.StreetAddress2);
                    tokenizeRequest.City = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.City);
                    tokenizeRequest.State = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.State);
                    tokenizeRequest.PostalCode = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.PostalCode);
                    tokenizeRequest.Country = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Country);
                    tokenizeRequest.Phone = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Phone);
                    tokenizeRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.AccountType);

                    // Add other custom card properties from interaction properties
                    foreach (var cardProperty in cardProperties)
                    {
                        if (GenericNamespace.PaymentCard.Equals(cardProperty.Namespace, StringComparison.Ordinal) && IsCustomCardProperty(cardProperty.Name))
                        {
                            tokenizeRequest.OtherCardProperties.Add(cardProperty);
                        }
                    }
                }

                // Add other custom card properties from request properties
                foreach (var requestProperty in request.Properties)
                {
                    if (GenericNamespace.PaymentCard.Equals(requestProperty.Namespace, StringComparison.Ordinal)
                        && !tokenizeRequest.OtherCardProperties.Any(p => p.Name.Equals(requestProperty.Name, StringComparison.OrdinalIgnoreCase))
                        && IsCustomCardProperty(requestProperty.Name))
                    {
                        tokenizeRequest.OtherCardProperties.Add(requestProperty);
                    }
                }

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return tokenizeRequest;
            }

            private static bool IsCustomCardProperty(string propertyName)
            {
                return !PaymentCardProperties.AccountType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.AdditionalSecurityData.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CardEntryType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CardNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CardToken.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CardType.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CardVerificationValue.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.CashBackAmount.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.City.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Country.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.EncryptedPin.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.ExpirationMonth.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.ExpirationYear.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.IsSwipe.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Last4Digits.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Phone.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.PostalCode.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.State.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.StreetAddress.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.StreetAddress2.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track1.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track1Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track1KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track2.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track2Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track2KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track3.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track3Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track3KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track4.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track4Encrypted.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.Track4KeySerialNumber.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.UniqueCardId.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    && !PaymentCardProperties.VoiceAuthorizationCode.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
