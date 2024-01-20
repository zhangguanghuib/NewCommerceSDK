using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>Reprint Type.</summary>
	public enum ReprintType
    {
        /// <summary>Get the last receipt.</summary>
        GetLast = '2',
        /// <summary>Reprint the last receipt.</summary>
        Reprint = '1'
    }


    /// <summary>A PC-EFTPOS duplicate receipt request object.</summary>
	public class EFTReprintReceiptRequest : EFTRequest
	{
		/// <summary>Constructs a default EFTDuplicateReceiptRequest object.</summary>
		public EFTReprintReceiptRequest() : base(true, typeof(EFTReprintReceiptResponse))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
        /// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
        public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;

        /// <summary>Indicates whether to trigger receipt events.</summary>
        /// <value>Type: <see cref="ReceiptPrintModeType"/><para>The default is POSPrinter.</para></value>
        public ReceiptPrintModeType ReceiptAutoPrint { get; set; } = ReceiptPrintModeType.POSPrinter;

        /// <summary>Indicates whether to trigger receipt events.</summary>
        /// <value>Type: <see cref="ReceiptPrintModeType"/><para>The default is POSPrinter.</para></value>
        [System.Obsolete("Please use ReceiptAutoPrint instead of ReceiptPrintMode")]
        public ReceiptPrintModeType ReceiptPrintMode { get { return ReceiptAutoPrint; } set { ReceiptAutoPrint = value; } }

        /// <summary>Indicates whether PC-EFTPOS should cut receipts.</summary>
        /// <value>Type: <see cref="ReceiptCutModeType"/><para>The default is DontCut. This property only applies when <see cref="EFTRequest.ReceiptPrintMode"/> is set to EFTClientPrinter.</para></value>
        public ReceiptCutModeType CutReceipt { get; set; } = ReceiptCutModeType.DontCut;

        /// <summary>Indicates whether PC-EFTPOS should cut receipts.</summary>
        /// <value>Type: <see cref="ReceiptCutModeType"/><para>The default is DontCut. This property only applies when <see cref="EFTRequest.ReceiptPrintMode"/> is set to EFTClientPrinter.</para></value>
        [System.Obsolete("Please use CutReceipt instead of ReceiptCutMode")]
        public ReceiptCutModeType ReceiptCutMode { get { return CutReceipt; } set { CutReceipt = value; } }

        /// <summary>Indicates whether the receipt should be returned or reprinted.</summary>
        /// <value>Type: <see cref="ReprintType"/><para>The default is GetLast.</para></value>
        public ReprintType ReprintType { get; set; } = ReprintType.GetLast;

		public string OriginalTxnRef { get; set; } = "";
    }

	/// <summary>A PC-EFTPOS duplicate receipt response object.</summary>
	public class EFTReprintReceiptResponse : EFTResponse
	{
		/// <summary>Constructs a default duplicate receipt response object.</summary>
		public EFTReprintReceiptResponse() : base(typeof(EFTReprintReceiptRequest))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>Duplicate receipt text.</summary>
        /// <value>Type: <see cref="System.String">String array</see></value>
        public string[] ReceiptText { get; set; } = new string[] { "" };

        /// <summary>Indicates if the request was successful.</summary>
        /// <value>Type: <see cref="System.Boolean"/></value>
        public bool Success { get; set; } = false;

        /// <summary>The response code of the request.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 2 character response code. "00" indicates a successful response.</para></value>
        public string ResponseCode { get; set; } = "";

        /// <summary>The response text for the response code.</summary>
        /// <value>Type: <see cref="System.String"/></value>
        public string ResponseText { get; set; } = "";
    }
}