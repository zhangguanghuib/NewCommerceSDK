
namespace Microsoft.Dynamics
{
    namespace Retail.SampleConnector.Portable
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable;
        using Microsoft.Dynamics.Retail.PaymentSDK.Portable.Constants;

        internal static class PaymentUtilities
        {
            private const string OperationCompleted = "completed";

            internal static string GetPropertyStringValue(Hashtable properties, string propertyNamespace, string propertyName, List<PaymentError> errors, ErrorCode errorCode)
            {
                if(properties == null)
                {
                    throw new ArgumentNullException(nameof(properties));
                }

                string value;
                if(!PaymentProperty.GetPropertyValue(properties,propertyNamespace, propertyName, out value))
                {
                    value = null;
                    if(errors != null)
                    {
                        errors.Add(new PaymentError(errorCode, MissingPropertyMessage(propertyNamespace, propertyName)));
                    }
                }

                return value;
            }

            private static string MissingPropertyMessage(string propertyNamespace, string propertyName)
            {
                return string.Format("Missing {0}:{1}", propertyNamespace, propertyName);
            }
        }
    }
}
