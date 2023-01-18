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
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO.Ports;
    using Microsoft.Dynamics.Commerce.HardwareStation;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.HardwareStation.CardPayment;
    using PaySdk = Microsoft.Dynamics.Retail.PaymentSDK.Portable;
    using System.Threading;


    /// <summary>
    /// The new ping controller is the new controller class to receive ping request.
    /// </summary>
    [RoutePrefix("KREPAYMENTEDC")]
    public class KREPaymentEDCController : IController
    {
        public System.IO.Ports.SerialPort serialPort1;
        internal string finalMessage;
        internal const int DataBits = 8;
        private System.ComponentModel.IContainer components = null;
        public KREPaymentEDCServices edcServices;
        //public KREPaymentEDCResponse edcResponse;
        internal int sumRespon;
        internal string responAll;
        internal string responCard;
        internal string responApprovalCode;
        internal string responApproval;
        internal string responDate;
        internal string responExpiredDate;
        internal bool qris;
        private string rtfTerminal;
        internal string responseIsSucceed;
        internal string responseApproval;
        internal string cardNumber;
        private string paymentConnectorName;

        internal const string folderConfig = @"C:\StoreCommerce\configedc";
        internal const string fileNameConfig = "EDCConfig.txt";
        internal const string pathFileConfig = folderConfig + @"\" + fileNameConfig;
        List<KREEDCList> edcList { get; set; }

        //[HttpPost]
        //public Task<string> EdcController(KREPaymentEDCRequest edcRequest, IEndpointContext context)
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.EdcClick(edcRequest.amount, edcRequest.isQris);
        //    this.createPaymentInfo(responCard);

        //    return Task.FromResult(string.Format("EDC Request Sent " + responCard + responApproval));

        //}


        //[HttpPost]
        //public async Task<KREPaymentEDCResponse> EdcController(KREPaymentEDCRequest edcRequest, IEndpointContext context)
        //{
        //    KREPaymentEDCResponse edcResponse;
        //    KREPaymentEDCResponseEntity responseEntity = new KREPaymentEDCResponseEntity();

        //    try
        //    {
        //        components = new System.ComponentModel.Container();
        //        loadEdcList();
        //        EdcClick(edcRequest.amount, edcRequest.isQris);


        //        while (responApproval == null)
        //        {
        //            ReceivedTextCimb(serialPort1.ReadExisting(), edcRequest.isQris);
        //            responseEntity.IsSucceed = "true";
        //            responseEntity.ResponApproval = responApproval;
        //        }

        //        edcResponse = new KREPaymentEDCResponse(responseEntity);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return await Task.FromResult(edcResponse);
        //}

        [HttpPost]
        public async Task<string> EdcController(KREPaymentEDCRequest edcRequest, IEndpointContext context)
        {
            KREPaymentEDCResponse edcResponse;
            KREPaymentEDCResponseEntity responseEntity = new KREPaymentEDCResponseEntity();

            try
            {
                components = new System.ComponentModel.Container();
                loadEdcList();

                EdcClick(edcRequest.amount, edcRequest.isQris);


                while (responApproval == null)
                {
                    ReceivedTextCimb(serialPort1.ReadExisting(), edcRequest.isQris);
                    responseEntity.IsSucceed = "true";
                    responseEntity.ResponApproval = responApproval;
                }

                edcResponse = new KREPaymentEDCResponse(responseEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responApproval;
            //return await Task.FromResult(responApproval);
        }


        [HttpPost]
        public async Task<string> EdcControllerV2(KREPaymentEDCRequest edcRequest, IEndpointContext context)
        {
            KREPaymentEDCResponse edcResponse;
            KREPaymentEDCResponseEntity responseEntity = new KREPaymentEDCResponseEntity();
            try
            {
                //components = new System.ComponentModel.Container();
                //loadEdcList();

                //EdcClick(edcRequest.amount, edcRequest.isQris);

                SerialPort serialPort = new SerialPort();
                serialPort.PortName = "COM3";
                //波特率
                serialPort.BaudRate = 9600;
                //奇偶校验
                serialPort.Parity = Parity.None;
                //数据位
                serialPort.DataBits = 8;
                //停止位
                serialPort.StopBits = StopBits.One;

                bool isOpen = serialPort.IsOpen;
                if (!isOpen)
                {
                    serialPort.Open();
                }
                //发送字节数组
                //参数1：包含要写入端口的数据的字节数组。
                //参数2：参数中从零开始的字节偏移量，从此处开始将字节复制到端口。
                //参数3：要写入的字节数。 
                serialPort.Write(edcRequest.amount);
                responApproval = await CheckApprovalAsyn(serialPort).ConfigureAwait(false);
                if (responApproval != null)
                {
                    responseEntity.IsSucceed = "true";
                    responseEntity.ResponApproval = responApproval;
                }

                edcResponse = new KREPaymentEDCResponse(responseEntity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(responApproval);
        }

        private async Task<string> CheckApprovalAsyn(SerialPort serialPort)
        {
            responApproval = null;
            int i = 0;
            while (responApproval == null)
            {
                await Task.Delay(1000);
                //ReceivedTextCimb(serialPort1.ReadExisting(), edcRequest.isQris);

                //If there is some data from serial port to be read
                if (serialPort.BytesToRead > 0)
                {
                    byte[] result = new byte[serialPort.BytesToRead];
                    serialPort.Read(result, 0, serialPort.BytesToRead);
                    responApproval = "Approved";
                    break;
                }

                //Simulate timeout after 10 times try, set cancelled
                if (i >= 10)
                {
                    responApproval = "Canceled";
                    break;
                }
                ++i;
            }
            return responApproval;
        }

        public string getEdcNameBca()
        {
            return "BCA";
        }

        public string getEdcNameCimb()
        {
            return "CIMB";
        }

        public string getEdcNameBNI()
        {
            return "BNI";
        }
        private PaymentInfo createPaymentInfo(string responCard)
        {
            // Several below values are hardcoded, real implementations should take this data from real device.
            PaySdk.PaymentProperty[] properties = GetPaymentProperties(this.paymentConnectorName);
            var paymentInfo = new PaymentInfo()
            {
                CardNumberMasked = responCard
            };
            return paymentInfo;
        }

        private static PaySdk.PaymentProperty[] GetPaymentProperties(string paymentConnectorName)
        {
            // Several below values are hardcoded, real implementations should take this data from real device.
            PaySdk.PaymentProperty assemblyNameProperty = new PaySdk.PaymentProperty();
            assemblyNameProperty.Namespace = PaySdk.Constants.GenericNamespace.MerchantAccount;
            assemblyNameProperty.Name = PaySdk.Constants.MerchantAccountProperties.AssemblyName;
            assemblyNameProperty.ValueType = PaySdk.DataType.String;
            //assemblyNameProperty.StoredStringValue = typeof(PaymentDeviceSample).Assembly.GetName().Name;


            PaySdk.PaymentProperty connectorNameProperty = new PaySdk.PaymentProperty();
            connectorNameProperty.Namespace = PaySdk.Constants.GenericNamespace.Connector;
            connectorNameProperty.Name = PaySdk.Constants.ConnectorProperties.ConnectorName;
            connectorNameProperty.StoredStringValue = paymentConnectorName;

            PaySdk.PaymentProperty[] properties = new PaySdk.PaymentProperty[] { assemblyNameProperty, connectorNameProperty };

            return properties;
        }
        private List<KREEDCList> loadEdcList()
        {
            string line;
            System.IO.StreamReader fileIn = new System.IO.StreamReader(pathFileConfig);
            edcList = new List<KREEDCList>();
            while ((line = fileIn.ReadLine()) != null)
            {
                string[] words = line.Split(',');
                if (words.Length < 3) continue;
                edcList.Add(new KREEDCList(words[0].ToUpper().Trim(), words[1].ToUpper().Trim(), Convert.ToInt32(words[2].Trim())));
            }

            fileIn.Close();

            return edcList;
        }

        private void getEdcName()
        {
            //edcClass = new EDC();
            KREEDCList edc;
            
            edc = edcList.Where(f => f.GetEdcName().Equals(getEdcNameCimb())).FirstOrDefault();
            if (edc != null)
            {
                openPort(edc.GetBaudRate(), edc.GetComName());
            }
        }

        private void EdcClick(string amount, bool isQris)
        {
            edcServices = new KREPaymentEDCServices();
            serialPort1 = new System.IO.Ports.SerialPort(this.components);
            
            getEdcName();
            //openPort(115200, "COM4");
            sendMessage(amount, isQris);   
        }

        public void openPort(int baudRate, string portComm)
        {
            //bool error = false;

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

        private void sendMessage(string amount, bool isQris)
        {
            try
            {
                finalMessage = edcServices.createSendDataCimb(amount, isQris);
                //serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceivedCimb);


                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(finalMessage);
                }
            }
            catch (System.IO.IOException)
            {
                
            }
        }

        //private void port_DataReceivedCimb(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        //{
        //    this.ReceivedTextCimb(serialPort1.ReadExisting());
        //}

        private delegate void SetTextCallback(string text);
        private void ReceivedTextCimb(string text, bool isQris)
        {
            //SetTextCallback d = new SetTextCallback(ReceivedTextCimb);
            //invoke(d, new object[] { text });

            string pedLeft = String2Hex(text).PadLeft(2, '0');
            switch (pedLeft)
            {
                case "00":
                    sumRespon = 0;
                    break;

                case "06":
                    this.rtfTerminal += String2Hex(text).PadLeft(2, '0');
                    break;

                default:
                    this.rtfTerminal += String2Hex(text).PadLeft(2, '0');
                    break;
            }

            finalMessage = HexAsciiConvert("06");
            serialPort1.Write(finalMessage);
            responAll = HexAsciiConvert(this.rtfTerminal);

            if (responAll.Length >= 20)
            {
                responApprovalCode = responAll.Substring(20, 2); //cek response status

                if (responApprovalCode.Trim() == "00") //if OK
                {
                    if (isQris)
                    {
                        responCard = responAll.Substring(36, 12);
                        responApproval = responAll.Substring(36, 12);
                        responDate = responAll.Substring(69, 8);
                    }
                    else
                    {
                        responCard = responAll.Substring(92, 16);
                        responApproval = responAll.Substring(33, 6);
                        responDate = responAll.Substring(75, 8);
                    }
                }
            }
        }

        private void invoke(SetTextCallback d, object[] v)
        {
            throw new NotImplementedException();
        }

        private string String2Hex(string str)
        {
            string decString = str;
            byte[] bytes = System.Text.Encoding.Default.GetBytes(decString);
            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", "");

            return hexString;

        }

        private string HexAsciiConvert(string hex)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i <= hex.Length - 2; i += 2)
            {
                sb.Append(Convert.ToChar(Int32.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber)));
            }

            return sb.ToString();
        }

    }
}