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
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        /// <summary>
        /// Merchant account properties of the payment connector.
        /// </summary>
        public static class SampleMerchantAccountProperty
        {
            /// <summary>
            /// ProviderId property.
            /// </summary>
            public const string ProviderId = "ProviderId";

            /// <summary>
            /// Environment property.
            /// </summary>
            public const string Environment = "Environment";

            /// <summary>
            /// TestString property.
            /// </summary>
            public const string TestString = "TestString";

            /// <summary>
            /// TestDecimal property.
            /// </summary>
            public const string TestDecimal = "TestDecimal";

            /// <summary>
            /// TestDate property.
            /// </summary>
            public const string TestDate = "TestDate";

            private static ArrayList arrayList = new ArrayList()
            {
                MerchantAccountProperties.AssemblyName,
                MerchantAccountProperties.ServiceAccountId,
                MerchantAccountProperties.MerchantId,
                ProviderId,
                Environment,
                TestString,
                TestDecimal,
                TestDate,
                MerchantAccountProperties.SupportedCurrencies,
                MerchantAccountProperties.SupportedTenderTypes,
            };

            /// <summary>
            /// Gets the merchant account properties array.
            /// </summary>
            public static ArrayList ArrayList
            {
                get
                {
                    return arrayList;
                }
            }
        }
    }
}
