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
        /// Represents tax details.
        /// </summary>
        internal class TaxDetail
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TaxDetail"/> class.
            /// </summary>
            internal TaxDetail()
            {
            }

            /// <summary>
            /// Gets or sets the tax type identifier.
            /// </summary>
            /// <value>
            /// The tax type identifier.
            /// </value>
            internal string TaxTypeIdentifier { get; set; }

            /// <summary>
            /// Gets or sets the tax rate.
            /// </summary>
            /// <value>
            /// The tax rate.
            /// </value>
            internal decimal? TaxRate { get; set; }

            /// <summary>
            /// Gets or sets the tax description.
            /// </summary>
            /// <value>
            /// The tax description.
            /// </value>
            internal string TaxDescription { get; set; }

            /// <summary>
            /// Gets or sets the tax amount.
            /// </summary>
            /// <value>
            /// The tax amount.
            /// </value>
            internal decimal? TaxAmount { get; set; }
        }
    }
}
