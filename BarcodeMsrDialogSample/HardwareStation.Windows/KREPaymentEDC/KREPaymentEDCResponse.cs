using System.Runtime.Serialization;
using Microsoft.Dynamics.Commerce.Runtime.Messages;

namespace KREPaymentEDC.HardwareStation
{

    [DataContract]
    public class KREPaymentEDCResponse : Response
    {
        public KREPaymentEDCResponse(KREPaymentEDCResponseEntity entityObject)
        {
            this.entityObject = entityObject;
        }
        public KREPaymentEDCResponseEntity entityObject { get; set; }
    }
}
