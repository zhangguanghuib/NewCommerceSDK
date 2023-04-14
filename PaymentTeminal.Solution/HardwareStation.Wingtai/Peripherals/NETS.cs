using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Configuration;
//using System.Windows.Forms;

//THM - nets base class process for NETS terminal model VX520

namespace WTR.HWExt.Peripherals
{
    public class NETS
    {
        enum ProcessState { SendCommand, GetResponse };

        //data to be passed - change with response model        
        protected ResponseModel responseFromTerminal;

        public LogHelper logHelper;
        protected ResponseMessage responseMessage;
        public int sendCommandCounter = 0;

        private int TimerCount = 0;

        //constant
        protected const byte ENQ = 0x05;
        protected const byte ACK = 0x06;
        protected const byte NACK = 0x15;
        protected const byte EOT = 0x04;
        protected const byte STX = 0x02;
        protected const byte ETX = 0x03;
        protected const char charNAK = (char)0x15;
        protected const char charACK = (char)0x06;

        //serial component variable
        string comMessage = string.Empty;
        string indata = string.Empty;
        string indata2 = string.Empty;
        int responseFlag = 1;
        string indataBuffer = string.Empty;
        public System.IO.Ports.SerialPort Device;
        ProcessState state;

        #region SerialPort
        //initialize serial port device
        public void InitDevice(string _COMPort)
        {
            //change for USB converter.
            Device = new SerialPort(_COMPort, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            Device.Encoding = System.Text.Encoding.GetEncoding(28591);
            Device.Handshake = System.IO.Ports.Handshake.None;
            Device.DiscardNull = true;

            //handler for data receive
            Device.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public void Open()
        {
            if (Device.IsOpen)
            {
                Close();
            }
            Device.Open();
        }

        public void Close()
        {
            if (Device != null)
                Device.Close();
        }

        protected Boolean OpenDeviceInProcess(string _COMPORT)
        {
            this.InitDevice(_COMPORT);
            if (Device.IsOpen)
            {
                Device.Close();
            }
            try
            {
                Open();
            }
            catch (Exception ex)
            {
                logHelper.saveLog("Error on initialize terminal COM port" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                responseFromTerminal.ResponseText = ex.Message;
                return false;
            }
            return true;
        }

        #endregion

        private void init()
        {
            //init log
            //E:\Logs\NETSLogs
            //E:\Logs\CreditcardLogs
            //logHelper = new LogHelper(@"C:\NETSLog\NETS.txt", this);
            logHelper = new LogHelper(Path.GetTempPath() + "NETSLog\\NETS.txt", this);

            //logHelper = new LogHelper(@"E:\Logs\NETSLogs\NETS.txt", this);

            //init response model
            responseFromTerminal = new ResponseModel();
            //init response message constructor
            responseMessage = this.constructResponseMessage();
        }

        //Main flow engine for the payment process
        public virtual bool PaymentProcess(string _amount, string _ECR, string _COMPORT)
        {
            bool successFlag = false;

            //initialize object
            this.init();

            //construct amount and ECR
            StringBuilder amountFormat;
            StringBuilder ECRFormat;
            amountFormat = this.constructAmountRequest(_amount);
            ECRFormat = this.constructECRRequest(_amount, _ECR);

            string output = string.Empty;

            try
            {
                //try open the port
                if (this.OpenDeviceInProcess(_COMPORT))
                {
                    bool isSendCommandSuccess = false;
                    for (int i = 1; i <= 1; i++)
                    {
                        if (SendCommand(amountFormat.ToString(), ECRFormat.ToString()))//update
                        {
                            indataBuffer = string.Empty;
                            indata = string.Empty;//reset indata after get response from SendCommand
                            isSendCommandSuccess = true;
                            output = GetResponse(responseFlag);

                            break;
                        }
                        else
                        {
                            isSendCommandSuccess = false;
                        }
                        Thread.Sleep(1950);
                    }

                    //get all messages response from payment terminal
                    //convert response from terminal
                    if (indata != "")
                    {
                        responseFromTerminal = responseMessage.ConvertValues(indata);
                    }

                    logHelper.saveLog("Response data : " + System.Environment.NewLine + indata);
                    logHelper.saveLog("Response code is : " + responseFromTerminal.ResponseCode);

                    if (isSendCommandSuccess)
                    {
                        if (responseFlag == 0)
                        {
                            successFlag = true;
                        }
                        else
                        {
                            //After 3 times LRC not right if approved then its approved
                            if (responseFromTerminal.ResponseCode == "00")
                            {
                                successFlag = true;
                            }
                            else
                            {
                                successFlag = false;
                            }
                        }
                    }
                    else
                    {
                        responseFromTerminal.ResponseText = "Send command not successfull, please check log for details";
                        logHelper.saveLog("Send command not successfull, please check log for details");
                        successFlag = false;
                    }
                }
            }
            catch (TimeoutException tex)
            {
                successFlag = false; // make it to 2 also to identify the timeout
                Close();
                responseFromTerminal.ResponseText = "Terminal timeout exception, please check log for details";
                logHelper.saveLog("Terminal timeout exception" + System.Environment.NewLine + "Error details : " + tex.Message + System.Environment.NewLine + "Error stack : " + tex.StackTrace);
            }
            catch (Exception ex)
            {
                successFlag = false;
                Close();
                responseFromTerminal.ResponseText = "Terminal error exception, please check log for details";
                logHelper.saveLog("Terminal error exceptions" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
            }
            finally
            {
                Close();
            }

            logHelper.saveLog("End transaction");

            return successFlag;
        }

        //Send command to NETS terminal
        protected virtual bool SendCommand(string _amount, string _ecr)
        {
            bool isSent = false;
            byte[] requestMessageToTerminal;

            //construct request message to terminal
            requestMessageToTerminal = this.constructRequestMessageToTerminal(_amount, _ecr);

            try
            {
                try
                {
                    indataBuffer = string.Empty;
                    indata = string.Empty;
                    logHelper.saveLog("[POS send command to terminal] :" + System.Environment.NewLine + BitConverter.ToString(requestMessageToTerminal));
                    //send request to terminal
                    Device.Write(requestMessageToTerminal, 0, requestMessageToTerminal.Length);
                }
                catch (Exception ex)
                {
                    responseFromTerminal.ResponseText = ex.Message;
                    logHelper.saveLog("Error on send request to terminal" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                    throw ex;
                }

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                state = ProcessState.SendCommand;
                do
                {
                    //check if it's more than 2 seconds
                    if (watch.Elapsed.TotalMilliseconds >= 1950)
                    {
                        break;
                    }
                }
                while ((indata == string.Empty) || (watch.Elapsed.TotalMilliseconds >= 2000));

                watch.Stop();

                Thread.Sleep(1000); //need to give buffer time ?

                logHelper.saveLog("Send command data counter : " + sendCommandCounter + " indata : " + indata);

                if (indata != "") //if indata has value
                {
                    //get the response
                    char receive = indata.Substring(0, 1)[0];

                    if (receive == charNAK && sendCommandCounter <= 1) //if NAK received
                    {
                        logHelper.saveLog("[ Tml Reply NACK ]  : " + Convert.ToByte(receive).ToString());

                        sendCommandCounter++;
                        //send the command again
                        isSent = SendCommand(_amount, _ecr);
                    }
                    else if (receive == charACK) // if ACK received
                    {
                        isSent = true;
                        logHelper.saveLog("[ Tml Reply ACK ]  : " + Convert.ToByte(receive).ToString());
                    }
                    else //nothing received
                    {
                        isSent = false;
                        responseFromTerminal.ResponseText = "Nothing received from terminal please check COM port or the terminal";
                        logHelper.saveLog("Nothing received from terminal please check COM port or the terminal");
                    }
                }
                else if (indata == "" && sendCommandCounter <= 1) //no data received
                {
                    //SaveLogFile("NACK FAIL!! ", "RESPONSE: " + indata);    
                    logHelper.saveLog("[ Tml Reply ]  : ");
                    sendCommandCounter++;
                    isSent = SendCommand(_amount, _ecr);
                }
                else
                {
                    //send command more than 3 times then show error
                    if (sendCommandCounter > 2)
                    {
                        responseFromTerminal.ResponseText = "There is a problem in sending/receiving the data to terminal, please check log for details";
                        logHelper.saveLog("There is a problem in sending/receiving the data to terminal, please check log for details");
                    }
                    isSent = false;
                }
            }
            catch (Exception ex)
            {
                responseFromTerminal.ResponseText = ex.Message;
                logHelper.saveLog("Error on send request to terminal" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                throw ex;
            }

            logHelper.saveLog("End send command, request sent status : " + isSent.ToString() + " counter : " + sendCommandCounter + " indata : " + indata);

            return isSent;
        }

        //get response from NETS terminal
        protected virtual string GetResponse(int _response)
        {
            logHelper.saveLog("Start waiting response from terminal");
            Exception exception = null;
            string output = string.Empty;
            System.Timers.Timer MyTimer = new System.Timers.Timer();

            try
            {
                MyTimer.Interval = (2 * 60 * 1000); // 2 mins
                MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(MyTimer_Tick);
                MyTimer.Start();

                System.Diagnostics.Stopwatch watchTwoMinutes = new System.Diagnostics.Stopwatch();
                watchTwoMinutes.Start();
                state = ProcessState.GetResponse;
                do
                {
                    if (exception != null)
                    {
                        /// 2017NOV11 - Changing 
                        logHelper.saveLog("Exception:" + exception.Message + " Inner Exception:" + (exception.InnerException != null ? exception.InnerException.ToString() : "")
                            + "Error stack : " + exception.StackTrace);
                        //END

                        throw exception;
                    }

                    //check if it's over 2 minutes
                    if (watchTwoMinutes.Elapsed.TotalMilliseconds >= 120000)
                    {
                        MyTimer.Stop();
                        watchTwoMinutes.Stop();
                        if (string.IsNullOrEmpty(indata))
                        {
                            logHelper.saveLog("Terminal is time out");
                            responseFromTerminal.ResponseCode = "NEC_TO";
                            responseFromTerminal.ResponseText = "Terminal is time out";
                            return "Terminal is time out";
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                while ((indata == string.Empty || indata == ""));// || (indata.Substring(indata.Length - 2, 1)[0] != (char)3)); //|| !indata.Contains(""))

                if (exception != null)
                {
                    MyTimer.Stop();
                    throw exception;
                }

                bool isLoop = false;
                int sendNACK = 0;

                do
                {
                    //loop for NAK
                    //DebugHardWrite("GetResponse - VALIDATION" + "RESPONSE: " + indata);

                    byte calcLRC = new byte();
                    byte lrc = new byte();
                    byte[] resBufferCurrent;

                    if (sendNACK == 0)
                    {
                        byte[] resBuffer = new byte[indata.Length];
                        resBuffer = GetBytes8BitEncoding(indata);
                        resBufferCurrent = resBuffer;

                        lrc = resBuffer[resBuffer.Length - 1];
                        calcLRC = calculateLRC(resBuffer);
                    }
                    else
                    {
                        byte[] resBuffer1 = new byte[indata.Length];
                        resBuffer1 = GetBytes8BitEncoding(indata);
                        resBufferCurrent = resBuffer1;

                        lrc = resBuffer1[resBuffer1.Length - 1];
                        calcLRC = calculateLRC(resBuffer1);
                    }

                    byte lrcT2 = calculateLRCForLastByte(lrc);

                    logHelper.saveLog("Response with " +
                                          " [NAK count]: " + sendNACK +
                                          " length : " + indata.Length.ToString() + System.Environment.NewLine +
                                          " [LRC = ] : " + lrc + System.Environment.NewLine +
                                          " [Calculated LRC = ] : " + calcLRC + System.Environment.NewLine +
                                          " Data : " + System.Environment.NewLine + indata + System.Environment.NewLine +
                                          " [Tml response] : " + System.Environment.NewLine + BitConverter.ToString(resBufferCurrent));


                    logHelper.saveLog("[Calculated LRC = ] :" + calcLRC);

                    logHelper.saveLog("[ LRC = ] :" + lrc);

                    if (calcLRC == lrc)
                    {
                        logHelper.saveLog("[LRC Valid]");

                        byte[] ackbuf = new byte[1];
                        ackbuf[0] = 0x06;
                        logHelper.saveLog("[POS Sending ACK] : " + BitConverter.ToString(ackbuf));
                        try
                        {
                            Device.Write(ackbuf, 0, ackbuf.Length);
                        }
                        catch (Exception ex)
                        {
                            responseFromTerminal.ResponseText = "Error at processing response, please check log for details";
                            logHelper.saveLog("Error at processing response" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace
                                + " Inner Exception:" + (ex.InnerException != null ? ex.InnerException.ToString() : ""));
                            MyTimer.Stop();
                            throw ex;
                        }

                        output = indata;
                        isLoop = false;
                    }
                    else
                    {
                        #region Negative Acknowledgement
                        logHelper.saveLog("[LRC InValid]");
                        sendNACK += 1;
                        byte[] nackbuf = new byte[1];
                        nackbuf[0] = 0x15;

                        // WTR RAD NETS CERT
                        //if (sendNACK < 3)

                        if (sendNACK < 3)// Send NACK 3 times
                        {
                            logHelper.saveLog(" [Response - POS Sending NACK] : " + sendNACK + " time(s) " + ": " + BitConverter.ToString(nackbuf));
                            try
                            {
                                indataBuffer = string.Empty;
                                indata = string.Empty;
                                Device.Write(nackbuf, 0, nackbuf.Length);
                            }
                            catch (Exception ex)
                            {
                                responseFromTerminal.ResponseText = "Error at processing response, please check log for details";
                                logHelper.saveLog("Error at processing response" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);
                                MyTimer.Stop();
                                throw ex;
                            }

                            do
                            {
                                exception = null;
                                if (exception != null)
                                {
                                    MyTimer.Stop();
                                    throw exception;
                                }
                            }
                            while ((indata == string.Empty || !indata.Contains("")) && TimerCount <= 120);

                            //DebugHardWrite("GetResponse" + "Sending NegativeAcknowledgement BIGBOX*********:" + indata.ToString());

                            if (indata != string.Empty && TimerCount != 121)
                            {
                                isLoop = true;
                            }
                            else if (indata == String.Empty && TimerCount == 121)
                            {
                                isLoop = false;
                                MyTimer.Stop();
                                responseFromTerminal.ResponseText = "Response : Terminal is time out";
                                logHelper.saveLog("Response : Terminal is time out");
                                return "Response : Terminal is time out";
                            }
                        }
                        else
                        {
                            isLoop = false;
                            MyTimer.Stop();

                            // WTR RAD NETS CERT
                            try
                            {
                                //indataBuffer = string.Empty;
                                //indata = string.Empty;
                                Device.Write(nackbuf, 0, nackbuf.Length);
                            }
                            catch (Exception ex)
                            {
                                responseFromTerminal.ResponseText = "Error at processing response, please check log for details";
                                logHelper.saveLog("Error at processing response" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine + "Error stack : " + ex.StackTrace);

                                throw ex;
                            }
                            //END

                            //THM - check if LRC incorrect but has an approve response                        
                            ResponseMessage responseMessage = this.constructResponseMessage();
                            responseFromTerminal = responseMessage.ConvertValues(indata);

                            logHelper.saveLog("Response : NAK sending reached 3 times with response code : " + responseFromTerminal.ResponseCode);

                            if (responseFromTerminal.ResponseCode == "00")
                            {
                                return output;
                            }
                            else
                            {
                                responseFromTerminal.ResponseText = "Response : NACK sending reached 3 times. No Correct Response";
                                return "Response : NACK sending reached 3 times. No Correct Response";
                            }
                        }
                        #endregion
                    }
                }
                while (isLoop == true);

                if (TimerCount > 120)
                {
                    MyTimer.Stop();

                    if (indata == "")
                    {
                        logHelper.saveLog("Terminal is time out");
                        responseFromTerminal.ResponseCode = "NEC_TO";
                        responseFromTerminal.ResponseText = "Terminal is time out";
                        return "Terminal is time out";
                    }
                }

                MyTimer.Stop();
                Device.Close();
            }
            catch (Exception ex)
            {
                responseFromTerminal.ResponseText = ex.Message;
                logHelper.saveLog("Response - error in response" + System.Environment.NewLine + "Error details : " + ex.Message + System.Environment.NewLine
                    + "Error stack : " + ex.StackTrace + " Inner Exception:" + (ex.InnerException != null ? ex.InnerException.ToString() : ""));
                MyTimer.Stop();
            }


            return output;
        }

        //standard serial port receive handler
        protected void DataReceivedHandler(object _sender, SerialDataReceivedEventArgs _e)
        {
            SerialPort sp = (SerialPort)_sender;
            sp.Encoding = System.Text.Encoding.GetEncoding(28591);
            indataBuffer += sp.ReadExisting();

            //check the state and how to handle the data received
            switch (state)
            {
                case ProcessState.SendCommand:
                    indata = indataBuffer.Substring(indataBuffer.Length - 1, 1);
                    break;
                case ProcessState.GetResponse:
                    this.CheckGetResponseReceivedFullMessage(indataBuffer);
                    break;
            }

            //debugging purpose
            this.DebugLogForDataReceivedHandler(indataBuffer);
        }

        //log the data
        protected void DebugLogForDataReceivedHandler(string _debugInData)
        {
            string fileName = @"C:\NETS-resp.txt";
            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(state + "-" + _debugInData);
            }
        }

        //Check when GetResponse data is in the loop
        protected void CheckGetResponseReceivedFullMessage(string _inData)
        {
            if (_inData.Length > 2)
            {
                char lastchar = _inData.Substring(_inData.Length - 2, 1)[0];
                if (lastchar == (char)3) //check if its ETX
                {
                    indata = _inData;
                }
                else
                {
                    /// 2017NOV11 - Changing 
                    char lchar = _inData.Substring(_inData.Length - 1, 1)[0];

                    if (lchar == (char)3)
                    {
                        logHelper.saveLog("Response with LRC 0");
                        indata = _inData + "\0";
                    }
                    else
                    {
                        //ENd  ---- else loop also added.

                        indata = string.Empty;

                    }// End
                }
            }
            else
            {
                indata = string.Empty;
            }
        }

        //timer tick 2 minutes event
        protected void MyTimer_Tick(Object myObject, EventArgs myEventArgs)
        {
            TimerCount = 121;
        }

        //calculate LRC
        protected byte calculateLRC(byte[] b)
        {
            if (b == null)
            {
                return 0;
            }

            int j = 0;
            if (b[0] == 0x02)
            {
                j = 1;
            }
            else if (b[1] == 0x02)
            {
                j = 2;
            }
            else if (b[2] == 0x02)
            {
                j = 3;
            }
            else if (b[3] == 0x02)
            {
                j = 4;
            }

            byte lrc = 0x00;
            for (int i = j; i <= b.Length - 2; i++)
            {
                //SaveLogFile("calculateLRCNegative ------ if loop", null, "b[1]: " + b[i].ToString());
                lrc ^= b[i];
            }
            return lrc;
        }

        //calculate LRC last byte
        protected byte calculateLRCForLastByte(byte b)
        {
            byte lrc = 0x00;
            lrc ^= b;
            return lrc;
        }

        //calculate LRC for request
        protected byte calculateLRCRequest(byte[] bytes)
        {
            byte LRC = 0x00;
            for (int i = 1; i < bytes.Length; i++)
            {
                LRC ^= bytes[i];
            }
            return LRC;
        }

        //get message header for request
        protected byte[] GetMessageHeader(string FunctionCode)
        {
            string HeaderFiller = "000000000000";
            string RFU = "00";
            string EOM = "0";

            string ret = string.Empty;

            ret = HeaderFiller + FunctionCode + RFU + EOM;

            byte[] buff = Encoding.Default.GetBytes(ret);
            return buff;
        }

        //get message data for request
        protected byte[] GenerateMessageData(string FieldCode, int LenByte, string data)
        {
            string ret = string.Empty;

            byte[] len = IntToBCD(LenByte);

            int messageDataLength = FieldCode.Length + data.Length + 2;
            byte[] buffer = new byte[messageDataLength];

            byte[] fc = Encoding.Default.GetBytes(FieldCode);
            fc.CopyTo(buffer, 0);//Field code to transfer
            buffer[2] = len[0];
            buffer[3] = len[1];

            byte[] dt = Encoding.Default.GetBytes(data);
            Buffer.BlockCopy(dt, 0, buffer, 4, data.Length);

            return buffer;

        }

        //integer to byte code for request
        protected byte[] IntToBCD(int input)
        {
            if (input > 9999 || input < 0)
                throw new ArgumentOutOfRangeException("input");

            int thousands = input / 1000;
            int hundreds = (input -= thousands * 1000) / 100;
            int tens = (input -= hundreds * 100) / 10;
            int ones = (input -= tens * 10);

            byte[] bcd = new byte[] {
            (byte)(thousands << 4 | hundreds),
            (byte)(tens << 4 | ones)
            };

            return bcd;
        }

        //get response from terminal
        public ResponseModel getResponse()
        {
            return responseFromTerminal;
        }

        //construct Amount
        protected virtual StringBuilder constructAmountRequest(string _amount)
        {
            //this class needs to be overridden
            throw new System.ArgumentNullException("AmountRequest method needs to be overridden");
        }

        //construct ECR format
        protected virtual StringBuilder constructECRRequest(string _amount, string _ECR)
        {
            //this class needs to be overridden
            throw new System.ArgumentNullException("ECRRequest method needs to be overridden");
        }

        //construct request message to payment terminal
        protected virtual byte[] constructRequestMessageToTerminal(string _amount, string _ECR)
        {
            //this class needs to be overridden
            throw new System.ArgumentNullException("ECRRequest method needs to be overridden");
        }

        //conver to byte with 8 bit encoding
        private byte[] GetBytes8BitEncoding(string _str)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding(28591).GetBytes(_str);

            return bytes;
        }

        //construct response message
        protected ResponseMessage constructResponseMessage()
        {
            ResponseMessage respMessage;

            if (this.GetType().Equals(typeof(NETS_FC80_NETSLogOn)))
            {
                respMessage = new ResponseMessage_FC80_NETSLogOn(this);
            }
            else if (this.GetType().Equals(typeof(NETS_FC28_NETSPurchase)))
            {
                respMessage = new ResponseMessage_FC28_NETSPurchase(this);
            }
            else if (this.GetType().Equals(typeof(NETS_FC24_NETSContactlessDebit)))
            {
                respMessage = new ResponseMessage_FC24_NETSContactlessDebit(this);
            }
            else if (this.GetType().Equals(typeof(NETS_FC81_NETSSettlement)))
            {
                respMessage = new ResponseMessage_FC81_NETSSettlement(this);
            }
            else if (this.GetType().Equals(typeof(NETS_FC55_NETSTerminalStatus)))
            {
                respMessage = new ResponseMessage_FC55_NETSTerminalStatus(this);
            }
            else
            {
                respMessage = new ResponseMessage(this);
            }

            return respMessage;
        }
    }
}
