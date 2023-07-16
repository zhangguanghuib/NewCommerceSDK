/// <summary>
///   class is used for default PCEFTPOS settings
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Commerce.HardwareStation.Extension.UPOS_HardwareStation.UPOS_PCEFTPOS
{
    public interface ISettings
    {
        string EFTClientAddress { get; set; }
        void Save();
        void Load();
    }

    class Settings : ISettings
    {
        public string EFTClientAddress
        {
            get
            {
                return "127.0.0.1:2011";
            }
            set
            {
              
            }
        }

        public void Load()
        {
          
        }

        public void Save()
        {
          
        }
    }
}
