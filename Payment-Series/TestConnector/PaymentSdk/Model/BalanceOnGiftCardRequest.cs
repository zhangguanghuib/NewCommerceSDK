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
        /// Contains attributes associated with a Balance on Gift Card request.
        /// Also contains a helper method to convert <see cref="Request"/>  object into a <see cref="BalanceOnGiftCardRequest"/> object.
        /// </summary>
        internal class BalanceOnGiftCardRequest : RequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BalanceOnGiftCardRequest"/> class.
            /// </summary>
            internal BalanceOnGiftCardRequest()
                : base()
            {
            }

            /// <summary>
            /// Gets or sets the card type associated with the Balance on Gift Card request.
            /// </summary>
            internal string CardType { get; set; }

            /// <summary>
            /// Gets or sets whether the request was a swipe for the Balance on Gift Card request.
            /// </summary>
            internal bool? IsSwipe { get; set; }

            /// <summary>
            ///  Gets or sets the card number associated with the Balance on Gift Card request.
            /// </summary>
            internal string CardNumber { get; set; }

            /// <summary>
            ///  Gets or sets the Track 1 data associated with the Balance on Gift Card request.
            /// </summary>
            internal string Track1 { get; set; }

            /// <summary>
            ///  Gets or sets the Track 2 data associated with the Balance on Gift Card request.
            /// </summary>
            internal string Track2 { get; set; }

            /// <summary>
            ///  Gets or sets the card token string associated with the Balance on Gift Card request.
            /// </summary>
            internal string CardToken { get; set; }

            /// <summary>
            ///  Gets or sets the last four digits of the card number associated with the Balance on Gift Card request.
            /// </summary>
            internal string Last4Digit { get; set; }

            /// <summary>
            /// Converts the original <see cref="Request"/> into a <see cref="BalanceOnGiftCardRequest"/> object while performing necessary validation.
            /// Method will throw <see cref="SampleException"/> if validation fails during processing of <see cref="Request"/> object passed.
            /// </summary>
            /// <param name="request">Original request needing to be converted to <see cref="BalanceOnGiftCardRequest"/>.</param>
            /// <returns><see cref="BalanceOnGiftCardRequest"/> object representing the passed-in <see cref="Request"/> object.</returns>
            internal static BalanceOnGiftCardRequest ConvertFrom(Request request)
            {
                BalanceOnGiftCardRequest bogcRequest = new BalanceOnGiftCardRequest();
                List<PaymentError> errors = new List<PaymentError>();
                bogcRequest.ReadBaseProperties(request, errors);

                /*
                 * READ CARD DATA
                 */
                Hashtable hashtable = PaymentProperty.ConvertToHashtable(request.Properties);
                bogcRequest.CardType = PaymentUtilities.GetPropertyStringValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.CardType,
                    errors,
                    ErrorCode.InvalidRequest);

                // Ensure CardType==GiftCard
                if (bogcRequest.CardType != null && !bogcRequest.CardType.Equals(Microsoft.Dynamics.Retail.PaymentSDK.Portable.CardType.GiftCard.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add(new PaymentError(ErrorCode.CardTypeNotSupported, string.Format("Card type is not supported: {0}.", bogcRequest.CardType)));
                }

                bogcRequest.IsSwipe = PaymentUtilities.GetPropertyBooleanValue(
                    hashtable,
                    GenericNamespace.PaymentCard,
                    PaymentCardProperties.IsSwipe);

                if (bogcRequest.IsSwipe ?? false)
                {
                    // Extract and assign values from original request.
                    bogcRequest.Track1 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track1);
                    bogcRequest.Track2 = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.Track2);

                    bogcRequest.CardNumber = PaymentUtilities.ParseTrack1ForCardNumber(bogcRequest.Track1);
                    if (bogcRequest.CardNumber == null)
                    {
                        bogcRequest.CardNumber = PaymentUtilities.ParseTrack2ForCardNumber(bogcRequest.Track2);
                    }

                    if (bogcRequest.CardNumber == null)
                    {
                        errors.Add(new PaymentError(ErrorCode.InvalidCardTrackData, "Invalid card track data."));
                    }
                }
                else
                {
                    bogcRequest.CardToken = PaymentUtilities.GetPropertyStringValue(
                        hashtable,
                        GenericNamespace.PaymentCard,
                        PaymentCardProperties.CardToken);

                    if (bogcRequest.CardToken == null)
                    {
                        bogcRequest.CardNumber = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.CardNumber,
                            errors,
                            ErrorCode.InvalidCardNumber);
                    }
                    else
                    {
                        bogcRequest.Last4Digit = PaymentUtilities.GetPropertyStringValue(
                            hashtable,
                            GenericNamespace.PaymentCard,
                            PaymentCardProperties.Last4Digits);
                    }
                }

                if (errors.Count > 0)
                {
                    throw new SampleException(errors);
                }

                return bogcRequest;
            }
        }
    }
}
