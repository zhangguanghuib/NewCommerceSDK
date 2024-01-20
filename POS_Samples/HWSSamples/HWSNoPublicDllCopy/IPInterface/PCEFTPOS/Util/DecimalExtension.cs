using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public static class DecimalExtension
    {
        /// <summary>
        /// Pad a decimal string to an int
        /// </summary>
        /// <param name="v"></param>
        /// <param name="totalWidth">The width of the string to return</param>
        /// <example>
        /// <code>
        /// // Working example
        /// var d = 1.45m;
        /// var s = d.PadLeftAsInt(5);
        /// Debug.Assert(s == "00145");
        /// 
        /// // Error
        /// var d = 1.45m;
        /// var s = d.PadLeftAsInt(1); // throws ArgumentOutOfRangeException 
        /// </code>
        /// </example>
        /// <returns>A string of length totalWidth</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number is too long for the totalWidth provided</exception>
        public static string PadLeftAsInt(this decimal v, int totalWidth)
        {
            var s = String.Format($"{{0:D{totalWidth}}}", (int)(v * 100));
            if (s.Length > totalWidth)
                throw new ArgumentOutOfRangeException(nameof(totalWidth), $"The number '{v}' cannot fit in a padded string of length:{totalWidth}");
            return s;
        }
    }

}
