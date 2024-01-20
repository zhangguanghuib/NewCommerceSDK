/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace KREPaymentEDC.HardwareStation
{
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;

        /// <summary>
        /// The new ping controller is the new controller class to receive ping request.
        /// </summary>
        [RoutePrefix("PAYMENTEDC")]
        public class KREPaymentController : IController
        {
            public System.IO.Ports.SerialPort serialPort1;
            internal string finalMessage;
            internal const int DataBits = 8;

            [HttpPost]
            public Task<string> EdcController(IEndpointContext context)
            {
                
                this.EdcClick();

                return Task.FromResult(string.Format("MASHOOK SINI"));

            }

            public void EdcClick()
            {
                this.openPort(115200, "COM4");
                this.sendMessage();
            }

            public void openPort(int baudRate, string portComm)
            {
                //bool error = false;

               if(serialPort1 == null)
               {
                   serialPort1 = new System.IO.Ports.SerialPort();
               }

                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();

                }
                else
                {

                    serialPort1.BaudRate = baudRate;
                    serialPort1.DataBits = DataBits;
                    serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    serialPort1.Parity = System.IO.Ports.Parity.None;
                    serialPort1.Handshake = System.IO.Ports.Handshake.None;
                    serialPort1.PortName = portComm;


                    serialPort1.Open(); 
                }

            }

            private void sendMessage()
            {
                finalMessage = "023C41303841314232433344344230373103527A";
                serialPort1.Write(finalMessage);

            }
    }
}