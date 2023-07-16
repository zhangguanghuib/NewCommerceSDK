/// <summary>
///   class is static to hold EFT object
/// </summary>
using PCEFTPOS.EFTClient.IPInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Contoso.Commerce.HardwareStation.Extension.UPOS_HardwareStation.UPOS_PCEFTPOS
{
    static public class EFTClientIPHelper
    {
        public static IEFTClientIPAsync _eftAsync;
   
        static EFTClientIPHelper()
        {
        
        }      
    }
}
