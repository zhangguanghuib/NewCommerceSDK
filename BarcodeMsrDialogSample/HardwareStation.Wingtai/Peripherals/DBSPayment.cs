using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace WTR.HWExt.Peripherals
{
    /*
    *	Tag-Length-Value element definition
    */
    public struct TLV
    {
        //	Number code that indicate the type of value
        public readonly byte[] Code;
        public int Tag
        {
            get { return ((Code[0] - (byte)'0') * 10) + (Code[1] - (byte)'0'); }
        }

        // Length of the value
        public readonly byte[] Size;
        public int Length
        {
            get { return Data.Length; }
        }

        // Value 
        public readonly byte[] Data;
        public string Text
        {
            get
            {
                int len = Data.Length;
                System.Text.StringBuilder sb = new System.Text.StringBuilder(len);
                for (int i = 0; i < len; i++) sb.Append((char)Data[i]);
                return sb.ToString();
            }
        }
        public int Value
        {
            get
            {
                int value = 0;
                for (int i = 0; i < Data.Length; i++) value = (value * 10) + (Data[i] - (byte)'0');
                return value;
            }
        }
        public double Amount
        {
            get
            {
                int value = 0;
                for (int i = 0; i < Data.Length; i++) value = (value * 10) + (Data[i] - (byte)'0');
                return (double)value / 100.00;
            }
        }

        // Constructors
        public TLV(int tag, double value) : this(tag, 12, (int)(value * 100.00)) { }
        public TLV(int tag, int len, double value) : this(tag, len, (int)(value * 100.00)) { }
        public TLV(int tag, int len, int value)
        {
            Code = new byte[2];
            Size = new byte[2];
            Data = new byte[len];

            Code[0] = (byte)('0' + ((tag / 10) & 0xff));
            Code[1] = (byte)('0' + ((tag % 10) & 0xff));
            Size[0] = (byte)('0' + ((len / 10) & 0xff));
            Size[1] = (byte)('0' + ((len % 10) & 0xff));
            for (int i = 1; i <= len; i++)
            {
                Data[len - i] = (byte)('0' + (value % 10));
                value = value / 10;
            }
        }
        public TLV(int tag, string text)
        {
            int len = text.Length;
            Code = new byte[2];
            Size = new byte[2];
            Data = new byte[len];

            Code[0] = (byte)('0' + ((tag / 10) & 0xff));
            Code[1] = (byte)('0' + ((tag % 10) & 0xff));
            Size[0] = (byte)('0' + ((len / 10) & 0xff));
            Size[1] = (byte)('0' + ((len % 10) & 0xff));
            for (int i = 0; i < len; i++) Data[i] = (byte)text[i];
        }
        public TLV(string s) : this(s, 0) { }
        public TLV(string s, int offset)
        {
            int len = ((byte)(s[offset + 2] - '0') * 10) + (byte)(s[offset + 3] - '0');
            Code = new byte[2];
            Size = new byte[2];
            Data = new byte[len];

            Code[0] = (byte)(s[offset]);
            Code[1] = (byte)(s[offset + 1]);
            Size[0] = (byte)(s[offset + 2]);
            Size[1] = (byte)(s[offset + 3]);
            for (int i = 0; i < len; i++) Data[i] = (byte)(s[offset + 4 + i]);
        }

        public TLV(byte[] tag, byte[] size, byte[] value)
        {
            Code = new byte[tag.Length];
            Size = new byte[size.Length];
            Data = new byte[value.Length];

            Code = tag;
            Size = size;
            Data = value;
        }
        /// <summary>
        /// Split a string into list of TLV
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static System.Collections.Generic.List<TLV> Split(string s)
        {
            //"0216XXXXXXXXXXXX98260412000000000006071002111706221404XXXX2201C3712487106206212380640096839020041084123456742121681681234565203DBS53620000008000E800A000000004101000MasterCard      A45A1B9D99C2B1315410MASTERCARD5501M5601D612000115021105025800001620600005064060000039602119826EDY PANYUN                "
            System.Collections.Generic.List<TLV> vs = new System.Collections.Generic.List<TLV>();

            try
            {
                string ss = s;
                while (ss.Length > 4)
                {
                    int tag = 0;
                    int len = 0;
                    string value = string.Empty;
                    bool isOK = false;

                    isOK = Int32.TryParse(ss.Substring(0, 2), out tag);
                    if (isOK)
                    {
                        isOK = Int32.TryParse(ss.Substring(2, 2), out len);
                    }
                    if (isOK)
                    {
                        value = ss.Substring(4, len);

                        TLV v = new TLV(GetBytes(ss.Substring(0, 2)), GetBytes(ss.Substring(2, 2)), GetBytes(value));
                        vs.Add(v);

                        if (ss.Length > 4 + value.Length)
                            ss = ss.Substring(4 + value.Length);
                        else
                            ss = string.Empty;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }

            return vs;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            return buffer;
        }
    }

    /*
    *	Serial device definition
    */
    public class DBS_ECR
    {
        public string COMMAND_SALE = "C200";
        public string COMMAND_PREAUTH = "C201";
        public string COMMAND_PREAUTH_CAPTURE_OFFLINE = "C202";
        public string COMMAND_REFUND = "C203";
        public string COMMAND_CASH_ADVANCE = "C204";
        public string COMMAND_VOID = "C300";
        public string COMMAND_INQUIRY = "C400";
        public string COMMAND_ECHO = "C902";
        public string COMMAND_BEGIN_SHIFT = "C800";
        public string COMMAND_TIP_ADJUSTMENT = "C500";
        public string COMMAND_SETTLEMENT = "C700";
        public string COMMAND_NETS_FLASHPAY = "C610";

        public int AnswerTimeout = 1500;				// answer timeout, default 1500ms
        public int CharacterTimout = 50;				// inter character timeout, default 50ms
        public int MessageTimeout = 6000;				// message timeout, default 6000ms
        public int EnqiryDelay = 1000;					// read attempt delay for ENQ, default 500ms - 1000ms
        public int SessionDelay = 1000;					// read attempt delay for no ACK, default 1000ms
        public int CommandTimeout = 61000;				// command timeout, default 61000ms

        protected const byte ENQ = 0x05;
        protected const byte ACK = 0x06;
        protected const byte NACK = 0x15;
        protected const byte EOT = 0x04;
        protected const byte STX = 0x02;
        protected const byte ETX = 0x03;

        public enum TagID
        {
            CardNumber = 2,
            TransactionAmount = 4,
            AvailableBalance = 5,
            DateTime = 7,
            ExpiryDate = 14,
            EntryMode = 22,
            CardData = 35,
            RetrievalReference = 37,
            ApprovalCode = 38,
            ResponseCode = 39,
            TerminalID = 41,
            MerchantID = 42,
            HostLabel = 52,
            EMVData = 53,
            CardLabel = 54,
            Cardtype = 55,
            Hosttype = 56,
            CommandIdentifier = 57,
            CustomData2 = 58,
            CustomData3 = 59,
            OriginalUniqueTraceReference = 60,
            ECRUniqueTraceReference = 61,
            InvoiceNumber = 62,
            TransactionInfo = 63,
            BatchNumber = 64,
            Hostbitmap = 65,
            CouponsVouchers = 66,
            AdditionalPrintingFlags = 96,
            POSInvoiceAdditionalInfo = 97,
            CardHolderName = 98,
            EmployeeID = 99
        }

        /*
        *	Command definition
        */
        public class Command
        {
            public string Code;
            public TLV[] Values;

            /// <summary>
            /// Convert command into array of bytes
            /// </summary>
            /// <returns></returns>
            public byte[] GetBytes()
            {
                if (Values == null || Code == null)
                    return new byte[0];

                //Declare
                int len = 0;                                        // declare len
                for (int i = 0; i < Values.Length; i++) len += Values[i].Length + 4; //Get len value
                byte[] buf = new byte[len + 7];						// declare buf after get the len value -> STX(1) + HEADER(4) + len + ETX(1) + LRC(1)
                int idx = 1;                                        //start from 1 to exclude the STX 

                // STX
                buf[0] = STX;

                //Command
                for (int i = 0; i < Code.Length; i++) buf[idx++] = (byte)Code[i];

                //Messages
                foreach (TLV v in Values)
                {
                    for (int i = 0; i < 2; i++) buf[idx++] = v.Code[i];
                    for (int i = 0; i < 2; i++) buf[idx++] = v.Size[i];
                    for (int i = 0; i < v.Data.Length; i++) buf[idx++] = v.Data[i];
                }

                //ETX
                buf[idx++] = ETX;

                //LRC
                buf[idx] = Compute_LRC(buf);

                return buf;
            }

            public static byte Compute_LRC(byte[] bytes, bool includeLRC = true)
            {
                //get LRC value (LRC is calculated XORing the entire packet excluding STX, including ETX. The LRC value can range from 0x00 to 0xFF.)
                byte LRC = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (i == 0 && bytes[0] == STX)
                        continue;

                    if (includeLRC && i >= bytes.Length - 2) //exclude etx and lrc
                        continue;

                    LRC ^= bytes[i];
                }
                LRC ^= ETX;
                return LRC;
            }
        }
    }


    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("E807E897-209C-4E83-9885-96E0A940E2A4")]
    [ProgId("PaymentDevice.DBSPayment")]
    [ComDefaultInterface(typeof(IDBSPayment))]
    public class DBSPayment : DBS_ECR, IDBSPayment//, IObjectSafety
    {
        private string approvalCode;
        private string retrievalReferenceNumber;
        private string invoiceNo;
        private string transactionDate;
        private string transactionTime;
        private string transactionInfo;
        private string cardLabel;
        private string eftTerminalID;
        private string eftCardNumber;
        private string cardHolderName;
        private string outputMessage;
        private string expiryDate;
        private string entryMode;
        private string merchantID;
        private string hostLabel;
        private string eMVData;
        private string cardtype;
        private string hosttype;
        private string eCRUniqueTraceReference;
        private string batchNumber;
        private string additionalPrintingFlags;
        private string responseByteMessage;

        public System.IO.Ports.SerialPort Device;
        string indata = string.Empty;
        int commandIdentifierNo = 1;

        protected void InitDevice(string name)
        {
            Device = new System.IO.Ports.SerialPort(name, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            Device.Handshake = System.IO.Ports.Handshake.None;
        }

        protected void Open()
        {
            if (Device.IsOpen)
            {
                Close();
            }

            Device.Open();
        }

        protected void Close()
        {
            if (Device != null)
                Device.Close();
        }

        protected Command ReadCommand(out int Received)
        {
            Received = 1;
            byte[] ack = new byte[] { ACK };
            byte[] nack = new byte[] { NACK };
            string incomingMessageString = string.Empty;

            for (int i = 1; i < 4; i++)
            {
                SaveLogFile("Read Command Loop", i.ToString());
                try
                {
                    Device.ReadTimeout = CommandTimeout;
                    byte b = (byte)Device.ReadByte();
                    if (b == ENQ)
                    {
                        Device.ReadTimeout = AnswerTimeout;
                        Device.Write(ack, 0, ack.Length);
                        SaveLogFile("ENQ ACK SUCCESS! ", "");

                        Device.ReadTimeout = CommandTimeout;
                        incomingMessageString = Device.ReadTo(((char)ETX).ToString());
                        SaveLogFile("Read Respond", incomingMessageString);

                        if (!incomingMessageString.StartsWith(((char)STX).ToString()))
                        {
                            string[] respondmsg = incomingMessageString.Split((char)STX);
                            if (respondmsg.Length == 2 && respondmsg[1].Length > 4)
                                incomingMessageString = ((char)STX).ToString() + respondmsg[1];

                            SaveLogFile("Read Respond start from STX", incomingMessageString);
                        }

                        if (incomingMessageString != string.Empty)
                        {
                            b = (byte)Device.ReadByte();
                            if (b != EOT)
                            {
                                //check lrc
                                byte[] incomingMessages1 = GetBytes(incomingMessageString);
                                byte lrc = Command.Compute_LRC(incomingMessages1, false);

                                SaveLogFile("GetResponse in Bytes", ByteArrayToString(incomingMessages1));
                                SaveLogFile("LRC RECEIVED! ", b.ToString());
                                SaveLogFile("CALCULATED LRC is ", lrc.ToString());

                                if (b != lrc)
                                {
                                    SaveLogFile("Send Nack ", i.ToString());
                                    Device.ReadTimeout = AnswerTimeout;
                                    Device.Write(nack, 0, nack.Length);

                                    incomingMessageString = string.Empty;
                                }
                                else
                                {
                                    SaveLogFile("Send Ack ", i.ToString());
                                    Device.ReadTimeout = AnswerTimeout;
                                    Device.Write(ack, 0, ack.Length);

                                    Device.ReadTimeout = CommandTimeout;
                                    b = (byte)Device.ReadByte(); //retrieve EOT
                                }
                            }
                            if (b == EOT)
                            {
                                SaveLogFile("EOT RECEIVED! ", "");
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(EnqiryDelay);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                    System.Threading.Thread.Sleep(SessionDelay);
                }

                if (incomingMessageString != string.Empty)
                {
                    byte[] allresponse = GetBytes(incomingMessageString);
                    if (allresponse != null)
                        responseByteMessage = ByteArrayToString(allresponse);


                    string[] messages = incomingMessageString.Split((char)STX);
                    if (messages.Length == 2 && messages[1].Length > 4)
                    {
                        incomingMessageString = messages[1];
                        byte[] incomingMessages = GetBytes(incomingMessageString);

                        Command cmd = new Command();
                        cmd.Code = incomingMessageString.Substring(0, 4);
                        cmd.Values = TLV.Split(incomingMessageString.Substring(4)).ToArray();
                        Received = 0;
                        return cmd;
                    }
                }
            }
            return null;
        }

        protected bool SendCommand(Command cmd)
        {
            byte[] enq = new byte[] { ENQ };
            byte[] eot = new byte[] { EOT };
            byte[] buf = cmd.GetBytes();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    SaveLogFile("-------- DBS SendCommand ENQ LOOP " + i.ToString() + " ----------\n", "");
                    Device.ReadTimeout = AnswerTimeout;
                    Device.Write(enq, 0, enq.Length);

                    byte b = (byte)Device.ReadByte();
                    if (b == ACK)
                    {
                        SaveLogFile("ENQ ACK SUCCESS! ", "");
                        SaveLogFile("Send Command", ByteArrayToString(buf));
                        Device.Write(buf, 0, buf.Length);

                        b = (byte)Device.ReadByte();
                        if (b == ACK)
                        {
                            SaveLogFile("COMMAND ACK SUCCESS! ", "");
                            return true;
                        }
                    }
                    if (b == NACK)
                    {
                        SaveLogFile("NACK RESPONSE RECEIVED! ", "");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                    try { Device.Write(eot, 0, eot.Length); }
                    catch { }
                    System.Threading.Thread.Sleep(SessionDelay);
                }
                Thread.Sleep(1950);
            }
            return false;
        }

        protected void SaveLogFile(object method, string inputstring)
        {
            LogHelper.Info(String.Format("{0} : {1}", method.ToString(), inputstring));
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        protected string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", " ");
        }

        #region Tag Number 57: Command identifier

        /*  Tag Number 57: Command identifier
        A 6 byte numeric identifier to trace every C20X, C300 and C500 commands entering the terminal. 
        The Command Identifier (Tag 57) is a running number starting from 000001. 
        Incremented for each C20X, C300 and C500 command.
        
        PC/Cash-registers are required to generate a new Command identifier (Tag 57) for
        every C20X, C300 and C500 command.
        During INQUIRY (C400), the Command Identifier (Tag 57) should be the same as the
        original command. 
         
        Example:
        1) PC/Cash-register sends a C200 command with Tag57=000001.
        2) But R200 response fails to reach PC/Cash-register.
        3) The upcoming C400 command must use the same Tag57 value (000001).
        The following table describes the need to generate a new command identifier (Tag
        57) for each of the transaction types.

        SALE: NEW
        PRE-AUTH: NEW
        PRE-AUTH CAPTURE: NEW
        REFUND: NEW
        CASH ADVANCE: NEW
        VOID: NEW
        INQUIRY: SAME VALUE AS ORGINAL COMMAND
        ECHO: NOT APPLICABLE
        BEGIN SHIFT: NOT APPLICABLE
        TIP ADJUSTMENT: NEW
        */
        #endregion

        #region Tag Number 61: ECR Unique Trace Reference
        /*  Tag Number 61: ECR Unique Trace Reference
        A number generated by PC/Cash-register to uniquely identify a transaction.
        PC/Cash-register must generate a new unique trace reference for every new SALE
        (C200), PRE-AUTH(C201), REFUND(C203) and CASH-ADVANCE(C204).

        Format: <3 bytes prefix> <YYMMDD> <hhmmss> <5 bytes counter>
        <3 bytes prefix>: The 3 bytes alphanumeric prefix value will be provided by DBS to every PC/Cash-register vendor.
        <YYMMDD>: Transaction date.
        <hhmmss>: Transaction time.
        <5 bytes counter>: A running number starting from 00001. Incremented during each
        transaction.

        SALE: NEW
        PRE-AUTH: NEW
        PRE-AUTH CAPTURE: SAME VALUE AS THE ORIGNAL PRE-AUTH (C201) TRANSACTION
        REFUND: NEW
        CASH ADVANCE: NEW
        VOID: SAME VALUE AS ORIGINAL TRANSACTION
        INQUIRY: SAME VALUE AS ORIGINAL TRANSACTION
        ECHO: NOT APPLICABLE
        BEGIN SHIFT: NOT APPLICABLE
        TIP ADJUSTMENT: SAME VALUE AS ORIGINAL TRANSACTION
        */
        #endregion

        public bool MakeDBSPayment(string amount, string COMPORT, string ECRUniqueTraceReference)
        {
            amount = amount.Replace(".", "").Replace(",", "");

            bool successFlag = false;
            string output = string.Empty;
            try
            {
                SaveLogFile("--DBS TRANSACTION START--", ECRUniqueTraceReference);
                SaveLogFile("MakeDBSPayment", "Input Amount is " + amount + " ::: Given COMPort is " + COMPORT);


                InitDevice(COMPORT);
                Device.Open();

                DBS_ECR.Command command = new DBS_ECR.Command();
                command.Code = COMMAND_SALE;

                TLV[] ts = new TLV[]
                {
                    new TLV( (int) DBS_ECR.TagID.TransactionAmount, amount ),
                    new TLV( (int) DBS_ECR.TagID.CommandIdentifier, commandIdentifierNo.ToString("000000")),
                    new TLV( (int) DBS_ECR.TagID.ECRUniqueTraceReference, ECRUniqueTraceReference)
                };
                command.Values = ts;

                bool sucks = false;
                if (SendCommand(command))
                {
                    sucks = true;

                    int Received;
                    Command result = ReadCommand(out Received);

                    if (Received == 0 && result != null)
                    {
                        foreach (TLV tlv in result.Values)
                        {
                            if (tlv.Tag == (int)DBS_ECR.TagID.ResponseCode)
                            {
                                if (tlv.Text == "00")
                                {
                                    SaveLogFile("MakeDBSPayment", "Transaction Approved!");
                                    successFlag = true;

                                    foreach (TLV tlvResponseData in result.Values)
                                    {
                                        switch (tlvResponseData.Tag)
                                        {
                                            case (int)DBS_ECR.TagID.CardNumber:
                                                eftCardNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.DateTime:
                                                transactionDate = tlvResponseData.Text.Substring(2, 2) + "/" + tlvResponseData.Text.Substring(0, 2) + "/" + DateTime.Today.Year.ToString();
                                                transactionTime = tlvResponseData.Text.Substring(4, 6);
                                                break;
                                            case (int)DBS_ECR.TagID.ApprovalCode:
                                                approvalCode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TransactionInfo:
                                                transactionInfo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.RetrievalReference:
                                                retrievalReferenceNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardHolderName:
                                                cardHolderName = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TerminalID:
                                                eftTerminalID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardLabel:
                                                cardLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.InvoiceNumber:
                                                invoiceNo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ExpiryDate:
                                                expiryDate = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EntryMode:
                                                entryMode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.MerchantID:
                                                merchantID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.HostLabel:
                                                hostLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EMVData:
                                                eMVData = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Cardtype:
                                                cardtype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Hosttype:
                                                hosttype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ECRUniqueTraceReference:
                                                eCRUniqueTraceReference = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.BatchNumber:
                                                batchNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.AdditionalPrintingFlags:
                                                additionalPrintingFlags = tlvResponseData.Text;
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                }
                                else
                                {
                                    outputMessage = GetErrorMessage(tlv.Text);
                                    SaveLogFile("MakeDBSPayment", "Response Code:" + tlv.Text);
                                }

                                break;
                            }
                        }
                    }
                }
                if (!sucks)
                {
                    successFlag = false;
                    outputMessage = "Unable to send command to EDC Terminal";
                }
            }
            catch (TimeoutException tex)
            {
                successFlag = false; // make it to 2 also to identify the timeout
                Close();

                LogHelper.Error(tex);
                SaveLogFile("MakeDBSPayment", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + tex.Message;
            }
            catch (Exception ex)
            {
                successFlag = false;
                Close();

                LogHelper.Error(ex);
                SaveLogFile("MakeDBSPayment", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + ex.Message;
            }
            finally
            {
                Close();
            }
            SaveLogFile("MakeDBSPayment", "Output Message: " + outputMessage);
            SaveLogFile("--DBS TRANSACTION END--", ECRUniqueTraceReference);
            return successFlag;
        }


        public bool MakeNetsFlashPay(string amount, string COMPORT, string ECRUniqueTraceReference)
        {
            amount = amount.Replace(".", "").Replace(",", "");

            bool successFlag = false;
            string output = string.Empty;
            try
            {
                SaveLogFile("--DBS TRANSACTION START--", ECRUniqueTraceReference);
                SaveLogFile("MakeNetsFlashPay", "Input Amount is " + amount + " ::: Given COMPort is " + COMPORT);


                InitDevice(COMPORT);
                Device.Open();

                DBS_ECR.Command command = new DBS_ECR.Command();
                command.Code = COMMAND_NETS_FLASHPAY;

                TLV[] ts = new TLV[]
                {
                    new TLV( (int) DBS_ECR.TagID.TransactionAmount, amount ),
                    new TLV( (int) DBS_ECR.TagID.CommandIdentifier, commandIdentifierNo.ToString("000000")),
                    new TLV( (int) DBS_ECR.TagID.ECRUniqueTraceReference, ECRUniqueTraceReference)
                };
                command.Values = ts;

                bool sucks = false;
                if (SendCommand(command))
                {
                    sucks = true;

                    int Received;
                    Command result = ReadCommand(out Received);

                    if (Received == 0 && result != null)
                    {
                        foreach (TLV tlv in result.Values)
                        {
                            if (tlv.Tag == (int)DBS_ECR.TagID.ResponseCode)
                            {
                                if (tlv.Text == "00")
                                {
                                    SaveLogFile("MakeNetsFlashPay", "Transaction Approved!");
                                    successFlag = true;

                                    foreach (TLV tlvResponseData in result.Values)
                                    {
                                        switch (tlvResponseData.Tag)
                                        {
                                            case (int)DBS_ECR.TagID.CardNumber:
                                                eftCardNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.DateTime:
                                                transactionDate = tlvResponseData.Text.Substring(2, 2) + "/" + tlvResponseData.Text.Substring(0, 2) + "/" + DateTime.Today.Year.ToString();
                                                transactionTime = tlvResponseData.Text.Substring(4, 6);
                                                break;
                                            case (int)DBS_ECR.TagID.ApprovalCode:
                                                approvalCode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TransactionInfo:
                                                transactionInfo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.RetrievalReference:
                                                retrievalReferenceNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardHolderName:
                                                cardHolderName = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TerminalID:
                                                eftTerminalID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardLabel:
                                                cardLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.InvoiceNumber:
                                                invoiceNo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ExpiryDate:
                                                expiryDate = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EntryMode:
                                                entryMode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.MerchantID:
                                                merchantID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.HostLabel:
                                                hostLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EMVData:
                                                eMVData = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Cardtype:
                                                cardtype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Hosttype:
                                                hosttype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ECRUniqueTraceReference:
                                                eCRUniqueTraceReference = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.BatchNumber:
                                                batchNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.AdditionalPrintingFlags:
                                                additionalPrintingFlags = tlvResponseData.Text;
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                }
                                else
                                {
                                    outputMessage = GetErrorMessage(tlv.Text);
                                    SaveLogFile("MakeNetsFlashPay", "Response Code:" + tlv.Text);
                                }

                                break;
                            }
                        }
                    }
                }
                if (!sucks)
                {
                    successFlag = false;
                    outputMessage = "Unable to send command to EDC Terminal";
                }
            }
            catch (TimeoutException tex)
            {
                successFlag = false; // make it to 2 also to identify the timeout
                Close();

                LogHelper.Error(tex);
                SaveLogFile("MakeNetsFlashPay", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + tex.Message;
            }
            catch (Exception ex)
            {
                successFlag = false;
                Close();

                LogHelper.Error(ex);
                SaveLogFile("MakeNetsFlashPay", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + ex.Message;
            }
            finally
            {
                Close();
            }
            SaveLogFile("MakeNetsFlashPay", "Output Message: " + outputMessage);
            SaveLogFile("--DBS TRANSACTION END--", ECRUniqueTraceReference);
            return successFlag;
        }

        public bool VoidDBSPayment(string amount, string COMPORT, string ECRUniqueTraceReference, string retrievalRefNo, string approvalCode, string invoiceNo)
        {
            commandIdentifierNo++;
            bool successFlag = false;
            string output = string.Empty;
            try
            {
                SaveLogFile("--DBS TRANSACTION START--", ECRUniqueTraceReference);
                SaveLogFile("VoidDBSPayment", "ECRUniqueTraceReference is " + ECRUniqueTraceReference + " ::: Given COMPort is " + COMPORT);


                InitDevice(COMPORT);
                Device.Open();

                DBS_ECR.Command command = new DBS_ECR.Command();
                command.Code = COMMAND_VOID;

                TLV[] ts = new TLV[]
                {
                    new TLV( (int) DBS_ECR.TagID.TransactionAmount, amount ),
                    new TLV( (int) DBS_ECR.TagID.CommandIdentifier, commandIdentifierNo.ToString("000000")),
                    new TLV( (int) DBS_ECR.TagID.ECRUniqueTraceReference, ECRUniqueTraceReference) ,
                    new TLV( (int) DBS_ECR.TagID.RetrievalReference, retrievalRefNo) ,
                    new TLV( (int) DBS_ECR.TagID.ApprovalCode, approvalCode) ,
                    new TLV( (int) DBS_ECR.TagID.InvoiceNumber, invoiceNo)
                };
                command.Values = ts;

                bool sucks = false;
                if (SendCommand(command))
                {
                    sucks = true;

                    int Received;
                    Command result = ReadCommand(out Received);

                    if (Received == 0 && result != null)
                    {
                        foreach (TLV tlv in result.Values)
                        {
                            if (tlv.Tag == (int)DBS_ECR.TagID.ResponseCode)
                            {
                                if (tlv.Text == "00")
                                {
                                    SaveLogFile("VoidDBSPayment", "Transaction Approved!");
                                    successFlag = true;

                                    foreach (TLV tlvResponseData in result.Values)
                                    {
                                        switch (tlvResponseData.Tag)
                                        {
                                            case (int)DBS_ECR.TagID.CardNumber:
                                                eftCardNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.DateTime:
                                                transactionDate = tlvResponseData.Text.Substring(2, 2) + "/" + tlvResponseData.Text.Substring(0, 2) + "/" + DateTime.Today.Year.ToString();
                                                transactionTime = tlvResponseData.Text.Substring(4, 6);
                                                break;
                                            case (int)DBS_ECR.TagID.ApprovalCode:
                                                approvalCode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TransactionInfo:
                                                transactionInfo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.RetrievalReference:
                                                retrievalReferenceNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardHolderName:
                                                cardHolderName = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.TerminalID:
                                                eftTerminalID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.CardLabel:
                                                cardLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.InvoiceNumber:
                                                invoiceNo = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ExpiryDate:
                                                expiryDate = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EntryMode:
                                                entryMode = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.MerchantID:
                                                merchantID = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.HostLabel:
                                                hostLabel = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.EMVData:
                                                eMVData = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Cardtype:
                                                cardtype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.Hosttype:
                                                hosttype = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.ECRUniqueTraceReference:
                                                eCRUniqueTraceReference = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.BatchNumber:
                                                batchNumber = tlvResponseData.Text;
                                                break;
                                            case (int)DBS_ECR.TagID.AdditionalPrintingFlags:
                                                additionalPrintingFlags = tlvResponseData.Text;
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                }
                                else
                                {
                                    outputMessage = GetErrorMessage(tlv.Text);
                                    SaveLogFile("VoidDBSPayment", "Response Code:" + tlv.Text);
                                }

                                break;
                            }
                        }
                    }

                }
                if (!sucks)
                {
                    successFlag = false;
                    outputMessage = "Unable to send command to EDC Terminal";
                }
            }
            catch (TimeoutException tex)
            {
                successFlag = false; // make it to 2 also to identify the timeout
                Close();

                LogHelper.Error(tex);
                SaveLogFile("VoidDBSPayment", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + tex.Message;
            }
            catch (Exception ex)
            {
                successFlag = false;
                Close();

                LogHelper.Error(ex);
                SaveLogFile("VoidDBSPayment", "Encounter Error, please check the log file");
                outputMessage = "Transaction is Failed! " + ex.Message;
            }
            finally
            {
                Close();
            }
            SaveLogFile("VoidDBSPayment", "Output Message: " + outputMessage);
            SaveLogFile("--DBS TRANSACTION END--", ECRUniqueTraceReference);
            return successFlag;
        }

        public string GetErrorMessage(string responseCode)
        {
            string desc = string.Empty;
            switch (responseCode)
            {
                case "01":
                    desc = "REFER TO CARD ISSUER";
                    return "CALL BANK! " + desc;
                case "02":
                    desc = "REFER TO CARD ISSUER’S SPECIAL CONDITION";
                    return "CALL BANK! " + desc;
                case "05":
                    desc = "DO NOT HONOUR";
                    return "CALL BANK! " + desc;
                case "31":
                    desc = "BANK NOT SUPPORTED BY SWITCH (HELP NS)";
                    return "CALL BANK! " + desc;
                case "41":
                    desc = "LOST CARD";
                    return "CALL BANK! " + desc;
                case "43":
                    desc = "STOLEN CARD PICK UP";
                    return "CALL BANK! " + desc;
                case "51":
                    desc = "TRANSACTION DECLINED";
                    return "CALL BANK! " + desc;
                case "91":
                    desc = "ISSUER/SWITCH INOPERATIVE";
                    return "CALL BANK! " + desc;
                case "03":
                    desc = "ERROR CALL HELP SN";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "12":
                    desc = "INVALID TRANSACTION (HELP TR)";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "13":
                    desc = "INVALID AMOUNT (HELP AM)";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "25":
                    desc = "UNABLE TO LOCATE RECORD ON FILE (HELP NT)";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "30":
                    desc = "FORMAT ERROR (HELP FE)";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "58":
                    desc = "TRANSACTION NOT PERMITTED IN TERMINAL";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "76":
                    desc = "INVALID PRODUCT CODES (HELP DC)";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "77":
                    desc = "RECONCILE ERROR";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "78":
                    desc = "TRACE# NOT FOUND";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "89":
                    desc = "BAD TERMINAL ID";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "94":
                    desc = "DUPLICATE TRANSMISSION";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "96":
                    desc = "SYSTEM MALFUNCTION";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "XX":
                    desc = "TERMINAL NOT PROPERLY SETUP";
                    return "CONTACT TERMINAL VENDOR! " + desc;
                case "CT":
                    desc = "";
                    return "TIMEOUT! TRY AGAIN!";
                case "14":
                    desc = "INVALID CARD NUMBER (HELP RE)";
                    return "CHECK CARD NUMBER! " + desc;
                case "WC":
                    desc = "CARD NUMBER DOES NOT MATCH";
                    return "CHECK CARD NUMBER! " + desc;
                case "19":
                    desc = "RE-ENTER TRANSACTION";
                    return "RESUBMIT! " + desc;
                case "54":
                    desc = "CHECK CARD EXPIRY OR USE DIFFERENT CARD";
                    return "EXPIRED CARD! " + desc;
                case "55":
                    desc = "INCORRECT PIN";
                    return "REENTER PIN! " + desc;
                case "SE":
                    desc = "TERMINAL FULL";
                    return "INITIATE SETTLEMENT! " + desc;
                case "PE":
                    desc = "PIN ENTRY ERROR";
                    return "REDO TRANSACTION! " + desc;
                case "IC":
                    desc = "INVALID CARD";
                    return "CARD IS NOT SUPPORTED BY THIS TERMINAL, USE A DIFFERENT CARD! " + desc;
                case "EC":
                    desc = "CARD IS EXPIRED";
                    return "USE A DIFFERENT CARD! " + desc;
                case "CE":
                    desc = "CONNECTION ERROR";
                    return "PLEASE RETRY! " + desc;
                case "RE":
                    desc = "RECORD NOT FOUND";
                    return "CHECK INVOICE NUMBER / TRAC! " + desc;
                case "HE":
                    desc = "WRONG HOST NUMBER PROVIDED";
                    return "CHECK HOST NUMBER! " + desc;
                case "LE":
                    desc = "LINE ERROR";
                    return "CHECK PHONE LINE! " + desc;
                case "VB":
                    desc = "TRANSACTION ALREADY VOIDED";
                    return desc;
                case "FE":
                    desc = "FILE EMPTY / NO TRANSACTION TO VOID";
                    return desc;
                case "TA":
                    desc = "TRANSACTION ABORTED BY USER";
                    return desc;
                case "TL":
                    desc = "TOP UP LIMIT EXCEEDED";
                    return desc;
                case "PL":
                    desc = "PAYMENT LIMIT EXCEEDED";
                    return desc;
                case "AE":
                    desc = "AMOUNT DID NOT MATCH";
                    return "CHECK AMOUNT! " + desc;
                case "DL":
                    desc = "LOGON NOT DONE";
                    return "DO LOGON! " + desc;
                case "BT":
                    desc = "BAD TLV COMMAND FORMAT";
                    return "PLEASE CHECK YOUR COMMAND PACKET FORMAT! " + desc;
                case "IS":
                    desc = "TRANSACTION NOT FOUND, INQUIRY SUCCESSFUL";
                    return "INQUIRY SUCCESSFUL, YOU MAY CONTINUE TO START A NEW TRANSACTION! " + desc;
                case "CD":
                    desc = "CARD DECLINED TRANSACTION";
                    return "PLEASE TRY AGAIN, TRY NOT TO REMOVE CARD FROM TERMINAL WHILE TRANSACTION IS PROCESSING. PLEASE PROMPT CUSTOMER TO ENTER PIN IF REQUIRED! " + desc;
                case "LH":
                    desc = "LOYALTY HOST IS TEMPORARY OFFLINE";
                    return "REBATES WILL ONLY BE ISSUED THE FOLLOWING DAY. CUSTOMER CANNOT REDEEM FOR NOW! " + desc;
                case "IN":
                    desc = "INVALID CARD";
                    return "EZLINK CARD IS REFUNDED or BLACKLISTED! " + desc;
                case "CO":
                    desc = "CARD NOT READ PROPERLY. TRY AGAIN!";
                    return desc;
                case "TN":
                    desc = "PLEASE ENSURE CORRECT CARD IS USED";
                    return desc;
                default:
                    return "UNKNOWN RESPONSE CODE! RESPONSE CODE IS " + responseCode;

            }
        }
        public string ApprovalCode()
        {
            return approvalCode;
        }

        public string InvoiceNo()
        {
            return invoiceNo;
        }

        public string RetrievalReferenceNumber()
        {
            return retrievalReferenceNumber;
        }

        public string TransactionDate()
        {
            return transactionDate;
        }

        public string TransactionTime()
        {
            return transactionTime;
        }

        public string CardHolderName()
        {
            return cardHolderName;
        }

        public string EFTCardNumber()
        {
            return eftCardNumber;
        }

        public string EFTTerminalID()
        {
            return eftTerminalID;
        }

        public string TransactionInfo()
        {
            return transactionInfo;
        }

        public string CardLabel()
        {
            return cardLabel;
        }

        public string OutputMessage()
        {
            return outputMessage;
        }

        public string ExpiryDate()
        {
            return expiryDate;
        }

        public string EntryMode()
        {
            return entryMode;
        }

        public string MerchantID()
        {
            return merchantID;
        }

        public string HostLabel()
        {
            return hostLabel;
        }

        public string EMVData()
        {
            return eMVData;
        }

        public string Cardtype()
        {
            return cardtype;
        }

        public string Hosttype()
        {
            return hosttype;
        }

        public string ECRUniqueTraceReference()
        {
            return eCRUniqueTraceReference;
        }

        public string BatchNumber()
        {
            return batchNumber;
        }

        public string AdditionalPrintingFlags()
        {
            return additionalPrintingFlags;
        }

        public string ResponseByteMessage()
        {
            return responseByteMessage;
        }



        #region IObjectSafety Members

        public enum ObjectSafetyOptions
        {
            INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001,
            INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002,
            INTERFACE_USES_DISPEX = 0x00000004,
            INTERFACE_USES_SECURITY_MANAGER = 0x00000008
        };

        public int GetInterfaceSafetyOptions(ref Guid riid, out int pdwSupportedOptions, out int pdwEnabledOptions)
        {
            ObjectSafetyOptions m_options = ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_CALLER | ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_DATA;
            pdwSupportedOptions = (int)m_options;
            pdwEnabledOptions = (int)m_options;
            return 0;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            return 0;
        }

        #endregion
    }
}

