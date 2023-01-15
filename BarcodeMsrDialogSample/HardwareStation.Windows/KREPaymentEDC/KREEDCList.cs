using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KREPaymentEDC.HardwareStation
{
    class KREEDCList
    {
        private string edcName { get; set; }
        private string comName { get; set; }
        private Int32 baudRate { get; set; }

        public KREEDCList(string edcName, string comName, int baudRate)
        {
            this.edcName = edcName;
            this.comName = comName;
            this.baudRate = baudRate;
        }

        public string GetEdcName() { return edcName; }
        public string GetComName() { return comName; }
        public int GetBaudRate() { return baudRate; }
    }
}
