/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.XZReportsFrance
    {
        using System;
        using Microsoft.Dynamics.Retail.Resources.Strings;

        /// <summary>
        /// Encapsulates localization functionality for custom French X/Z reports.
        /// </summary>
        public class XZReportsLocalizer
        {
            private const string LocalizationResourceFileNameTemplate = "{0}.Properties.Resources.Resources";

            private static readonly Lazy<XZReportsLocalizer> LocalizerInstance = new Lazy<XZReportsLocalizer>(() => new XZReportsLocalizer());
            private readonly StringLocalizer localizer;

            private XZReportsLocalizer()
            {
                string resourceFileName = string.Format(LocalizationResourceFileNameTemplate, typeof(XZReportsLocalizer).Namespace);
                this.localizer = new StringLocalizer(resourceFileName, typeof(XZReportsLocalizer).Assembly);
            }

            /// <summary>
            /// Gets <c>XZReportsLocalizer</c> instance.
            /// </summary>
            public static XZReportsLocalizer Instance
            {
                get
                {
                    return LocalizerInstance.Value;
                }
            }

            /// <summary>
            /// Translates the specified text identifier according to culture name.
            /// </summary>
            /// <param name="cultureName">The name of the culture.</param>
            /// <param name="textId">The text identifier.</param>
            /// <returns>The localized string.</returns>
            public string Translate(string cultureName, string textId)
            {
                return this.localizer.Translate(cultureName, textId);
            }
        }
    }
}
