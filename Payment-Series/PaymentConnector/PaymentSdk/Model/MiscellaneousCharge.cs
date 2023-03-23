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

        /// <summary>
        /// Represents a miscellaneous charge.
        /// </summary>
        internal class MiscellaneousCharge
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MiscellaneousCharge"/> class.
            /// </summary>
            internal MiscellaneousCharge()
            {
            }

            /// <summary>
            /// Gets or sets the type of the charge.
            /// </summary>
            /// <value>
            /// The type of the charge.
            /// </value>
            internal string ChargeType { get; set; }

            /// <summary>
            /// Gets or sets the charge amount.
            /// </summary>
            /// <value>
            /// The charge amount.
            /// </value>
            internal decimal? ChargeAmount { get; set; }
        }
    }
}
