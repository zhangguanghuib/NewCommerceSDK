using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>Indicates which tabs of the PC-EFTPOS Client control panel to display.</summary>
	public enum ControlPanelType
	{
		/// <summary>Show the control panel with all tabs available.</summary>
		Full = '0',
		/// <summary>Show the control panel with only the settlement tab available.</summary>
		Settlement = '1',
		/// <summary>Show the control panel with only the journal viewer tab available.</summary>
		JournalViewer = '2',
		/// <summary>Show the control panel with only the PIN pad setup tab available.</summary>
		PINPadSetup = '3',
		/// <summary>Show the control panel with only the status tab available.</summary>
		Status = '4'
	}

	/// <summary>Indicates when to trigger the <see cref="EFTClientIP.OnDisplayControlPanel"/> event.</summary>
	public enum ControlPanelReturnType
	{
		/// <summary>Trigger the event immediately.</summary>
		Immediately = '0',
		/// <summary>Trigger the event when the control panel is closed.</summary>
		WhenClosed = '1',
		/// <summary>Trigger the event immediately and when the control panel is closed.</summary>
		ImmediatelyAndWhenClosed = '2',
	}

#pragma warning disable CS0618
    /// <summary>A PC-EFTPOS show control panel request object.</summary>
    public class EFTControlPanelRequest : ControlPanelRequest
    {

    }
#pragma warning restore CS0618


    /// <summary>ControlPanelRequest is obsolete. Please use EFTControlPanelRequest</summary>
    [Obsolete("ControlPanelRequest is obsolete. Please use EFTControlPanelRequest")]
    public class ControlPanelRequest : EFTRequest
    {
		/// <summary>Constructs a default show control panel request object.</summary>
		public ControlPanelRequest() : base(true, typeof(EFTControlPanelResponse))
		{
		}

        /// <summary>Indicates which tabs of the PC-EFTPOS Client control panel to display.</summary>
        /// <value>Type: <see cref="ControlPanelType" /><para>The default is <see cref="ControlPanelType.Full" />.</para></value>
        public ControlPanelType ControlPanelType { get; set; } = ControlPanelType.Full;
        
        /// <summary>Indicates which tabs of the PC-EFTPOS Client control panel to display.</summary>
        /// <value>Type: <see cref="ControlPanelReturnType" /><para>The default is <see cref="ControlPanelReturnType.Immediately" />.</para></value>
        public ControlPanelReturnType ReturnType { get; set; } = ControlPanelReturnType.Immediately;

        /// <summary>Indicates whether to trigger receipt events.</summary>
        /// <value>Type: <see cref="ReceiptPrintModeType"/><para>The default is POSPrinter.</para></value>
        public ReceiptPrintModeType ReceiptPrintMode { get; set; } = ReceiptPrintModeType.POSPrinter;

        /// <summary>Indicates whether PC-EFTPOS should cut receipts.</summary>
        /// <value>Type: <see cref="ReceiptCutModeType"/><para>The default is DontCut. This property only applies when <see cref="EFTRequest.ReceiptPrintMode"/> is set to EFTClientPrinter.</para></value>
        public ReceiptCutModeType ReceiptCutMode { get; set; } = ReceiptCutModeType.DontCut;

    }

	/// <summary>A PC-EFTPOS show control panel response object.</summary>
	public class EFTControlPanelResponse : EFTResponse
	{
		/// <summary>Constructs a default show control panel response object.</summary>
		public EFTControlPanelResponse() : base(typeof(EFTControlPanelRequest))
		{
		}

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