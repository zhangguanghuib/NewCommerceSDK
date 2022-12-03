namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using System;
        using System.Collections.Generic;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        internal abstract class RequestBase
        {
            internal RequestBase() : base()
            {

            }

            internal string Locale { get; set; }

            internal string AssemblyName { get; set; }

            internal string ServiceAccountId { get; set; }

            internal string MerchanId { get; set; }

            internal string ProviderId { get; set; }

            internal string Environment { get; set; }

            internal string TestString { get; set; }

            internal decimal? TestDecimal { get; set; }

            internal DateTime? TestDate { get; set; }

            internal string SupportedCurrencies { get; set; }

            internal string SupportedTenderTypes { get; set; }

            internal string IndustryType { get; set; }

            internal bool? IsTestMode { get; set; }

            protected void ReadBaseProperties(Request request, List<PaymentError> errors)
            {
                if(request == null)
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Request is null.");
                }

                if (string.IsNullOrWhiteSpace(request.Locale))
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Locale is null or whitespaces");
                }
                else
                {
                    this.Locale = request.Locale;
                }

                if(request.Properties == null || request.Properties.Length == 0)
                {
                    throw new SampleException(ErrorCode.InvalidRequest, "Request properties is null or empty");
                }

                Hashtable properties = PaymentProperty.ConvertToHashtable(request.Properties);
            }
        }
    }
}
