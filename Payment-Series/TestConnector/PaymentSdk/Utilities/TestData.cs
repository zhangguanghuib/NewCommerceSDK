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

        /// <summary>
        /// Predefined test data for the payment connector.
        /// </summary>
        public static class TestData
        {
            // AVS test values

            /// <summary>
            /// The AVS returned billing name.
            /// </summary>
            public const string AVSReturnedBillingName = "0.01";

            /// <summary>
            /// The AVS returned billing address.
            /// </summary>
            public const string AVSReturnedBillingAddress = "0.03";

            /// <summary>
            /// The AVS returned billing and postal code.
            /// </summary>
            public const string AVSReturnedBillingAndPostalCode = "0.05";

            /// <summary>
            /// The AVS returned billing postal code.
            /// </summary>
            public const string AVSReturnedBillingPostalCode = "0.07";

            /// <summary>
            /// The AVS returned none.
            /// </summary>
            public const string AVSReturnedNone = "0.09";

            /// <summary>
            /// The AVS not returned.
            /// </summary>
            public const string AVSNotReturned = "0.11";

            /// <summary>
            /// The AVS none.
            /// </summary>
            public const string AVSNone = "0.13";

            /// <summary>
            /// The AVS verification not supported.
            /// </summary>
            public const string AVSVerificationNotSupported = "0.15";

            /// <summary>
            /// The AVS system unavailable.
            /// </summary>
            public const string AVSSystemUnavailable = "0.17";

            // CVV2 test values

            /// <summary>
            /// The cv v2 failure.
            /// </summary>
            public const string CVV2Failure = "001";

            /// <summary>
            /// The cv v2 issuer not registered.
            /// </summary>
            public const string CVV2IssuerNotRegistered = "003";

            /// <summary>
            /// The cv v2 not processed.
            /// </summary>
            public const string CVV2NotProcessed = "005";

            /// <summary>
            /// The cv v2 unknown.
            /// </summary>
            public const string CVV2Unknown = "007";

            // Authorization test values

            /// <summary>
            /// The authorization declined.
            /// </summary>
            public const string AuthorizationDeclined = "1.12";

            /// <summary>
            /// The authorization none.
            /// </summary>
            public const string AuthorizationNone = "1.14";

            /// <summary>
            /// The authorization referral.
            /// </summary>
            public const string AuthorizationReferral = "1.16";

            /// <summary>
            /// The authorization partial authorization.
            /// </summary>
            public const string AuthorizationPartialAuthorization = "1.18";

            /// <summary>
            /// The authorization immediate capture required.
            /// </summary>
            public const string AuthorizationImmediateCaptureRequired = "1.20";

            /// <summary>
            /// The authorization success with available balance returned.
            /// </summary>
            public const string AuthorizationAvailableBalance = "1.22";

            /// <summary>
            /// The authorization failed with timed out.
            /// </summary>
            public const string AuthorizationTimedOut = "1.24";

            // Capture test values

            /// <summary>
            /// The capture failure.
            /// </summary>
            public const string CaptureFailure = "2.12";

            /// <summary>
            /// The capture queued for batch.
            /// </summary>
            public const string CaptureQueuedForBatch = "2.14";

            /// <summary>
            /// The capture none.
            /// </summary>
            public const string CaptureNone = "2.16";

            /// <summary>
            /// The capture failure and void failure.
            /// </summary>
            public const string CaptureFailureVoidFailure = "2.18";

            /// <summary>
            /// The capture failure due to invalid voice authorization code.
            /// </summary>
            public const string CaptureFailureInvalidVoiceAuthCode = "2.20";

            /// <summary>
            /// The capture failure due to multiple capture.
            /// </summary>
            public const string CaptureFailureMultipleCapture = "2.22";

            /// <summary>
            /// The capture failed with timed out.
            /// </summary>
            public const string CaptureTimedOut = "2.24";

            // Refund test values

            /// <summary>
            /// The refund failure.
            /// </summary>
            public const string RefundFailure = "3.12";

            /// <summary>
            /// The refund queue for batch.
            /// </summary>
            public const string RefundQueueForBatch = "3.14";

            /// <summary>
            /// The refund none.
            /// </summary>
            public const string RefundNone = "3.16";

            /// <summary>
            /// The refund failed with timed out.
            /// </summary>
            public const string RefundTimedOut = "3.18";

            /// <summary>
            /// The refund type is not supported.
            /// </summary>
            public const string RefundNotSupported = "3.20";

            // Void test values

            /// <summary>
            /// The void failure.
            /// </summary>
            public const string VoidFailure = "4.12";

            /// <summary>
            /// The void none.
            /// </summary>
            public const string VoidNone = "4.14";

            /// <summary>
            /// The void failure due to multiple void.
            /// </summary>
            public const string VoidFailureAuthorizationVoided = "4.16";

            /// <summary>
            /// The authorization partial authorization returns void failure amount.
            /// </summary>
            public const string AuthorizationPartialAuthorizationReturnVoidFailure = "4.18";

            /// <summary>
            /// The void failed with timed out.
            /// </summary>
            public const string VoidTimedOut = "4.20";

            /// <summary>
            /// The test string.
            /// </summary>
            public const string TestString = "Test string 1234567890 1234567890 End.";

            /// <summary>
            /// The test decimal.
            /// </summary>
            public const decimal TestDecimal = 12345.67M;

            /// <summary>
            /// The supported currencies.
            /// </summary>
            public const string SupportedCurrencies = "AUD;BRL;CAD;CHF;CNY;CZK;DKK;EUR;GBP;HKD;HUF;INR;JPY;KPW;KRW;MXN;NOK;NZD;PLN;SEK;SGD;TWD;USD;ZAR";

            /// <summary>
            /// The supported tender types.
            /// </summary>
            public const string SupportedTenderTypes = "Visa;MasterCard;Amex;Discover;Debit";

            /// <summary>
            /// The merchant identifier.
            /// </summary>
            public static readonly Guid MerchantId = new Guid("136E9C86-31A1-4177-B2B7-A027C63EDBE0");

            /// <summary>
            /// The provider identifier.
            /// </summary>
            public static readonly Guid ProviderId = new Guid("467079B4-1601-4F79-83C9-F569872EB94E");

            /// <summary>
            /// The test date.
            /// </summary>
            public static readonly DateTime TestDate = new DateTime(2011, 9, 22, 11, 3, 0);

            /// <summary>
            /// The test payment methods.
            /// </summary>
            public static readonly List<PaymentMethod> SupportedPaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod(name: "amex", isLinkedRefundSupported: true, isUnlinkedRefundSupported: true),
                new PaymentMethod(name: "discover", isLinkedRefundSupported: true, isUnlinkedRefundSupported: true),
                new PaymentMethod(name: "mc", isLinkedRefundSupported: true, isUnlinkedRefundSupported: true),
                new PaymentMethod(name: "visa", isLinkedRefundSupported: true, isUnlinkedRefundSupported: true),
            };
        }
    }
}
