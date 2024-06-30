namespace Contoso.PricingEngine.TradeAgreementCalculator
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using System.Collections.Generic;

    public static class PricingEngine
    {
        /// <summary>
        /// Gets minimum and maximum date time from set of sales lines or default date/time.
        /// </summary>
        /// <param name="salesLines">Lines to read date range from.</param>
        /// <param name="defaultDate">Date to fall back to if lines are missing dates.</param>
        /// <returns>Truncated min and max date suitable for querying price rules.</returns>
        internal static Tuple<DateTimeOffset, DateTimeOffset> GetMinAndMaxActiveDates(IEnumerable<SalesLine> salesLines, DateTimeOffset defaultDate)
        {
            DateTimeOffset? minDateTime = null;
            DateTimeOffset? maxDateTime = null;

            // if we have sales lines, find any min/max if any dates are specified
            if (salesLines != null)
            {
                foreach (var line in salesLines)
                {
                    if (line.SalesDate != null)
                    {
                        if (minDateTime == null || line.SalesDate < minDateTime)
                        {
                            minDateTime = line.SalesDate;
                        }

                        if (maxDateTime == null || line.SalesDate > maxDateTime)
                        {
                            maxDateTime = line.SalesDate;
                        }
                    }
                }
            }

            // default dates if none found
            minDateTime = minDateTime ?? defaultDate;
            maxDateTime = maxDateTime ?? defaultDate;

            // extend range to contain default date if necessary
            minDateTime = (minDateTime.Value < defaultDate) ? minDateTime : defaultDate;
            maxDateTime = (maxDateTime.Value > defaultDate) ? maxDateTime : defaultDate;

            // return discovered date range, truncated to midnight
            return new Tuple<DateTimeOffset, DateTimeOffset>(minDateTime.Value, maxDateTime.Value);
        }

    }
}
