//using MagIC3DBSECRLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using MagIC3DBSECRLib;//DBSTerminal

namespace WTR.HWExt.Peripherals
{
    public class IngenicoPaymentDevice
    {
        public int TERMINAL_WAIT_TIMEOUT = 90000;
        public int RECV_INTERVAL = 1000;
        public int inRunningNumber = 1;
        delegate void SetTextCallback(string text, string response);
        delegate void SetMessageCallback(string text);
        public string stComSelected;
        public string stResponse;
        public string stCommand;

        public bool boCancel;
        public Thread thread;
        public BackgroundWorker bgWorker;

        //WTR_HGM - create Textbox control to handle Data
        TextBox textBoxAmount = new TextBox();
        TextBox textBoxCommandString = new TextBox();
        TextBox textBoxResponse = new TextBox();
        TextBox textBoxReceipt = new TextBox();
        //WTR_HGM - END

        //int _count;
        string stCommandID;
        string stValue;
        string stAmount;
        string stReceiptValue;// = string.Empty;
        bool IsContactlessSale = false;//if Contactless Sale Command

        //WTR_HGM - Response Codes 
        /*
            TID: 12345678  
            MID: 168168123456
            DATE TIME: 22/06 15:26:27
            INVOICE: 000096  BATCH: 000004
            MASTERCARD: XXXXXXXXXXXX0371
            RRN: 925557854726
            APPROVAL CODE: 777678
            ENTRY MODE: CONTACTLESS
            APP LABEL: MasterCard      
            AID: A000000004101000
            TVR TSI: 0000000000     
            TC: AAE86EFDBF5D1F80
            AMOUNT:   $0.01
         */
        public string CardNumberMasked;
        public string Amount;
        public string TransactionDate;
        public string ExpDate;
        public string EntryMode;
        public string RetrievalReferenceNumber;
        public string ApprovalCode;
        public string ResponseCode;
        public string ResponseMessage;
        public string TerminalID;
        public string MerchantID;
        public string Host;
        //EMV Data Start
        public string TVRTSI;
        public string AID;
        public string ApplicationLabel;
        public string TC;
        // EMV Data End
        public string CardLabel;
        public string CardType;
        public string HostType;
        public string CommandIdentifier;
        public string CustomData2;
        public string CustomData3;
        public string UTRN;
        public string InvoiceNumber;
        public string TransactionInfo;
        public string BatchNumber;
        public string CouponsVouchers; //Not used currently
        public string CardHolderName;
        public string CardEntryStatus;
        //WTR_HGM -end
        //HGM Start
        public string BatchInformation;
        public List<CCSettlementResponse> CCSettleResponseList;


        //WTR RAD
        public LogHelper logHelper;

        private void init()
        {
            //init log
            logHelper = new LogHelper(Path.GetTempPath() + "CREDITCARDLOG\\Credit.txt", this);

        }


        //END



        //Sale 
        #region NEC_WTR Custom Sale Command
        //Method Call to use to Handle Payment

        public string SendSaleCommand(string COMPORT, string AMOUNT, bool CONTACTLESS)
        {
            //WTR RAD
            init();
            //END


            //WTR_HGM
            //TODO Add additional 00 to AMOUNT
            //AMOUNT = AMOUNT;// + "00";
            //Textbox initialization defaul values
            if (CONTACTLESS)
                IsContactlessSale = true;

            textBoxAmount.Text = AMOUNT;
            textBoxCommandString.Text = "C20004120000000001005706000001";//initial value

            if (this.textBoxAmount.Text.Length == 0 || this.textBoxAmount.Text.Length > 12)
            {
                return "error";
            }

            if (IsContactlessSale) //WTR_HGM = if (checkBoxClessOnly.Checked == true)
            {
                textBoxCommandString.Text = "C630";//contactless sale command
            }
            else
            {
                textBoxCommandString.Text = "C200";//Sale Command
            }

            if (textBoxCommandString.Text.Length > 0 && textBoxCommandString.Text.Length <= 12)
            {
                textBoxCommandString.Text += "0412";
                textBoxCommandString.Text += textBoxAmount.Text.PadLeft(12, '0');
            }
            textBoxCommandString.Text += "5706";
            textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
            incrementRunningNumber();

            stComSelected = COMPORT;
            stCommand = textBoxCommandString.Text;

            if (stComSelected.Length == 0)
                return "error";
            var threads = new List<Thread>();

            //WTR_HGM - thread start
            thread = new Thread(new ThreadStart(SendRecvThreadFunction));
            thread.Start();

            threads.Add(thread);

            foreach (var threadT in threads)
            {
                threadT.Join();
            }

            //WTR_HGM - thread end.
            //wait for the thread to finish before returning
            return stReceiptValue;//Return Value containing result.
            //Get Response
        }
        #endregion

        //Void
        #region NEC_WTR Custom Void Command
        //Requires Terminal Password.
        //Function call to Void Previous Transactions
        //Parameters 
        //Invoice Number
        /*
            The purpose of this command is to VOID/Cancel a particular C20X transaction.
            This command will instruct the terminal to void any of the following transaction types: 
            SALE (C200) 
            PRE-AUTH CAPTURE (C202) 
            REFUND (C203) 
            CASH-ADVANCE (C204)
         */
        public string SendVoidCommand(string COMPORT, string InvoiceNumber)
        {
            if (InvoiceNumber.Length == 0 || InvoiceNumber.Length > 6)
            {
                return string.Empty;
            }
            textBoxCommandString.Text = "C300";
            if (InvoiceNumber.Length > 0 && InvoiceNumber.Length <= 6)
            {
                textBoxCommandString.Text += "6206";
                textBoxCommandString.Text += InvoiceNumber.PadLeft(6, '0');
            }
            textBoxCommandString.Text += "5706";
            textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
            incrementRunningNumber();

            stComSelected = COMPORT;
            stCommand = textBoxCommandString.Text;

            if (stComSelected.Length == 0)
                return "error";
            var threads = new List<Thread>();

            //WTR_HGM - thread start
            thread = new Thread(new ThreadStart(SendRecvThreadFunction));
            thread.Start();

            threads.Add(thread);

            foreach (var threadT in threads)
            {
                threadT.Join();
            }

            //WTR_HGM - thread end.
            //wait for the thread to finish before returning
            return stReceiptValue;//Return Value containing result.
            //Get Response
        }

        #endregion

        //Offline Sale
        #region NEC_WTR Custom Offline Sale Command
        //Method Call to use to Handle Payment
        //bool IsContactlessSale = false;//if Contactless Sale Command
        public string SendOfflineSaleCommand(string COMPORT, string AMOUNT, string OFFLINEAPPROVALCODE)
        {
            //Textbox initialization defaul values
            textBoxAmount.Text = AMOUNT;
            textBoxCommandString.Text = "C20004120000000001005706000001";//initial value

            if (this.textBoxAmount.Text.Length == 0 || this.textBoxAmount.Text.Length > 12)
            {
                return "error";
            }

            if (IsContactlessSale) //WTR_HGM = if (checkBoxClessOnly.Checked == true)
            {
                textBoxCommandString.Text = "C630";//contactlest sale command
            }
            else
                textBoxCommandString.Text = "C200";//Sale Command

            if (textBoxCommandString.Text.Length > 0 && textBoxCommandString.Text.Length <= 12)
            {
                textBoxCommandString.Text += "0412";
                textBoxCommandString.Text += textBoxAmount.Text.PadLeft(12, '0');
            }
            textBoxCommandString.Text += "5706";
            textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
            incrementRunningNumber();

            stComSelected = COMPORT;
            stCommand = textBoxCommandString.Text;

            if (stComSelected.Length == 0)
                return "error";
            var threads = new List<Thread>();

            //WTR_HGM - thread start
            thread = new Thread(new ThreadStart(SendRecvThreadFunction));
            thread.Start();

            threads.Add(thread);

            foreach (var threadT in threads)
            {
                threadT.Join();
            }

            //WTR_HGM - thread end.
            //wait for the thread to finish before returning
            return stReceiptValue;//Return Value containing result.
            //Get Response
        }
        #endregion

        //Refund
        #region NEC_WTR Custom Refund Command
        public string SendRefundCommand(string COMPORT, string AMOUNT)
        {
            init();
            //Textbox initialization defaul values
            textBoxAmount.Text = AMOUNT;
            textBoxCommandString.Text = "C20004120000000001005706000001";//initial value

            if (textBoxAmount.Text.Length == 0 || textBoxAmount.Text.Length > 12)
            {
                textBoxAmount.Focus();
                return string.Empty;
            }

            textBoxCommandString.Text = "C203";
            if (textBoxCommandString.Text.Length > 0 && textBoxCommandString.Text.Length <= 12)
            {
                textBoxCommandString.Text += "0412";
                textBoxCommandString.Text += textBoxAmount.Text.PadLeft(12, '0');
            }
            textBoxCommandString.Text += "5706";
            textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
            incrementRunningNumber();

            /*
                // refund
                if (textBoxAmount.Text.Length == 0 || textBoxAmount.Text.Length > 12)
                {
                    textBoxAmount.Focus();
                    return;
                }

                textBoxCommandString.Text = "C203";
                if (textBoxCommandString.Text.Length > 0 && textBoxCommandString.Text.Length <= 12)
                {
                    textBoxCommandString.Text += "0412";
                    textBoxCommandString.Text += textBoxAmount.Text.PadLeft(12, '0');
                }
                textBoxCommandString.Text += "5706";
                textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
                incrementRunningNumber();
                buttonSendAndRecv.PerformClick();
             */
            stComSelected = COMPORT;
            stCommand = textBoxCommandString.Text;

            if (stComSelected.Length == 0)
                return "error";
            var threads = new List<Thread>();

            //WTR_HGM - thread start
            thread = new Thread(new ThreadStart(SendRecvThreadFunction));
            thread.Start();

            threads.Add(thread);

            foreach (var threadT in threads)
            {
                threadT.Join();
            }

            //WTR_HGM - thread end.
            //wait for the thread to finish before returning
            return stReceiptValue;//Return Value containing result.
            //Get Response
        }
        #endregion
        //Settlement
        /*
         This command will instruct the terminal to perform settlement and is mandatory for all kiosk based / unattended systems. 
         */
        #region NEC_WTR Custom Settlement Command
        public List<CCSettlementResponse> SendSettlementCommand(string COMPORT)
        {
            /*
                textBoxCommandString.Text = "C700";
                textBoxCommandString.Text += "5706";
                textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
                incrementRunningNumber();
                buttonSendAndRecv.PerformClick();
             */
            textBoxCommandString.Text = "C700";
            textBoxCommandString.Text += "5706";
            textBoxCommandString.Text += inRunningNumber.ToString().PadLeft(6, '0');
            incrementRunningNumber();


            stComSelected = COMPORT;
            stCommand = textBoxCommandString.Text;

            if (stComSelected.Length == 0)
                return CCSettleResponseList;
            var threads = new List<Thread>();
            CCSettleResponseList = new List<CCSettlementResponse>();
            //WTR_HGM - thread start
            thread = new Thread(new ThreadStart(SendRecvThreadFunction));
            thread.Start();

            threads.Add(thread);

            foreach (var threadT in threads)
            {
                threadT.Join();
            }

            //WTR_HGM - thread end.
            //wait for the thread to finish before returning
            return CCSettleResponseList;//Return Value containing result.
            //Get Response
        }
        #endregion

        #region Ingenico Code

        private void incrementRunningNumber()
        {
            inRunningNumber++;
            if (inRunningNumber > 999999)
                inRunningNumber = 1;
        }
        private void WorkThreadFunction()
        {
            //IMagIC3Connector DBSTerminal = new MagIC3ConnectorClass();
            //long lgPortStat = DBSTerminal.EchoTest(stComSelected, out stResponse);
            
            /*string errorText;//WTR_HGM
            string OkText;
            if (lgPortStat != 0)
                errorText = "PORT ERROR CODE:\n"; //ConvertResponse("PORT ERROR CODE:\n" + lgPortStat, null);

            else
                OkText = "GET RESPONSE OK"; //ConvertResponse("GET RESPONSE OK", stResponse);
                */
        }

        public int getTagValue(string ResponseString, int inTag, out string Value)
        {
            int i = 0;
            string tag;
            string length;
            string value;
            if (ResponseString == null)
                goto FnEnd;
            for (i = 4; i < ResponseString.Length;)
            {
                tag = ResponseString.Substring(i, 2);
                i += 2;
                length = ResponseString.Substring(i, 2);
                i += 2;
                if (int.Parse(length) > 0)
                {
                    value = ResponseString.Substring(i, int.Parse(length));

                    if (int.Parse(tag) == inTag)
                    {
                        Value = value;
                        return 0;
                    }
                    i += int.Parse(length);
                }
            }

        FnEnd:
            Value = null;
            return 1; // Error tag not found

        }

        //WTR_HGM TODO - Check if needed
        private void SendCommandThreadFunction()
        {
            //IMagIC3Connector DBSTerminal = new MagIC3ConnectorClass();

            //long lgPortStat = DBSTerminal.SendCommand(stComSelected, stCommand);
            
            /*string errorText;//WTR_HGM
            string OkText;
            if (lgPortStat != 0)
                errorText = "PORT ERROR CODE:\n";
            //ConvertResponse("PORT ERROR CODE:\n" + lgPortStat, null);
            else
                errorText = "SEND COMMAND OK";
                */
            //ConvertResponse("SEND COMMAND OK", null);
        }

        private void ShowMessageCallback(string text)
        {
            SetMessageCallback d = new SetMessageCallback(ShowMessageCallback);
            if (String.Compare(text, 0, "R971", 0, 4) == 0)
            {
                ConvertResponse(text, stResponse);
            }
        }

        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);

        private static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
        }

        private void SendRecvThreadFunction()
        {
            do
            {
                // IMagIC3Connector DBSTerminal = new MagIC3ConnectorClass();
                boCancel = false;
                int inRecvCount = 0;
                //long lgPortStat = DBSTerminal.SendCommand(stComSelected, stCommand);

                long lgPortStat = 0;

                //WTR RAD
                logHelper.saveLog("[POS SEND COMMAND] : COM is " + stComSelected + " Cmd " + stCommand);

                logHelper.saveLog("[Terminal Reply] : " + lgPortStat);

                //END

                //string errorText;//WTR_HGM
                //string OkText;

                if (lgPortStat != 0)
                {
                    //errorText = "PORT ERROR CODE:   \n";


                }
                else
                {
                    do
                    {
                        //boMessageCallback = false;
                        inRecvCount++;
                        //lgPortStat = DBSTerminal.RecvResponse(stComSelected, out stResponse, RECV_INTERVAL);
                        lgPortStat = 0;
                        stResponse = "R923";
                        if (lgPortStat == 0)
                        {
                            if (String.Compare(stResponse, 0, "R923", 0, 4) == 0 || String.Compare(stResponse, 0, "R971", 0, 4) == 0)
                            {
                                //boMessageCallback = true;
                                ShowMessageCallback(stResponse);
                                continue;
                            }
                            break;
                        }
                        else if (lgPortStat == 1460)
                        {
                            if (inRecvCount < (TERMINAL_WAIT_TIMEOUT / RECV_INTERVAL))
                            {
                                if (boCancel)//Cancel the Send command
                                {
                                    //DBSTerminal.Cancel(stComSelected);
                                    boCancel = false;
                                }
                                continue;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    } while (true);

                    if (lgPortStat != 0)
                    {
                        //errorText = "PORT ERROR CODE:\n";
                        ConvertResponse("PORT ERROR CODE:\n" + lgPortStat, null); //WTR_HGM
                    }
                    else
                    {
                        //OkText = "GET RESPONSE OK";

                        //WTR RAD
                        logHelper.saveLog("[Terminal Response] : " + stResponse);
                        //END

                        ConvertResponse("GET RESPONSE OK", stResponse); //WTR_HGM
                    }
                }
                string val = string.Empty;
            }
            while (false);//Send in a loop to Terminal

        }

        //WTR_HGM - TODO check if needed
        private void RecvResponseThreadFunction()
        {
            //IMagIC3Connector DBSTerminal = new MagIC3ConnectorClass();
            long lgPortStat = 0;
            bool boMessageCallback = false;
            do
            {
                boMessageCallback = false;
                //lgPortStat = DBSTerminal.RecvResponse(stComSelected, out stResponse, 90000);
                if (lgPortStat == 0)
                {
                    // Check if it's a R923 message callback. If it is then display the message on the screen.
                    if (String.Compare(stResponse, 0, "R923", 0, 4) == 0)
                    {
                        boMessageCallback = true;
                        ShowMessageCallback(stResponse);
                    }
                }
                else
                    break;

            } while (boMessageCallback);
        }

        private void ConvertResponse(string text, string response)
        {
            string stResponseCode;
            {
                if (stResponse == null)
                    return;
                //if (textBoxResponse.Text.Length != 0 &&
                if (String.Compare(stResponse, 0, "R20", 0, 3) == 0 ||
                   String.Compare(stResponse, 0, "R630", 0, 4) == 0 ||
                   String.Compare(stResponse, 0, "R300", 0, 4) == 0 ||
                   String.Compare(stResponse, 0, "R600", 0, 4) == 0 ||
                   String.Compare(stResponse, 0, "R971", 0, 4) == 0) //26 Sep 2016 HGM
                {
                    if (getTagValue(stResponse, 39, out stResponseCode) == 0)
                    {
                        if (String.Compare(stResponseCode, "00") == 0)
                        {
                            //SetControlPropertyThreadSafe(label5, "Text", "APPROVED!");
                            //label5.Text = "APPROVED!";
                            //label8.Text = "ENJOY!";

                            //WTR RAD
                            logHelper.saveLog("[RESPONSE CODE] : " + stResponseCode);
                            //END

                            getTagValue(stResponse, 57, out stCommandID);
                            //textBox4.Text = "";
                            SetControlPropertyThreadSafe(textBoxReceipt, "Text", "");
                            stReceiptValue = "";
                            stReceiptValue += "-----------------------------------\r\n";
                            if (String.Compare(stResponse, 0, "R200", 0, 4) == 0)
                                stReceiptValue += "              SALE\r\n";
                            else if (String.Compare(stResponse, 0, "R600", 0, 4) == 0)
                                stReceiptValue += "              SALE\r\n";
                            else if (String.Compare(stResponse, 0, "R630", 0, 4) == 0)
                                stReceiptValue += "              SALE\r\n";
                            else if (String.Compare(stResponse, 0, "R201", 0, 4) == 0)
                                stReceiptValue += "            PREAUTH\r\n";
                            else if (String.Compare(stResponse, 0, "R202", 0, 4) == 0)
                                stReceiptValue += "            OFFLINE\r\n";
                            else if (String.Compare(stResponse, 0, "R203", 0, 4) == 0)
                                stReceiptValue += "             REFUND\r\n";
                            else if (String.Compare(stResponse, 0, "R300", 0, 4) == 0)
                                stReceiptValue += "             VOID\r\n";
                            else
                                stReceiptValue += "           TRANSACTION\r\n";
                            stReceiptValue += "-----------------------------------\r\n";
                            //stReceiptValue += "\r\n";
                            /*
                                string TerminalID;
                                string MerchantID;
                                string TransactionDate;
                                string InvoiceNumber;
                                string BatchNumber;
                                string CardNumberMasked;
                                string RRN;
                                string ApprovalCode;
                                string EntryCode;
                                string ApplicationLabel;
                                string AID;
                                string TVRTSI;
                                string TC;
                                string Amount;
                             */





                            if (getTagValue(stResponse, 2, out stValue) == 0)
                            {
                                // card number
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";
                                CardNumberMasked = stValue;//WTR_HGM
                            }

                            if (getTagValue(stResponse, 4, out stValue) == 0)
                            {
                                stReceiptValue += "AMOUNT:   ";
                                stAmount = int.Parse(stValue.Substring(0, 10)).ToString();
                                stReceiptValue += "$";
                                stReceiptValue += stAmount;
                                Amount = stAmount;//WTR_HGM
                                stReceiptValue += ".";
                                stReceiptValue += stValue.Substring(10, 2);
                            }


                            if (getTagValue(stResponse, 7, out stValue) == 0)
                            {
                                // Transaction Date
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";
                                TransactionDate = stValue;//WTR_HGM
                            }

                            if (getTagValue(stResponse, 14, out stValue) == 0)
                            {
                                // Expiry Date
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";
                                ExpDate = stValue;//WTR_HGM
                            }

                            if (getTagValue(stResponse, 22, out stValue) == 0)
                            {
                                EntryMode = stValue; //WTR_HGM

                                stReceiptValue += "ENTRY MODE: ";
                                if (stValue[0] == 'E')
                                {
                                    stReceiptValue += "MANUAL ENTRY";
                                    CardEntryStatus = "MANUAL ENTRY";//WTR_HGM
                                }
                                else if (stValue[0] == 'M')
                                {
                                    stReceiptValue += "MAGSTRIPE";
                                    CardEntryStatus = "MAGSTRIPE";//WTR_HGM
                                }
                                else if (stValue[0] == 'F')
                                {
                                    stReceiptValue += "FALLBACK";
                                    CardEntryStatus = "FALLBACK";//WTR_HGM
                                }
                                else if (stValue[0] == 'C')
                                {
                                    stReceiptValue += "CHIP";
                                    CardEntryStatus = "CHIP";//WTR_HGM
                                }
                                else if (stValue[0] == 'P')
                                {
                                    stReceiptValue += "CONTACTLESS";
                                    CardEntryStatus = "CONTACTLESS";//WTR_HGM
                                }
                                else
                                {
                                    stReceiptValue += stValue;
                                    CardEntryStatus = stValue;//WTR)MAM
                                }
                                stReceiptValue += "\r\n";
                            }

                            if (getTagValue(stResponse, 37, out stValue) == 0)
                            {
                                stReceiptValue += "RRN: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";

                                RetrievalReferenceNumber = stValue;//WTR_HGM
                            }

                            if (getTagValue(stResponse, 38, out stValue) == 0)
                            {
                                stReceiptValue += "APPROVAL CODE: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";

                                ApprovalCode = stValue;//WTR_HGM
                            }

                            if (getTagValue(stResponse, 39, out stValue) == 0)
                            {
                                stReceiptValue += "RESPONSE CODE: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";
                                ResponseCode = stValue;//WTR_HGM
                                ResponseMessage = MapResponseCode(stResponseCode);
                            }

                            if (getTagValue(stResponse, 41, out stValue) == 0)
                            {
                                getTagValue(stResponse, 41, out stValue);
                                stReceiptValue += "TID: ";
                                stReceiptValue += stValue;
                                TerminalID = stValue;//WTR_HGM
                                stReceiptValue += "  ";
                            }

                            if (getTagValue(stResponse, 42, out stValue) == 0)
                            {
                                getTagValue(stResponse, 42, out stValue);
                                stReceiptValue += "MID: ";
                                stReceiptValue += stValue;
                                MerchantID = stValue;//WTR_HGM
                                stReceiptValue += "\r\n";
                            }


                            getTagValue(stResponse, 52, out stValue);
                            stReceiptValue += "HOST: ";
                            stReceiptValue += stValue;
                            Host = stValue;//WTR_HGM
                            stReceiptValue += "\r\n";


                            if (getTagValue(stResponse, 53, out stValue) == 0)
                            {

                                stReceiptValue += "APP LABEL: ";
                                stReceiptValue += stValue.Substring(30, 16);
                                ApplicationLabel = stValue.Substring(30, 16);//WTR_HGM
                                stReceiptValue += "\r\n";

                                stReceiptValue += "AID: ";
                                stReceiptValue += stValue.Substring(14, 16);
                                AID = stValue.Substring(14, 16);//WTR_HGM
                                stReceiptValue += "\r\n";

                                stReceiptValue += "TVR TSI: ";
                                stReceiptValue += stValue.Substring(0, 10);
                                TVRTSI = stValue.Substring(0, 10) + " " + stValue.Substring(10, 4);//WTR_HGM;
                                stReceiptValue += " ";
                                stReceiptValue += stValue.Substring(10, 4);
                                stReceiptValue += "\r\n";

                                stReceiptValue += "TC: ";
                                stReceiptValue += stValue.Substring(46, 16);
                                TC = stValue.Substring(46, 16);//WTR_HGM
                                stReceiptValue += "\r\n";

                            }
                            //stReceiptValue += "\r\n";
                            if (getTagValue(stResponse, 54, out stValue) == 0)
                            {
                                // card label
                                stReceiptValue += stValue;
                                stReceiptValue += ": ";
                                CardLabel = stValue;//WTR_HGM
                            }

                            getTagValue(stResponse, 55, out stValue);
                            stReceiptValue += "Card Type: ";
                            stReceiptValue += stValue;
                            CardType = stValue;//WTR_HGM
                            stReceiptValue += "\r\n";
                            //stReceiptValue += "\r\n";    

                            getTagValue(stResponse, 56, out stValue);
                            stReceiptValue += "Host Type: ";
                            stReceiptValue += stValue;
                            HostType = stValue;//WTR_HGM
                            stReceiptValue += "\r\n";
                            // Command Identifier
                            CommandIdentifier = stValue;//WTR_HGM

                            if (getTagValue(stResponse, 58, out stValue) == 0)
                            {
                                stReceiptValue += "Custom Data 2: ";
                                stReceiptValue += stValue;
                                CustomData2 = stValue;
                            }

                            if (getTagValue(stResponse, 59, out stValue) == 0)
                            {
                                stReceiptValue += "Custom Data 3: ";
                                stReceiptValue += stValue;
                                CustomData3 = stValue;
                            }

                            if (getTagValue(stResponse, 61, out stValue) == 0)
                            {
                                stReceiptValue += "UTRN: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "\r\n";

                                UTRN = stValue;
                            }

                            if (getTagValue(stResponse, 62, out stValue) == 0)
                            {
                                stReceiptValue += "INVOICE: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "  ";

                                InvoiceNumber = stValue;
                            }

                            if (getTagValue(stResponse, 63, out stValue) == 0)
                            {
                                stReceiptValue += "TRANSACTION INFO: ";
                                stReceiptValue += stValue;
                                stReceiptValue += "  ";

                                TransactionInfo = stValue;
                            }

                            if (getTagValue(stResponse, 64, out stValue) == 0)
                            {
                                stReceiptValue += "BATCH: ";
                                stReceiptValue += stValue;

                                BatchNumber = stValue;//WTR_HGM
                            }

                            if (String.Compare(stResponse, 0, "R971", 0, 4) == 0 &&
                                getTagValue(stResponse, 65, out stValue) == 0)
                            {
                                stReceiptValue += "BATCHINFO: ";
                                stReceiptValue += stValue;

                                BatchInformation = stValue;//WTR_HGM

                                CCSettlementResponse singleCardResponse = new CCSettlementResponse();
                                singleCardResponse.ResponseCode = ResponseCode;
                                singleCardResponse.TerminalID = TerminalID;
                                singleCardResponse.MerchantID = MerchantID;
                                singleCardResponse.CardLabel = CardLabel;
                                singleCardResponse.CardType = CardType;
                                singleCardResponse.Host = Host;
                                singleCardResponse.HostType = HostType;
                                singleCardResponse.BatchNumber = BatchNumber;
                                singleCardResponse.BatchInformation = BatchInformation;
                                CCSettleResponseList.Add(singleCardResponse);
                            }

                            if (getTagValue(stResponse, 98, out stValue) == 0)
                            {
                                stReceiptValue += "Card Holders Name: ";
                                stReceiptValue += stValue;

                                CardHolderName = stValue;//WTR_HGM
                            }
                            stReceiptValue += "\r\n";
                            stReceiptValue += "\r\n";
                            stReceiptValue += "\r\n";
                            stReceiptValue += "\r\n";

                            Log(stReceiptValue);
                            SetControlPropertyThreadSafe(textBoxReceipt, "Text", stReceiptValue);
                        }
                        else
                        {
                            ResponseCode = stResponseCode;
                            ResponseMessage = MapResponseCode(stResponseCode);
                            //SetControlPropertyThreadSafe(label5, "Text", "TRANSACTION FAILED!\nERROR CODE=" + stResponseCode);
                            //label5.Text = "TRANSACTION FAILED!\nERROR CODE=" + stResponseCode;
                        }

                    }
                }
                else if (textBoxResponse.Text.Length != 0 &&
                    (String.Compare(stResponse, 0, "R900", 0, 3) == 0))
                {
                    string stTerminalInfoValue;
                    stTerminalInfoValue = stResponse.Substring(4);
                    stTerminalInfoValue += "\r\n";
                    stTerminalInfoValue += "\r\n";
                    Log(stTerminalInfoValue);
                    SetControlPropertyThreadSafe(textBoxReceipt, "Text", stTerminalInfoValue);
                }
                else if (String.Compare(stResponse, 0, "R97", 0, 3) == 0)
                {
                    string stTerminalInfoValue;
                    stTerminalInfoValue = stResponse.Substring(39);
                    stTerminalInfoValue += "\r\n";
                    stTerminalInfoValue += "\r\n";
                    Log(stTerminalInfoValue);
                    SetControlPropertyThreadSafe(textBoxReceipt, "Text", stTerminalInfoValue);
                }
            }
        }

        private string MapResponseCode(string stResponseCode)
        {
            string responseMessage = string.Empty;
            string responseAction = string.Empty;
            switch (stResponseCode)
            {
                case "01":
                    responseMessage = "REFER TO CARD ISSUER";
                    responseAction = "CALL BANK";
                    break;
                case "02":
                    responseMessage = "REFER TO CARD ISSUER’S SPECIAL CONDITION";
                    responseAction = "CALL BANK";
                    break;
                case "03":
                    responseMessage = "ERROR CALL HELP SN";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "05":
                    responseMessage = "DO NOT HONOUR";
                    responseAction = "CALL BANK";
                    break;
                case "12":
                    responseMessage = "INVALID TRANSACTION (HELP TR)";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "13":
                    responseMessage = "INVALID AMOUNT (HELP AM)";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                // WTR RAD START
                case "14":
                    responseMessage = "INVALID CARD NUMBER (HELP RE)";
                    responseAction = "CHECK CARD#";
                    break;
                case "19":
                    responseMessage = "RE-ENTER TRANSACTION";
                    responseAction = "RESUBMIT ";
                    break;
                case "25":
                    responseMessage = "UNABLE TO LOCATE RECORD ON FILE (HELP NT)";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "30":
                    responseMessage = "FORMAT ERROR (HELP FE)";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "31":
                    responseMessage = "BANK NOT SUPPORTED BY SWITCH (HELP NS)";
                    responseAction = "CALL BANK";
                    break;
                case "41":
                    responseMessage = "LOST CARD";
                    responseAction = "CALL BANK";
                    break;
                case "43":
                    responseMessage = "STOLEN CARD PICKUP";
                    responseAction = "CALL BANK";
                    break;
                case "51":
                    responseMessage = "TRANSACTION DECLINED";
                    responseAction = "CALL BANK";
                    break;
                case "54":
                    responseMessage = "EXPIRED CARD";
                    responseAction = "CHECK CARD EXPIRY OR USE DIFFERENT CARD";
                    break;
                case "55":
                    responseMessage = "INCORRECT PIN";
                    responseAction = "REENTER PIN";
                    break;
                case "76":
                    responseMessage = "INVALID PRODUCT CODES (HELP DC)";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "77":
                    responseMessage = "RECONCILE ERROR";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "78":
                    responseMessage = "TRACE# NOT FOUND";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "89":
                    responseMessage = "BAD TERMINAL ID";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "91":
                    responseMessage = "ISSUER/SWITCH INOPERATIVE";
                    responseAction = "CALL BANK";
                    break;
                case "94":
                    responseMessage = "DUPLICATE TRANSMISSION";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "96":
                    responseMessage = "SYSTEM MALFUNCTION";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "SE":
                    responseMessage = "TERMINAL FULL";
                    responseAction = "INITIATE SETTLEMENT";
                    break;
                case "PE":
                    responseMessage = "PIN ENTRY ERROR";
                    responseAction = "REDO TRANSACTION";
                    break;
                case "IC":
                    responseMessage = "INVALID CARD";
                    responseAction = "CARD IS NOT SUPPORTED BY THIS TERMINAL, USE A DIFFERENT CARD";
                    break;
                case "EC":
                    responseMessage = "CARD IS EXPIRED";
                    responseAction = "USE A DIFFERENT CARD";
                    break;
                case "CE":
                    responseMessage = "CONNECTION ERROR";
                    responseAction = "PLEASE RETRY";
                    break;
                case "RE":
                    responseMessage = "RECORD NOT FOUND";
                    responseAction = "CHECK INVOICE NUMBER / TRACE";
                    break;
                case "HE":
                    responseMessage = "WRONG HOST NUMBER PROVIDED";
                    responseAction = "CHECK HOST #";
                    break;
                case "LE":
                    responseMessage = "LINE ERROR";
                    responseAction = "CHECK PHONE LINE";
                    break;
                case "VB":
                    responseMessage = "TRANSACTION ALREADY VOIDED";
                    responseAction = "";
                    break;
                case "FE":
                    responseMessage = "FILE EMPTY / NO TRANSACTION TO VOID ";
                    responseAction = "";
                    break;
                case "WC":
                    responseMessage = "CARD NUMBER DOES NOT MATCH";
                    responseAction = "CHECK CARD #";
                    break;
                case "TA":
                    responseMessage = "TRANSACTION ABORTED BY USER";
                    responseAction = "";
                    break;
                case "AE":
                    responseMessage = "AMOUNT DID NOT MATCH";
                    responseAction = "CHECK AMOUNT";
                    break;
                case "XX":
                    responseMessage = "TERMINAL NOT PROPERLY SETUP";
                    responseAction = "CONTACT TERMINAL VENDOR";
                    break;
                case "DL":
                    responseMessage = "LOGON NOT DONE";
                    responseAction = "DO LOGON";
                    break;
                case "BL":
                    responseMessage = "BAD TLV COMMAND FORMAT";
                    responseAction = "PLEASE CHECK YOUR COMMAND PACKET FORMAT.";
                    break;
                case "IS":
                    responseMessage = "TRANSACTION NOT FOUND, INQUIRY SUCCESSFUL";
                    responseAction = "INQUIRY SUCCESSFUL, YOU MAY CONTINUE TO START A NEW TRANSACTION";
                    break;
                case "CD":
                    responseMessage = "CARD DECLINED TRANSACTION";
                    responseAction = "PLEASE TRY AGAIN, TRY NOT TO REMOVE CARD FROM TERMINAL WHILE TRANSACTION IS PROCESSING. PLEASE PROMPT CUSTOMER TO ENTER PIN IF REQUIRED.";
                    break;
                case "LH":
                    responseMessage = "LOYALTY HOST IS TEMPORARY OFFLINE.";
                    responseAction = "REBATES WILL ONLY BE ISSUED THE FOLLOWING DAY. CUSTOMER CANNOT REDEEM FOR NOW.";
                    break;
                case "IN":
                    responseMessage = "INVALID CARD";
                    responseAction = "CEPAS CARD IS REFUNDED or BLACKLISTED";
                    break;
                case "CO":
                    responseMessage = "CARD NOT READ PROPERLY";
                    responseAction = "CARD NOT READ PROPERLY TRY AGAIN";
                    break;
                case "TL":
                    responseMessage = "TOP UP LIMIT EXCEEDED";
                    responseAction = "";
                    break;
                case "PL":
                    responseMessage = "PAYMENT LIMIT EXCEEDED";
                    responseAction = "";
                    break;
                case "CT":
                    responseMessage = "TERMINAL IS TIMEOUT";
                    responseAction = "";
                    break;
                case "FS":
                    responseMessage = "FALLBACK";
                    responseAction = "USE MAGNETICSTRIPE";
                    break;

                    //WTR RAD END 


            }

            // WTR RAD
            logHelper.saveLog("[RESPONSE CODE] :  " + stResponseCode + " [ResponseMessage is]  " + responseMessage + ":" + responseAction);
            //END

            return responseMessage + ":" + responseAction;
        }

        private static void Log(string logMessage)
        {
            string fileName = @"C:\IngenicoLog.txt";
            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(logMessage);
            }
        }

        #endregion

    }

}
