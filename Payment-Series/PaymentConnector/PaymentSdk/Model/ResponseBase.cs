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
        /// Base class for response.
        /// </summary>
        internal abstract class ResponseBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseBase"/> class.
            /// </summary>
            /// <param name="locale">The locale.</param>
            /// <param name="serviceAccountId">The service account identifier.</param>
            /// <param name="connectorName">Name of the connector.</param>
            internal ResponseBase(string locale, string serviceAccountId, string connectorName)
            {
                this.Locale = locale;
                this.ServiceAccountId = serviceAccountId;
                this.ConnectorName = connectorName;
            }

            /// <summary>
            /// Gets or sets the locale.
            /// </summary>
            /// <value>
            /// The locale.
            /// </value>
            internal string Locale { get; set; }

            /// <summary>
            /// Gets or sets the service account identifier.
            /// </summary>
            /// <value>
            /// The service account identifier.
            /// </value>
            internal string ServiceAccountId { get; set; }

            /// <summary>
            /// Gets or sets the name of the connector.
            /// </summary>
            /// <value>
            /// The name of the connector.
            /// </value>
            internal string ConnectorName { get; set; }

            /// <summary>
            /// Writes the base properties.
            /// </summary>
            /// <param name="response">The response.</param>
            /// <exception cref="ArgumentNullException">response.</exception>
            protected void WriteBaseProperties(Response response)
            {
                if (response == null)
                {
                    throw new ArgumentNullException(nameof(response));
                }

                response.Locale = this.Locale;

                var properties = new List<PaymentProperty>();
                if (response.Properties != null)
                {
                    properties.AddRange(response.Properties);
                }

                properties.Add(new PaymentProperty(GenericNamespace.MerchantAccount, MerchantAccountProperties.ServiceAccountId, this.ServiceAccountId));
                properties.Add(new PaymentProperty(GenericNamespace.Connector, ConnectorProperties.ConnectorName, this.ConnectorName));
                response.Properties = properties.ToArray();
            }
        }
    }
}
