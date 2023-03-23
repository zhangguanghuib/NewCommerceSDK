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
        /// Request to authorize payment.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class AuthorizeRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AuthorizeRequest"/> class.
            /// </summary>
            internal AuthorizeRequest()
                : base()
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
            /// Gets or sets the card token.
            /// </summary>
            /// <value>
            /// The card token.
            /// </value>
            internal string CardToken { get; set; }

            /// <summary>
            /// Gets or sets the last4 digit.
            /// </summary>
            /// <value>
            /// The last4 digit.
            /// </value>
            internal string Last4Digit { get; set; }

            /// <summary>
            /// Gets or sets the encrypted pin.
            /// </summary>
            /// <value>
            /// The encrypted pin.
            /// </value>
            internal string EncryptedPin { get; set; }

            /// <summary>
            /// Gets or sets the additional security data.
            /// </summary>
            /// <value>
            /// The additional security data.
            /// </value>
            internal string AdditionalSecurityData { get; set; }

            /// <summary>
            /// Gets or sets the card verification value.
            /// </summary>
            /// <value>
            /// The card verification value.
            /// </value>
            internal string CardVerificationValue { get; set; }

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
            /// Gets or sets the unique card identifier.
            /// </summary>
            /// <value>
            /// The unique card identifier.
            /// </value>
            internal string UniqueCardId { get; set; }

            /// <summary>
            /// Gets or sets the amount.
            /// </summary>
            /// <value>
            /// The amount.
            /// </value>
            internal decimal? Amount { get; set; }

            /// <summary>
            /// Gets or sets the currency code.
            /// </summary>
            /// <value>
            /// The currency code.
            /// </value>
            internal string CurrencyCode { get; set; }

            /// <summary>
            /// Gets or sets the support card tokenization.
            /// </summary>
            /// <value>
            /// The support card tokenization.
            /// </value>
            internal bool? SupportCardTokenization { get; set; }

            /// <summary>
            /// Gets or sets the allow partial authorization.
            /// </summary>
            /// <value>
            /// The allow partial authorization.
            /// </value>
            internal bool? AllowPartialAuthorization { get; set; }

            /// <summary>
            /// Gets or sets the authorization provider transaction identifier.
            /// </summary>
            /// <value>
            /// The authorization provider transaction identifier.
            /// </value>
            internal string AuthorizationProviderTransactionId { get; set; }

            /// <summary>
            /// Gets or sets the purchase level.
            /// </summary>
            /// <value>
            /// The purchase level.
            /// </value>
            internal string PurchaseLevel { get; set; }

            /// <summary>
            /// Gets or sets the payment tracking identifier.
            /// </summary>
            /// <value>
            /// The payment tracking identifier.
            /// </value>
            internal string PaymentTrackingId { get; set; }

            /// <summary>
            /// Converts from <see cref="Request"/>.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>New instance of <see cref="AuthorizeRequest"/>.</returns>
            internal static AuthorizeRequest ConvertFrom(Request request)
            {
                var authorizeRequest = new AuthorizeRequest();
                var errors = new List<PaymentError>();
                authorizeRequest.ReadBaseProperties(request, errors);

                // Read card data
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                authorizeRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);
                if (authorizeRequest.CardType != null
                    && !PaymentUtilities.ValidateCardType(authorizeRequest.SupportedTenderTypes, authorizeRequest.CardType))
                {
                    errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format("Card type is not supported: {0}.", authorizeRequest.CardType)));
                }

                authorizeRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.IsSwipe);
                if (authorizeRequest.IsSwipe ?? false)
                {
                    authorizeRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track1);
                    authorizeRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track2);

                    authorizeRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(authorizeRequest.Track1);
                    if (authorizeRequest.CardNumber == null)
                    {
                        authorizeRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(authorizeRequest.Track2);
                    }

                    if (authorizeRequest.CardNumber == null)
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                    }

                    decimal expirationYear, expirationMonth;
                    HelperUtilities.ParseTrackDataForExpirationDate(authorizeRequest.Track1 ?? string.Empty, authorizeRequest.Track2 ?? string.Empty, out expirationYear, out expirationMonth);
                    authorizeRequest.ExpirationYear = expirationYear;
                    authorizeRequest.ExpirationMonth = expirationMonth;
                }
                else
                {
                    authorizeRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardToken);

                    if (authorizeRequest.CardToken == null)
                    {
                        authorizeRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardNumber,
                            errors,
                            ErrorCode.InvalidCardNumber);
                    }
                    else
                    {
                        authorizeRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Last4Digits);
                    }

                    if (!string.Equals(authorizeRequest.CardType, Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        authorizeRequest.ExpirationYear = PaymentUtilities.GetPropertyDecimalValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.ExpirationYear,
                        errors,
                        ErrorCode.InvalidExpirationDate);
                        authorizeRequest.ExpirationMonth = PaymentUtilities.GetPropertyDecimalValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.ExpirationMonth,
                            errors,
                            ErrorCode.InvalidExpirationDate);
                    }
                }

                // Don't validate gift cards
                if (authorizeRequest.CardNumber != null
                    && !authorizeRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase)
                    && !HelperUtilities.ValidateBankCardNumber(authorizeRequest.CardNumber))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "Invalid card number."));
                }

                if (authorizeRequest.ExpirationYear != null && authorizeRequest.ExpirationYear.HasValue
                    && authorizeRequest.ExpirationMonth.HasValue
                    && authorizeRequest.ExpirationYear >= 0M
                    && authorizeRequest.ExpirationMonth != null && authorizeRequest.ExpirationMonth >= 0M
                    && !PaymentUtilities.ValidateExpirationDate(authorizeRequest.ExpirationYear.Value, authorizeRequest.ExpirationMonth.Value))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, "Invalid expiration date."));
                }

                if (Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.Debit.ToString().Equals(authorizeRequest.CardType, StringComparison.OrdinalIgnoreCase))
                {
                    authorizeRequest.EncryptedPin = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.EncryptedPin,
                        errors,
                        ErrorCode.CannotVerifyPin);
                    authorizeRequest.AdditionalSecurityData = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.AdditionalSecurityData);
                }

                authorizeRequest.CashBackAmount = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CashBackAmount);
                if (authorizeRequest.CashBackAmount.HasValue
                    && authorizeRequest.CashBackAmount > 0M
                    && !Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.Debit.ToString().Equals(authorizeRequest.CardType, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new PaymentError(ErrorCode.CashBackNotAvailable, "Cashback is not available."));
                }

                authorizeRequest.CardVerificationValue = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardVerificationValue);
                authorizeRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.VoiceAuthorizationCode);
                authorizeRequest.Name = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Name);
                authorizeRequest.StreetAddress = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.StreetAddress);
                authorizeRequest.StreetAddress2 = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.StreetAddress2);
                authorizeRequest.City = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.City);
                authorizeRequest.State = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.State);
                authorizeRequest.PostalCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.PostalCode);
                authorizeRequest.Country = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Country);
                authorizeRequest.Phone = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.Phone);
                authorizeRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.AccountType);
                authorizeRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.UniqueCardId);

                // Read transaction data
                authorizeRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Amount,
                    errors,
                    ErrorCode.InvalidAmount);
                authorizeRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.CurrencyCode,
                    errors,
                    ErrorCode.InvalidRequest);
                if (authorizeRequest.CurrencyCode != null
                    && !PaymentUtilities.ValidateCurrencyCode(authorizeRequest.SupportedCurrencies, authorizeRequest.CurrencyCode))
                {
                    errors.Add(new PaymentError(ErrorCode.UnsupportedCurrency, string.Format("Currency code is not supported: {0}.", authorizeRequest.CurrencyCode)));
                }

                authorizeRequest.SupportCardTokenization = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.SupportCardTokenization);
                authorizeRequest.AllowPartialAuthorization = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.AllowPartialAuthorization);

                authorizeRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.AuthorizationProviderTransactionId);

                authorizeRequest.PurchaseLevel = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PurchaseLevel);

                authorizeRequest.PaymentTrackingId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PaymentTrackingId);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return authorizeRequest;
            }
        }
    }
}
