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
        using System.Text;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Contains attributes associated with a Void request.
        /// Also contains a helper method to convert <see cref="Request"/> object into a <see cref="VoidRequest"/> object.
        /// </summary>
        internal class VoidRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VoidRequest"/> class.
            /// </summary>
            internal VoidRequest()
                : base()
            {
            }

            /// <summary>
            ///  Gets or sets the card number associated with the Void request.
            /// </summary>
            internal string CardNumber { get; set; }

            /// <summary>
            /// Gets or sets the card type associated with the Void request.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            /// Gets or sets whether the request was a swipe for the Void request.
            /// </summary>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            ///  Gets or sets the card token string associated with the Void request.
            /// </summary>
            internal string CardToken { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Void request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            ///  Gets or sets the account type associated with the Void request.
            /// </summary>
            internal string AccountType { get; set; }

            /// <summary>
            ///  Gets or sets the unique card ID associated with the Void request.
            /// </summary>
            internal string UniqueCardId { get; set; }

            /// <summary>
            ///  Gets or sets the voice authorization code associated with the Void request.
            /// </summary>
            internal string VoiceAuthorizationCode { get; set; }

            /// <summary>
            ///  Gets or sets the currency code associated with the Void request.
            /// </summary>
            internal string CurrencyCode { get; set; }

            /// <summary>
            ///  Gets or sets the authorization provider transaction ID associated with the Void request.
            /// </summary>
            internal string AuthorizationProviderTransactionId { get; set; }

            /// <summary>
            ///  Gets or sets the authorization approval code associated with the Void request.
            /// </summary>
            internal string AuthorizationApprovalCode { get; set; }

            /// <summary>
            ///  Gets or sets the authorization response code associated with the Void request.
            /// </summary>
            internal string AuthorizationResponseCode { get; set; }

            /// <summary>
            ///  Gets or sets the authorization approved amount associated with the Void request.
            /// </summary>
            internal decimal? AuthorizationApprovedAmount { get; set; }

            /// <summary>
            ///  Gets or sets the authorization cash back amount associated with the Void request.
            /// </summary>
            internal decimal? AuthorizationCashbackAmount { get; set; }

            /// <summary>
            ///  Gets or sets the authorization result associated with the Void request.
            /// </summary>
            internal string AuthorizationResult { get; set; }

            /// <summary>
            ///  Gets or sets the authorization provider message associated with the Void request.
            /// </summary>
            internal string AuthorizationProviderMessage { get; set; }

            /// <summary>
            ///  Gets or sets the authorization transaction date and time associated with the Void request.
            /// </summary>
            internal DateTime? AuthorizationTransactionDateTime { get; set; }

            /// <summary>
            ///  Gets or sets the authorization transaction type associated with the Void request.
            /// </summary>
            internal string AuthorizationTransactionType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the issued gift card is requested to be void.
            /// </summary>
            internal bool IsIssuedGiftCard { get; set; }

            /// <summary>
            /// Gets or sets the payment tracking identifier.
            /// </summary>
            /// <value>
            /// The payment tracking identifier.
            /// </value>
            internal string PaymentTrackingId { get; set; }

            /// <summary>
            /// Will convert the original <see cref="Request"/> into a <see cref="VoidRequest"/> object while performing necessary validation.
            /// Method will throw <see cref="SampleException"/> if validation fails during processing of <see cref="Request"/> object passed.
            /// </summary>
            /// <param name="request">Original request needing to be converted to <see cref="VoidRequest"/>.</param>
            /// <returns><see cref="VoidRequest"/> object representing the passed-in <see cref="Request"/> object.</returns>
            internal static VoidRequest ConvertFrom(Request request)
            {
                var voidRequest = new VoidRequest();
                var errors = new List<PaymentError>();
                var giftCardNamespace = GenericNamespace.ActivateGiftCardResponse;
                voidRequest.ReadBaseProperties(request, errors);

                // Check authorization response
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                PaymentProperty authorizationResponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, GenericNamespace.AuthorizationResponse, AuthorizationResponseProperties.Properties);
                PaymentProperty giftCardReponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, giftCardNamespace, AuthorizationResponseProperties.Properties);

                if (giftCardReponsePropertyList == null)
                {
                    // Might be LoadGiftCardResponse
                    giftCardNamespace = GenericNamespace.LoadGiftCardResponse;
                    giftCardReponsePropertyList = PaymentProperty.GetPropertyFromHashtable(hashtable, giftCardNamespace, AuthorizationResponseProperties.Properties);

                    if (giftCardReponsePropertyList == null && authorizationResponsePropertyList == null)
                    {
                        // Client might not forward activation/AddTo response
                        giftCardNamespace = GenericNamespace.PaymentCard;
                        giftCardReponsePropertyList = new PaymentProperty();
                    }
                }

                Hashtable authorizationHashtable = null;
                bool isGiftCardTransaction = false;

                // Authorization Response can be missing if it is a Gift Card transation. Otherwise, throw an exception.
                if (authorizationResponsePropertyList == null && giftCardReponsePropertyList == null)
                {
                    voidRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardType);

                    isGiftCardTransaction = voidRequest.CardType != null && voidRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);
                    if (!isGiftCardTransaction)
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Authorization response is missing."));
                        throw new SampleException(errors);
                    }
                }
                else if (giftCardReponsePropertyList != null)
                {
                    voidRequest.IsIssuedGiftCard = true;

                    voidRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        giftCardNamespace,
                        PaymentCardProperties.CardType);

                    voidRequest.AuthorizationApprovedAmount = PaymentUtilities.GetPropertyDecimalValue(
                        hashtable,
                        GenericNamespace.TransactionData,
                        TransactionDataProperties.Amount,
                        errors,
                        ErrorCode.InvalidRequest);

                    isGiftCardTransaction = voidRequest.CardType != null && voidRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    authorizationHashtable = PaymentProperty.ConvertToHashtable(authorizationResponsePropertyList.PropertyList);

                    // Read card data
                    voidRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.CardType,
                        errors,
                        ErrorCode.InvalidRequest);

                    isGiftCardTransaction = voidRequest.CardType != null && voidRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase);

                    voidRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.IsSwiped);
                    voidRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                            authorizationHashtable,
                            GenericNamespace.AuthorizationResponse,
                            AuthorizationResponseProperties.CardToken);
                    voidRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.Last4Digits);
                    voidRequest.AccountType = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.AccountType);
                    voidRequest.UniqueCardId = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.UniqueCardId);
                    voidRequest.VoiceAuthorizationCode = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.VoiceAuthorizationCode);

                    // Read authorization data
                    voidRequest.AuthorizationTransactionType = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.TransactionType,
                        errors,
                        ErrorCode.InvalidRequest);
                    if (voidRequest.AuthorizationTransactionType != null
                        && !TransactionType.Authorize.ToString().Equals(voidRequest.AuthorizationTransactionType, StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidTransaction, "Void does not support this type of transaction"));
                    }

                    voidRequest.AuthorizationApprovalCode = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.ApprovalCode);
                    voidRequest.AuthorizationApprovedAmount = PaymentUtilities.GetPropertyDecimalValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.ApprovedAmount,
                        errors,
                        ErrorCode.InvalidRequest);
                    voidRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.CurrencyCode,
                        errors,
                        ErrorCode.InvalidRequest);
                    voidRequest.AuthorizationCashbackAmount = PaymentUtilities.GetPropertyDecimalValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.CashBackAmount);
                    voidRequest.AuthorizationProviderMessage = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.ProviderMessage);
                    voidRequest.AuthorizationProviderTransactionId = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.ProviderTransactionId,
                        errors,
                        ErrorCode.InvalidRequest);
                    voidRequest.AuthorizationResponseCode = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.ResponseCode);
                    voidRequest.AuthorizationResult = PaymentUtilities.GetPropertyStringValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.AuthorizationResult,
                        errors,
                        ErrorCode.InvalidRequest);
                    voidRequest.AuthorizationTransactionDateTime = PaymentUtilities.GetPropertyDateTimeValue(
                        authorizationHashtable,
                        GenericNamespace.AuthorizationResponse,
                        AuthorizationResponseProperties.TransactionDateTime);
                }

                // If it is a Gift Card type transaction, attempt to set the Card Number (if not already set) using the PaymentCard properties.
                // If Card Number cannot be set using the PaymentCard properties, attempt to set it using the Card Token.
                if (isGiftCardTransaction)
                {
                    voidRequest.CardNumber = voidRequest.CardNumber ?? PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardNumber);

                    if (voidRequest.IsIssuedGiftCard)
                    {
                        voidRequest.CardNumber = voidRequest.CardNumber ?? PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        giftCardNamespace,
                        ActivateGiftCardResponseProperties.GiftCardNumber);
                    }

                    // If Card Number is not available through PaymentCard.CardNumber property, set it through the CardToken instead.
                    if (string.IsNullOrEmpty(voidRequest.CardNumber))
                    {
                        voidRequest.CardToken = voidRequest.CardToken ?? PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardToken);

                        if (string.IsNullOrEmpty(voidRequest.CardToken))
                        {
                            errors.Add(new PaymentError(ErrorCode.InvalidRequest, "Card Number and Card Token missing."));
                            throw new SampleException(errors);
                        }
                        else
                        {
                            UTF8Encoding encoder = new UTF8Encoding();
                            byte[] cardNumberBytes = Convert.FromBase64String(voidRequest.CardToken);
                            voidRequest.CardNumber = encoder.GetString(cardNumberBytes, 0, cardNumberBytes.Length);
                        }
                    }
                }

                voidRequest.PaymentTrackingId = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.PaymentTrackingId);

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return voidRequest;
            }
        }
    }
}
