namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>EFTPOS settlement types.</summary>
	public enum SettlementType
	{
		/// <summary>Perform a settlement on the terminal.</summary>
		/// <remarks>Can only be performed once per day.</remarks>
		Settlement = 'S',
		/// <summary>Perform a pre-settlement on the terminal.</summary>
		/// <remarks>Not supported by all PIN pads.</remarks>
		PreSettlement = 'P',
		/// <summary>Perform a last settlement on the terminal.</summary>
		/// <remarks>Not supported by all PIN pads.</remarks>
		LastSettlement = 'L',
		/// <summary>Perform a summary totals on the terminal.</summary>
		/// <remarks>Not supported by all PIN pads.</remarks>
		SummaryTotals = 'U',
		/// <summary>Perform a shift/sub totals on the terminal.</summary>
		/// <remarks>Not supported by all PIN pads.</remarks>
		SubShiftTotals = 'H',
		/// <summary>Peform a transaction listing on the terminal.</summary>
		/// <remarks>Not supported by all PIN pads.</remarks>
		DetailedTransactionListing = 'I',
        /// <summary>Start cash</summary>
        /// <remarks>Not supported by all PIN pads.</remarks>
        StartCash = 'M',
        /// <summary>SAF report</summary>
        /// <remarks>Not supported by all PIN pads.</remarks>
        StoreAndForwardTotals = 'F',
        /// <summary>Daily cash</summary>
        /// <remarks>StGeorge agency only.</remarks>
        DailyCashStatement = 'D'
    }

    /// <summary>EFTPOS settlement card totals data.</summary>
    public class SettlementCardTotals
    {
        /// <summary>Constructs a default settlement card totals object.</summary>
        public SettlementCardTotals() 
        {
        }

        /// <summary>Card type name.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string CardName { get; set; } = "";

        /// <summary>Count of purchases transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int PurchaseCount { get; set; } = 0;

        /// <summary>Total of purchases transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Decimal" /></value>
        public decimal PurchaseAmount { get; set; } = 0.0M;

        /// <summary>Count of cash out transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int CashOutCount { get; set; } = 0;

        /// <summary>Total of cash out transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Decimal" /></value>
        public decimal CashOutAmount { get; set; } = 0.0M;

        /// <summary>Count of refund transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int RefundCount { get; set; } = 0;

        /// <summary>Total of refund transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Decimal" /></value>
        public decimal RefundAmount { get; set; } = 0.0M;

        /// <summary>Count of all transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int TotalCount { get; set; } = 0;

        /// <summary>Total of all transactions made with this card type.</summary>
        /// <value>Type: <see cref="System.Decimal" /></value>
        public decimal TotalAmount { get; set; } = 0.0M;
    }

	/// <summary>EFTPOS settlement totals data.</summary>
	public class SettlementTotals
	{

		/// <summary>Constructs a default settlement totals object.</summary>
		public SettlementTotals(): base()
		{
		}

		/// <summary>Settlement totals description.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string TotalsDescription { get; set; } = "";

		/// <summary>Count of purchases transactions made.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int PurchaseCount { get; set; } = 0;

		/// <summary>Total of purchases transactions made.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal PurchaseAmount { get; set; } = 0.0M;

		/// <summary>Count of cash out transactions made.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int CashOutCount { get; set; } = 0;

		/// <summary>Total of cash out transactions made.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal CashOutAmount { get; set; } = 0.0M;

		/// <summary>Count of refund transactions made.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int RefundCount { get; set; } = 0;

		/// <summary>Total of refund transactions made.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal RefundAmount { get; set; } = 0.0M;

		/// <summary>Count of all transactions made.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		public int TotalCount { get; set; } = 0;

		/// <summary>Total of all transactions made.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal TotalAmount { get; set; } = 0.0M;

		/// <summary>Extra settlement totals data.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string Extra { get; set; } = "";
	}

	/// <summary>A PC-EFTPOS terminal settlement request object.</summary>
	public class EFTSettlementRequest : EFTRequest
	{
    	/// <summary>Constructs a default terminal settlement request object.</summary>
		public EFTSettlementRequest(): base(true, typeof(EFTSettlementResponse))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>EFT settlement type.</summary>
        /// <value>Type: <see cref="SettlementType" /><para>The default is <see cref="SettlementType.Settlement" />.</para></value>
        public SettlementType SettlementType { get; set; } = SettlementType.Settlement;

        /// <summary>PIN pad software version.</summary>
        /// <value>Type: <see cref="System.String" /><para>The default is FALSE.</para></value>
        public bool ResetTotals { get; set; } = false;

        /// <summary>Additional information sent with the request.</summary>
        /// <value>Type: <see cref="PadField"/></value>
        public PadField PurchaseAnalysisData { get; set; } = new PadField();

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
    }

	/// <summary>A PC-EFTPOS terminal settlement response object.</summary>
	public class EFTSettlementResponse : EFTResponse
	{
		//List<SettlementCardTotals> settleCardData;
		//SettlementTotals totalsData;

		/// <summary>Constructs a default terminal settlement response object.</summary>
		public EFTSettlementResponse() : base(typeof(EFTSettlementRequest))
		{
			//settleCardData = new List<SettlementCardTotals>();
			//totalsData = new SettlementTotals();
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        ///// <summary>Number of cards present in the settlement totals.</summary>
        ///// <value>Type: <see cref="System.Int32" /></value>
        //public int SettlementCardCount
        //{
        //    get { return settleCardData.Count; }
        //}

        ///// <summary>List of SettlementCardTotals objects.</summary>
        ///// <value>Type: <see cref="SettlementCardTotals">SettlementCardTotals list</see></value>
        //public List<SettlementCardTotals> SettlementCardData
        //{
        //    get { return settleCardData; }
        //    set { settleCardData = value; }
        //}

        ///// <summary>Settlement totals.</summary>
        ///// <value>Type: <see cref="SettlementTotals" /></value>
        //public SettlementTotals TotalsData
        //{
        //    get { return totalsData; }
        //    set { totalsData = value; }
        //}

        /// <summary>Settlement data</summary>
        public string SettlementData { get; set; } = "";

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