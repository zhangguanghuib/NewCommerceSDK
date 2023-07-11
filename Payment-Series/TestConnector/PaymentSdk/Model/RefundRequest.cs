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
        /// Contains attributes associated with a Refund request.
        /// Also contains a helper method to convert <see cref="Request"/> object into a <see cref="RefundRequest"/> object.
        /// </summary>
        internal class RefundRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RefundRequest"/> class.
            /// </summary>
            internal RefundRequest()
                : base()
            {
            }

            /// <summary>
            /// Gets or sets the card type associated with the Refund request.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            /// Gets or sets whether the request was a swipe for the Refund request.
            /// </summary>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            ///  Gets or sets the card number associated with the Refund request.
            /// </summary>
            internal string CardNumber { get; set; }

            /// <summary>
            ///  Gets or sets the Track 1 attribute associated with the Refund request.
            /// </summary>
            internal string Track1 { get; set; }

            /// <summary>
            ///  Gets or sets the Track 2 attribute associated with the Refund request.
            /// </summary>
            internal string Track2 { get; set; }

            /// <summary>
            ///  Gets or sets the card expiration year associated with the Refund request.
            /// </summary>
            internal decimal? ExpirationYear { get; set; }

            /// <summary>
            ///  Gets or sets the card expiration month associated with the Refund request.
            /// </summary>
            internal decimal? ExpirationMonth { get; set; }

            /// <summary>
            ///  Gets or sets the card token string associated with the Refund request.
            /// </summary>
            internal string CardToken { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Refund request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            /// Gets or sets the encrypted pin value associated with the Refund request.
            /// </summary>
            internal string EncryptedPin { get; set; }

            /// <summary>
            /// Gets or sets any additional security data associated with the Refund request.
            /// </summary>
            internal string AdditionalSecurityData { get; set; }

            /// <summary>
            /// Gets or sets the card verification value associated with the Refund request.
            /// </summary>
            internal string CardVerificationValue { get; set; }

            /// <summary>
            /// Gets or sets the card holder's name value associated with the Refund request.
            /// </summary>
            internal string Name { get; set; }

            /// <summary>
            /// Gets or sets the card holder's street address value associated with the Refund request.
            /// </summary>
            internal string StreetAddress { get; set; }

            /// <summary>
            /// Gets or sets the card holder's street address 2 value associated with the Refund request.
            /// </summary>
            internal string StreetAddress2 { get; set; }

            /// <summary>
            /// Gets or sets the card holder's city value associated with the Refund request.
            /// </summary>
            internal string City { get; set; }

            /// <summary>
            /// Gets or sets the card holder's state value associated with the Refund request.
            /// </summary>
            internal string State { get; set; }

            /// <summary>
            /// Gets or sets the card holder's postal code value associated with the Refund request.
            /// </summary>
            internal string PostalCode { get; set; }

            /// <summary>
            /// Gets or sets the card holder's country value associated with the Refund request.
            /// </summary>
            internal string Country { get; set; }

            /// <summary>
            /// Gets or sets the card holder's phone number value associated with the Refund request.
            /// </summary>
            internal string Phone { get; set; }

            /// <summary>
            /// Gets or sets the account type associated with the Refund request.
            /// </summary>
            internal string AccountType { get; set; }

            /// <summary>
            /// Gets or sets the card's unique ID associated with the Refund request.
            /// </summary>
            internal string UniqueCardId { get; set; }

            /// <summary>
            /// Gets or sets the amount associated with the Refund request.
            /// </summary>
            internal decimal? Amount { get; set; }

            /// <summary>
            /// Gets or sets the currency code associated with the Refund request.
            /// </summary>
            internal string CurrencyCode { get; set; }

            /// <summary>
            /// Gets or sets the supported card tokenization associated with the Refund request.
            /// </summary>
            internal bool? SupportCardTokenization { get; set; }

            /// <summary>
            /// Gets or sets the purchase level associated with the Refund request.
            /// </summary>
            internal string PurchaseLevel { get; set; }

            /// <summary>
            /// Gets or sets the level 2 data associated with the Refund request.
            /// </summary>
            internal Level2Data Level2Data { get; set; }

            /// <summary>
            /// Gets or sets the level 2 data associated with the Refund request.
            /// </summary>
            internal IEnumerable<Level3Data> Level3Data { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether there is any linked refund associated with the Refund request.
            /// </summary>
            internal bool IsLinkedRefund { get; set; }

            /// <summary>
            /// Gets or sets capture result associated with the Refund request.
            /// </summary>
            internal string CaptureResult { get; set; }

            /// <summary>
            /// Gets or sets capture approval code associated with the Refund request.
            /// </summary>
            internal string CaptureApprovalCode { get; set; }

            /// <summary>
            /// Gets or sets capture response associated with the Refund request.
            /// </summary>
            internal string CaptureResponseCode { get; set; }

            /// <summary>
            /// Gets or sets capture provider message associated with the Refund request.
            /// </summary>
            internal string CaptureProviderMessage { get; set; }

            /// <summary>
            /// Gets or sets capture provider transaction ID associated with the Refund request.
            /// </summary>
            internal string CaptureProviderTransactionId { get; set; }

            /// <summary>
            /// Gets or sets capture transaction type associated with the Refund request.
            /// </summary>
            internal string CaptureTransactionType { get; set; }

            /// <summary>
            /// Gets or sets capture transaction date and time associated with the Refund request.
            /// </summary>
            internal DateTime? CaptureTransactionDateTime { get; set; }

            /// <summary>
            /// Gets or sets the payment tracking identifier.
            /// </summary>
            /// <value>
            /// The payment tracking identifier.
            /// </value>
            internal string PaymentTrackingId { get; set; }

            /// <summary>
            /// Will convert the original <see cref="Request"/> into a <see cref="RefundRequest"/> object while performing necessary validation.
            /// Method will throw <see cref="SampleException"/> if validation fails during processing of <see cref="Request"/> object passed.
            /// </summary>
            /// <param name="request">Original request needing to be converted to <see cref="RefundRequest"/>.</param>
            /// <returns><see cref="RefundRequest"/> object representing the passed-in <see cref="Request"/> object.</returns>
            internal static RefundRequest ConvertFrom(Request request)
            {
                var refundRequest = new RefundRequest();
                var errors = new List<PaymentError>();
                refundRequest.ReadBaseProperties(request, errors);

                // Check capture response
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                PaymentProperty captureResponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, GenericNamespace.CaptureResponse, CaptureResponseProperties.Properties);

                // Read card data
                if (captureResponsePropertyList != null)
                {
                    refundRequest.IsLinkedRefund = refundRequest.Amount != decimal.Parse(TestData.RefundNotSupported);

                    // Linked refund, get card data from CaptureResponse
                    Hashtable captureHashtable = PaymentProperty.ConvertToHashtable(captureResponsePropertyList.PropertyList);
                    refundRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.CardType,
                        errors,
                        ErrorCode.InvalidRequest);
                    refundRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.CardToken);
                    refundRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.Last4Digits);
                    refundRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.UniqueCardId);

                    // Get other capture transaction data
                    refundRequest.CaptureTransactionType = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.TransactionType,
                        errors,
                        ErrorCode.InvalidRequest);
                    if (refundRequest.CaptureTransactionType != null
                        && !TransactionType.Capture.ToString().Equals(refundRequest.CaptureTransactionType, StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidTransaction, "Refund does not support this type of transaction"));
                    }

                    refundRequest.CaptureApprovalCode = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.ApprovalCode);
                    refundRequest.CaptureProviderMessage = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.ProviderMessage);
                    refundRequest.CaptureProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.ProviderTransactionId,
                        errors,
                        ErrorCode.InvalidRequest);
                    refundRequest.CaptureResponseCode = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.ResponseCode);
                    refundRequest.CaptureResult = PaymentUtilities.GetPropertyStringValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.CaptureResult,
                        errors,
                        ErrorCode.InvalidRequest);
                    refundRequest.CaptureTransactionDateTime = PaymentUtilities.GetPropertyDateTimeValue(
                        captureHashtable,
                        GenericNamespace.CaptureResponse,
                        CaptureResponseProperties.TransactionDateTime);
                }
                else
                {
                    // Not a linked refund, get card data from PaymentCard
                    refundRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);
                    if (refundRequest.CardType != null
                        && !PaymentUtilities.ValidateCardType(refundRequest.SupportedTenderTypes, refundRequest.CardType))
                    {
                        errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format("Card type is not supported: {0}.", refundRequest.CardType)));
                    }

                    refundRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.IsSwipe);
                    if (refundRequest.IsSwipe ?? false)
                    {
                        refundRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Track1);
                        refundRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Track2);

                        refundRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(refundRequest.Track1);
                        if (refundRequest.CardNumber == null)
                        {
                            refundRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(refundRequest.Track2);
                        }

                        if (refundRequest.CardNumber == null)
                        {
                            errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                        }

                        decimal expirationYear, expirationMonth;
                        HelperUtilities.ParseTrackDataForExpirationDate(refundRequest.Track1 ?? string.Empty, refundRequest.Track2 ?? string.Empty, out expirationYear, out expirationMonth);
                        refundRequest.ExpirationYear = expirationYear;
                        refundRequest.ExpirationMonth = expirationMonth;
                    }
                    else
                    {
                        refundRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardToken);

                        if (refundRequest.CardToken == null)
                        {
                            refundRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                                hashtable,
                                GenericNamespace.PaymentCard,
                                PaymentCardProperties.CardNumber,
                                errors,
                                ErrorCode.InvalidCardNumber);
                        }
                        else
                        {
                            refundRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                                hashtable,
                                GenericNamespace.PaymentCard,
                                PaymentCardProperties.Last4Digits);
                        }

                        if (!string.Equals(refundRequest.CardType, Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            refundRequest.ExpirationYear = PaymentUtilities.GetPropertyDecimalValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.ExpirationYear,
                            errors,
                            ErrorCode.InvalidExpirationDate);
                            refundRequest.ExpirationMonth = PaymentUtilities.GetPropertyDecimalValue(
                                hashtable,
                                GenericNamespace.PaymentCard,
                                PaymentCardProperties.ExpirationMonth,
                                errors,
                                ErrorCode.InvalidExpirationDate);
                        }
                    }

                    if (refundRequest.CardNumber != null
                        && !refundRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase)
                        && !HelperUtilities.ValidateBankCardNumber(refundRequest.CardNumber))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidCardNumber, "Invalid card number."));
                    }

                    if (refundRequest.ExpirationYear != null && refundRequest.ExpirationYear.HasValue
                        && refundRequest.ExpirationMonth != null && refundRequest.ExpirationMonth.HasValue
                        && refundRequest.ExpirationYear >= 0M
                        && refundRequest.ExpirationMonth >= 0M
                        && !PaymentUtilities.ValidateExpirationDate(refundRequest.ExpirationYear.Value, refundRequest.ExpirationMonth.Value))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidExpirationDate, "Invalid expiration date."));
                    }

                    if (Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.Debit.ToString().Equals(refundRequest.CardType, StringComparison.OrdinalIgnoreCase))
                    {
                        refundRequest.EncryptedPin = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.EncryptedPin,
                            errors,
                            ErrorCode.CannotVerifyPin);
                        refundRequest.AdditionalSecurityData = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.AdditionalSecurityData);
                    }

                    refundRequest.CardVerificationValue = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardVerificationValue);
                    refundRequest.Name = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Name);
                    refundRequest.StreetAddress = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.StreetAddress);
                    refundRequest.StreetAddress2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.StreetAddress2);
                    refundRequest.City = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.City);
                    refundRequest.State = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.State);
                    refundRequest.PostalCode = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.PostalCode);
                    refundRequest.Country = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Country);
                    refundRequest.Phone = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Phone);
                    refundRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.AccountType);
                    refundRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.UniqueCardId);
                }

                // Read transaction data
                refundRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Amount,
                    errors,
                    ErrorCode.InvalidAmount);
                refundRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.CurrencyCode,
                    errors,
                    ErrorCode.InvalidRequest);
                refundRequest.PaymentTrackingId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PaymentTrackingId);

                if (refundRequest.CurrencyCode != null
                    && !PaymentUtilities.ValidateCurrencyCode(refundRequest.SupportedCurrencies, refundRequest.CurrencyCode))
                {
                    errors.Add(new PaymentError(ErrorCode.UnsupportedCurrency, string.Format("Currency code is not supported: {0}.", refundRequest.CurrencyCode)));
                }

                refundRequest.SupportCardTokenization = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.SupportCardTokenization);

                refundRequest.PurchaseLevel = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PurchaseLevel);

                refundRequest.Level2Data = PaymentUtilities.GetLevel2Data(hashtable);
                refundRequest.Level3Data = PaymentUtilities.GetLevel3Data(hashtable);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return refundRequest;
            }
        }
    }
}
