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
        /// Request to capture payment.
        /// </summary>
        /// <seealso cref="Microsoft.Dynamics.Retail.SampleConnector.Portable.RequestBase" />
        internal class CaptureRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CaptureRequest"/> class.
            /// </summary>
            internal CaptureRequest()
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
            /// Gets or sets the is swipe.
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
            /// Gets or sets the voice authorization code.
            /// </summary>
            /// <value>
            /// The voice authorization code.
            /// </value>
            internal string VoiceAuthorizationCode { get; set; }

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
            /// Gets or sets the purchase level.
            /// </summary>
            /// <value>
            /// The purchase level.
            /// </value>
            internal string PurchaseLevel { get; set; }

            /// <summary>
            /// Gets or sets the level2 data.
            /// </summary>
            /// <value>
            /// The level2 data.
            /// </value>
            internal Level2Data Level2Data { get; set; }

            /// <summary>
            /// Gets or sets the level3 data.
            /// </summary>
            /// <value>
            /// The level3 data.
            /// </value>
            internal IEnumerable<Level3Data> Level3Data { get; set; }

            /// <summary>
            /// Gets or sets the authorization provider transaction identifier.
            /// </summary>
            /// <value>
            /// The authorization provider transaction identifier.
            /// </value>
            internal string AuthorizationProviderTransactionId { get; set; }

            /// <summary>
            /// Gets or sets the authorization approval code.
            /// </summary>
            /// <value>
            /// The authorization approval code.
            /// </value>
            internal string AuthorizationApprovalCode { get; set; }

            /// <summary>
            /// Gets or sets the authorization response code.
            /// </summary>
            /// <value>
            /// The authorization response code.
            /// </value>
            internal string AuthorizationResponseCode { get; set; }

            /// <summary>
            /// Gets or sets the authorization approved amount.
            /// </summary>
            /// <value>
            /// The authorization approved amount.
            /// </value>
            internal decimal? AuthorizationApprovedAmount { get; set; }

            /// <summary>
            /// Gets or sets the authorization cashback amount.
            /// </summary>
            /// <value>
            /// The authorization cashback amount.
            /// </value>
            internal decimal? AuthorizationCashbackAmount { get; set; }

            /// <summary>
            /// Gets or sets the authorization result.
            /// </summary>
            /// <value>
            /// The authorization result.
            /// </value>
            internal string AuthorizationResult { get; set; }

            /// <summary>
            /// Gets or sets the authorization provider message.
            /// </summary>
            /// <value>
            /// The authorization provider message.
            /// </value>
            internal string AuthorizationProviderMessage { get; set; }

            /// <summary>
            /// Gets or sets the authorization transaction date time.
            /// </summary>
            /// <value>
            /// The authorization transaction date time.
            /// </value>
            internal DateTime? AuthorizationTransactionDateTime { get; set; }

            /// <summary>
            /// Gets or sets the type of the authorization transaction.
            /// </summary>
            /// <value>
            /// The type of the authorization transaction.
            /// </value>
            internal string AuthorizationTransactionType { get; set; }

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
            /// <returns>An instance of <see cref="CaptureRequest"/>.</returns>
            internal static CaptureRequest ConvertFrom(Request request)
            {
                var captureRequest = new CaptureRequest();
                var errors = new List<PaymentError>();
                captureRequest.ReadBaseProperties(request, errors);

                // Check authorization response
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                PaymentProperty authorizationResponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Properties);
                Hashtable authorizationHashtable = null;
                if (authorizationResponsePropertyList == null)
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Authorization response is missing."));
                    throw new SampleException(errors);
                }
                else
                {
                    authorizationHashtable = PaymentProperty.ConvertToHashtable(authorizationResponsePropertyList.PropertyList);
                }

                // Read card data
                captureRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);

                captureRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.IsSwiped);
                if (captureRequest.IsSwipe ?? false)
                {
                    captureRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track1);
                    captureRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track2);
                    captureRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardNumber);

                    if (string.IsNullOrEmpty(captureRequest.CardNumber))
                    {
                        captureRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(captureRequest.Track1);
                        if (captureRequest.CardNumber == null)
                        {
                            captureRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(captureRequest.Track2);
                        }
                    }

                    if (string.IsNullOrEmpty(captureRequest.CardNumber))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                    }
                }
                else
                {
                    captureRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.CardToken);
                    if (captureRequest.CardToken == null)
                    {
                        captureRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardToken);
                        if (captureRequest.CardToken == null)
                        {
                            captureRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                                hashtable,
                                GenericNamespace.PaymentCard,
                                PaymentCardProperties.CardNumber,
                                errors,
                                ErrorCode.InvalidCardNumber);
                        }
                    }
                    else
                    {
                        if (captureRequest.CardType == "GiftCard" && captureRequest.CardNumber == null)
                        {
                            captureRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                                hashtable,
                                GenericNamespace.PaymentCard,
                                PaymentCardProperties.CardNumber,
                                errors,
                                ErrorCode.InvalidCardNumber);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(captureRequest.CardNumber)
                    && string.IsNullOrWhiteSpace(captureRequest.CardToken))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidRequest, string.Format("Neither card number nor card token is provided.")));
                }

                captureRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.Last4Digits);
                captureRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.AccountType);
                captureRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.UniqueCardId);
                captureRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.VoiceAuthorizationCode);

                // Read transaction data
                captureRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Amount,
                    errors,
                    ErrorCode.InvalidAmount);
                captureRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.CurrencyCode,
                    errors,
                    ErrorCode.InvalidRequest);
                captureRequest.PurchaseLevel = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PurchaseLevel);
                captureRequest.PaymentTrackingId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PaymentTrackingId);

                captureRequest.Level2Data = PaymentUtilities.GetLevel2Data(hashtable);
                captureRequest.Level3Data = PaymentUtilities.GetLevel3Data(hashtable);

                // Read authorization data
                captureRequest.AuthorizationTransactionType = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.TransactionType,
                    errors,
                    ErrorCode.InvalidRequest);
                if (captureRequest.AuthorizationTransactionType != null
                    && !TransactionType.Authorize.ToString().Equals(captureRequest.AuthorizationTransactionType, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new PaymentError(ErrorCode.InvalidTransaction, "Capture does not support this type of transaction"));
                }

                captureRequest.AuthorizationApprovalCode = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.ApprovalCode);
                captureRequest.AuthorizationApprovedAmount = PaymentUtilities.GetPropertyDecimalValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.ApprovedAmount,
                    errors,
                    ErrorCode.InvalidRequest);
                captureRequest.AuthorizationCashbackAmount = PaymentUtilities.GetPropertyDecimalValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.CashBackAmount);
                captureRequest.AuthorizationProviderMessage = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.ProviderMessage);
                captureRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.ProviderTransactionId,
                    errors,
                    ErrorCode.InvalidRequest);
                captureRequest.AuthorizationResponseCode = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.ResponseCode);
                captureRequest.AuthorizationResult = PaymentUtilities.GetPropertyStringValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.AuthorizationResult,
                    errors,
                    ErrorCode.InvalidRequest);
                captureRequest.AuthorizationTransactionDateTime = PaymentUtilities.GetPropertyDateTimeValue(
                    authorizationHashtable,
                    GenericNamespace.AuthorizationResponse,
                    AuthorizationResponseProperties.TransactionDateTime);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return captureRequest;
            }
        }
    }
}
