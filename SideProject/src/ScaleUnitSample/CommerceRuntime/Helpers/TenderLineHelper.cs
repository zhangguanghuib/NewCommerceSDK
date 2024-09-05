
namespace CommerceRuntime.Helpers
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TenderLineHelper
    {
        public static void FillTenderLinesNumber(IList<TenderLine> tenderLines)
        {
            if (tenderLines.IsNullOrEmpty())
            {
                return;
            }
            decimal num = tenderLines.Max((TenderLine l) => l.LineNumber) + 1m;
            foreach (TenderLine item in tenderLines.Where((TenderLine x) => x.Status != TenderLineStatus.Historical && x.LineNumber == 0m))
            {
                item.LineNumber = num++;
            }
        }
    }
}
