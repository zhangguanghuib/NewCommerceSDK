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
        using System.Security.Cryptography;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivateGiftCardRequest" /> class.
        /// Contains attributes associated with a Activate Gift Card request.
        /// Also contains a helper method to convert <see cref="Request"/> object into a <see cref="ActivateGiftCardRequest"/> object.
        /// </summary>
        internal class ActivateGiftCardRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActivateGiftCardRequest"/> class.
            /// </summary>
            internal ActivateGiftCardRequest()
                : base()
            {
            }

            /// <summary>
            ///  Gets or sets the card type associated with the Activate Gift Card request. It should be <see cref="CardType.GiftCard"/>.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            ///  Gets or sets whether the request was a swipe for the Activate Gift Card request.
            /// </summary>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            ///  Gets or sets the card number associated with the Activate Gift Card request.
            /// </summary>
            internal string CardNumber { get; set; }

            /// <summary>
            ///  Gets or sets the Track 1 data associated with the Activate Gift Card request.
            /// </summary>
            internal string Track1 { get; set; }

            /// <summary>
            ///  Gets or sets the Track 2 data associated with the Activate Gift Card request.
            /// </summary>
            internal string Track2 { get; set; }

            /// <summary>
            ///  Gets or sets the card token string associated with the Activate Gift Card request.
            /// </summary>
            internal string CardToken { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Activate Gift Card request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            ///  Gets or sets the amount associated with the Activate Gift Card request.
            /// </summary>
            internal decimal? Amount { get; set; }

            /// <summary>
            ///  Gets or sets the currency code associated with the Activate Gift Card request.
            /// </summary>
            internal string CurrencyCode { get; set; }

            /// <summary>
            ///  Gets or sets whether the request is to issue a new gift card for the Activate Gift Card request.
            /// </summary>
            internal bool? IsIssueGiftCard { get; set; }

            /// <summary>
            /// Will convert the original <see cref="Request"/> into a <see cref="ActivateGiftCardRequest"/> object while performing necessary validation.
            /// Method will throw <see cref="SampleException"/> if validation fails during processing of <see cref="Request"/> object passed.
            /// </summary>
            /// <param name="request">Original request needing to be converted to <see cref="ActivateGiftCardRequest"/>.</param>
            /// <returns><see cref="ActivateGiftCardRequest"/> object representing the passed-in <see cref="Request"/> object.</returns>
            internal static ActivateGiftCardRequest ConvertFrom(Request request)
            {
                var activateGiftCardRequest = new ActivateGiftCardRequest();
                var errors = new List<PaymentError>();
                activateGiftCardRequest.ReadBaseProperties(request, errors);

                // Read card data
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                activateGiftCardRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);

                // Ensure CardType==GiftCard
                if (activateGiftCardRequest.CardType != null && !activateGiftCardRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format("Card type is not supported: {0}.", activateGiftCardRequest.CardType)));
                }

                activateGiftCardRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.IsSwipe);

                if (activateGiftCardRequest.IsSwipe ?? false)
                {
                    activateGiftCardRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track1);
                    activateGiftCardRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track2);

                    activateGiftCardRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(activateGiftCardRequest.Track1);
                    if (activateGiftCardRequest.CardNumber == null)
                    {
                        activateGiftCardRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(activateGiftCardRequest.Track2);
                    }

                    if (activateGiftCardRequest.CardNumber == null)
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                    }
                }
                else
                {
                    activateGiftCardRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardToken);

                    if (activateGiftCardRequest.CardToken == null)
                    {
                        activateGiftCardRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardNumber);

                        if (string.IsNullOrEmpty(activateGiftCardRequest.CardNumber))
                        {
                            // Assume this is issuing new gift card
                            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
                            string giftCardNumber;

                            do
                            {
                                byte[] randomBytes = new byte[4];
                                provider.GetBytes(randomBytes);
                                string randomNumber = BitConverter.ToInt32(randomBytes, 0).ToString().PadLeft(8, '0');

                                giftCardNumber = "6" + randomNumber.Substring(randomNumber.Length - 8, 8);
                            }
                            while (SampleConnector.ActivatedGiftCards.ContainsKey(giftCardNumber));

                            activateGiftCardRequest.CardNumber = giftCardNumber;
                            activateGiftCardRequest.IsIssueGiftCard = true;
                        }
                    }
                    else
                    {
                        activateGiftCardRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Last4Digits);
                    }
                }

                // Read transaction data
                activateGiftCardRequest.Amount = PaymentUtilities.GetPropertyDecimalValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.Amount,
                    errors,
                    ErrorCode.InvalidAmount);
                activateGiftCardRequest.CurrencyCode = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.TransactionData,
                    TransactionDataProperties.CurrencyCode,
                    errors,
                    ErrorCode.InvalidRequest);

                if (activateGiftCardRequest.CurrencyCode != null
                    && !PaymentUtilities.ValidateCurrencyCode(activateGiftCardRequest.SupportedCurrencies, activateGiftCardRequest.CurrencyCode))
                {
                    errors.Add(new PaymentError(ErrorCode.UnsupportedCurrency, string.Format("Currency code is not supported: {0}.", activateGiftCardRequest.CurrencyCode)));
                }

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return activateGiftCardRequest;
            }
        }
    }
}
