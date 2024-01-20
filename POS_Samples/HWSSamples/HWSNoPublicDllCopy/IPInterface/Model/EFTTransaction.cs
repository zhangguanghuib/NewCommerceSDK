using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
	/// <summary>PC-EFTPOS terminal applications.</summary>
	public enum TerminalApplication
	{
		/// <summary>The request is for the EFTPOS application.</summary>
		EFTPOS,
		/// <summary>The request is for the Agency application.</summary>
		Agency,
		/// <summary>The request is for the GiftCard application.</summary>
		GiftCard,
		/// <summary>The request is for the Fuel application.</summary>
		Fuel,
		/// <summary>The request is for the Medicare application.</summary>
		Medicare,
		/// <summary>The request is for the Amex application.</summary>
		Amex,
		/// <summary>The request is for the ChequeAuth application.</summary>
		ChequeAuth,
		/// <summary>The request is for the Loyalty application.</summary>
		Loyalty,
		/// <summary>The request is for the PrePaidCard application.</summary>
		PrePaidCard,
		/// <summary>The request is for the ETS application.</summary>
		ETS
	}

	/// <summary>EFTPOS transaction types.</summary>
	public enum TransactionType
	{
		/// <summary>Transaction type was not set by the PIN pad (' ').</summary>
		NotSet = ' ',
		/// <summary>A purchase with optional cash-out EFT transaction type ('P').</summary>
		PurchaseCash = 'P',
		/// <summary>A cash-out only EFT transaction type ('C').</summary>
		CashOut = 'C',
		/// <summary>A refund EFT transaction type ('R').</summary>
		Refund = 'R',
		/// <summary>A pre-authorization EFT transaction type ('A').</summary>
		PreAuth = 'A',
		/// <summary>A pre-authorization / completion EFT transaction type ('L').</summary>
		PreAuthCompletion = 'L',
		/// <summary>A pre-authorization / enquiry EFT transaction type ('N').</summary>
		PreAuthEnquiry = 'N',
		/// <summary>A pre-authorization / cancel EFT transaction type ('Q').</summary>
		PreAuthCancel = 'Q',
		/// <summary>A completion EFT transaction type ('M').</summary>
		Completion = 'M',
		/// <summary>A tip adjustment EFT transaction type ('T').</summary>
		TipAdjust = 'T',
		/// <summary>A deposit EFT transaction type ('D').</summary>
		Deposit = 'D',
		/// <summary>A witdrawal EFT transaction type ('W').</summary>
		Withdrawal = 'W',
		/// <summary>A balance EFT transaction type ('B').</summary>
		Balance = 'B',
		/// <summary>A voucher EFT transaction type ('V').</summary>
		Voucher = 'V',
		/// <summary>A funds transfer EFT transaction type ('F').</summary>
		FundsTransfer = 'F',
		/// <summary>A order request EFT transaction type ('O').</summary>
		OrderRequest = 'O',
		/// <summary>A mini transaction history EFT transaction type ('H').</summary>
		MiniTransactionHistory = 'H',
		/// <summary>A auth pin EFT transaction type ('X').</summary>
		AuthPIN = 'X',
		/// <summary>A enhanced pin EFT transaction type ('K').</summary>
		EnhancedPIN = 'K',
		/// <summary>This command will cancel or void a previous sale ('I').</summary>
		Void = 'I',

		/// <summary>A Redemption allows the POS to use the card as a payment type. This will take the amount from the Card balance ('P').</summary>
		[Filter("ETS")]
		Redemption = 'P',
		/// <summary>A Refund to Card allows the POS to return the value of a previous sale to a  Card ('R').</summary>
		[Filter("ETS")]
		RefundToCard = 'R',
		/// <summary></summary>
		[Filter("ETS")]
		CardSaleTopUp = 'T',
		/// <summary></summary>
		[Filter("ETS")]
		CardSale = 'D',
		/// <summary>A Refund from card  allows the POS to instruct the host to take an amount from a Card ('W').</summary>
		[Filter("ETS")]
		RefundFromCard = 'W',
		/// <summary>A Balance returns the current balance of funds on the card ('B').</summary>
		[Filter("ETS")]
		CardBalance = 'B',
		/// <summary>Activates the card ('A').</summary>
		[Filter("ETS")]
		CardActivate = 'A',
		/// <summary>A de-activate returns a cards to state where the card requires activation before it can be used ('F'). </summary>
		[Filter("ETS")]
		CardDeactivate = 'F',
		/// <summary>This command will add a number of points (or dollars) to a card ('N').</summary>
		[Filter("ETS")]
		AddPointsToCard = 'N',
		/// <summary>This command will subtract a number of points (or dollars) from a card ('K').</summary>
		[Filter("ETS")]
		DecrementPointsFromCard = 'K',
		/// <summary>This command allows a POS to transfer points from a card to another source ('M').</summary>
		[Filter("ETS")]
		TransferPoints = 'M',
		/// <summary>This command will return the amount of cash that is currently on the card and decrement the entire amount from the card ('X').</summary>
		[Filter("ETS")]
		CashBackFromCard = 'X',
		/// <summary>This command will cancel or void a previous sale ('I').</summary>
		[Filter("ETS")]
		CancelVoid = 'I',
		/// <summary>This command adds a card to the card list on the Host ('L').</summary>
		[Filter("ETS")]
		AddCard = 'L',

		None = 0
	}

	public class FilterAttribute : Attribute
	{
		public FilterAttribute(string customString)
		{
			this.customString = customString;
		}
		private string customString;
		public string CustomString
		{
			get { return customString; }
			set { customString = value; }
		}
	}

	public class DescriptionAttribute : Attribute
	{
		public DescriptionAttribute(string description)
		{
			this.description = description;
		}
		private string description;
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	}

	/// <summary>Supported EFTPOS account types.</summary>
	public enum AccountType
	{
		/// <summary>The default account type for a card.</summary>
		Default = ' ',
		/// <summary>The savings account type.</summary>
		Savings = '3',
		/// <summary>The cheque account type.</summary>
		Cheque = '1',
		/// <summary>The credit account type.</summary>
		Credit = '2'
	}

	/// <summary>The card entry type of the transaction.</summary>
	public enum CardEntryType
	{
		/// <summary>Manual entry type was not set by the PIN pad.</summary>
		NotSet = ' ',
		/// <summary>Unknown manual entry type. PIN pad may not support this flag.</summary>
		Unknown = '0',
		/// <summary>Card was swiped.</summary>
		Swiped = 'S',
		/// <summary>Card number was keyed.</summary>
		Keyed = 'K',
		/// <summary>Card number was read by a bar code scanner.</summary>
		BarCode = 'B',
		/// <summary>Card number was read from a chip card.</summary>
		ChipCard = 'E',
		/// <summary>Card number was read from a contactless reader.</summary>
		Contactless = 'C',
	}

	/// <summary>The communications method used to process the transaction.</summary>
	public enum CommsMethodType
	{
		/// <summary>Comms method type was not set by the PIN pad.</summary>
		NotSet = ' ',
		/// <summary>Transaction was sent to the bank using an unknown method.</summary>
		Unknown = '0',
		/// <summary>Transaction was sent to the bank using a P66 modem.</summary>
		P66 = '1',
		/// <summary>Transaction was sent to the bank using an Argent.</summary>
		Argent = '2',
		/// <summary>Transaction was sent to the bank using an X25.</summary>
		X25 = '3'
	}

	/// <summary>The currency conversion status for the transaction.</summary>
	public enum CurrencyStatus
	{
		/// <summary>Currency conversion status was not set by the PIN pad.</summary>
		NotSet = ' ',
		/// <summary>Transaction amount was processed in Australian Dollars.</summary>
		AUD = '0',
		/// <summary>Transaction amount was currency converted.</summary>
		Converted = '1'
	}

	/// <summary>The Pay Pass status of the transcation.</summary>
	public enum PayPassStatus
	{
		/// <summary>Pay Pass conversion status was not set by the PIN pad.</summary>
		NotSet = ' ',
		/// <summary>Pay Pass was used in the transaction.</summary>
		PayPassUsed = '1',
		/// <summary>Pay Pass was not used in the transaction.</summary>
		PayPassNotUsed = '0'
	}

	/// <summary>Flags that indicate how the transaction was processed.</summary>
	public class TxnFlags
	{
        readonly char[] flags;

		/// <summary>Constructs a TxnFlags object with default values.</summary>
		public TxnFlags()
		{
		}

		/// <summary>Constructs a TxnFlags object.</summary>
		/// <param name="flags">A <see cref="System.Char">Char array</see> representing the flags.</param>
		public TxnFlags(char[] flags)
		{
			this.flags = new char[8] { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
			System.Array.Copy(flags, 0, this.flags, 0, (flags.Length > 8) ? 8 : flags.Length);


			Offline = this.flags[0] == '1';
			ReceiptPrinted = this.flags[1] == '1';
			CardEntry = (CardEntryType)this.flags[2];
			CommsMethod = (CommsMethodType)this.flags[3];
			Currency = (CurrencyStatus)this.flags[4];
			PayPass = (PayPassStatus)this.flags[5];
			UndefinedFlag6 = this.flags[6];
			UndefinedFlag7 = this.flags[7];
		}

		/// <summary>Indicates if a receipt was printed for this transaction.</summary>
		/// <value>Type: <see cref="System.Boolean"/><para>Set to TRUE if a receipt was printed.</para></value>
		public bool ReceiptPrinted { get; set; } = false;

		/// <summary>Indicates if the transaction was approved offline.</summary>
		/// <value>Type: <see cref="System.Boolean"/><para>Set to TRUE if the transaction was approved offline.</para></value>
		public bool Offline { get; set; } = false;

		/// <summary>Indicates the card entry type.</summary>
		/// <value>Type: <see cref="CardEntryType"/></value>
		public CardEntryType CardEntry { get; set; } = CardEntryType.NotSet;

		/// <summary>Indicates the communications method used for this transaction.</summary>
		/// <value>Type: <see cref="CommsMethodType"/></value>
		public CommsMethodType CommsMethod { get; set; } = CommsMethodType.NotSet;

		/// <summary>Indicates the currency conversion status for this transaction.</summary>
		/// <value>Type: <see cref="CurrencyStatus"/></value>
		public CurrencyStatus Currency { get; set; } = CurrencyStatus.NotSet;

		/// <summary>Indicates the Pay Pass status for this transaction.</summary>
		/// <value>Type: <see cref="PayPassStatus"/></value>
		public PayPassStatus PayPass { get; set; } = PayPassStatus.NotSet;

		/// <summary>Undefined flag 6</summary>
		public char UndefinedFlag6 { get; set; } = ' ';

		/// <summary>Undefined flag 7</summary>
		public char UndefinedFlag7 { get; set; } = ' ';
	}

	/// <summary>A PC-EFTPOS transaction request object.</summary>
	public class EFTTransactionRequest : EFTRequest
	{
		/// <summary>Constructs a default EFTTransactionRequest object.</summary>
		public EFTTransactionRequest() : base(true, typeof(EFTTransactionResponse))
		{
		}

		/// <summary>The type of transaction to perform.</summary>
		/// <value>Type: <see cref="TransactionType"/><para>The default is <see cref="TransactionType.PurchaseCash"></see></para></value>
		public TransactionType TxnType { get; set; } = TransactionType.PurchaseCash;

		/// <summary>The type of transaction to perform.</summary>
		/// <value>Type: <see cref="TransactionType"/><para>The default is <see cref="TransactionType.PurchaseCash"></see></para></value>
		[System.Obsolete("Please use TxnType instead of Type")]
		public TransactionType Type { get { return TxnType; } set { TxnType = value; } }

		/// <summary>Two digit merchant code</summary>
		/// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
		public string Merchant { get; set; } = "00";

		/// <summary>The currency code for this transaction.</summary>
		/// <value>Type: <see cref="System.String"/><para>A 3 digit ISO currency code. The default is "   ".</para></value>
		public string CurrencyCode { get; set; } = "  ";

		/// <summary>The original type of transaction for voucher entry.</summary>
		/// <value>Type: <see cref="TransactionType"/><para>The default is <see cref="TransactionType.PurchaseCash"></see></para></value>
		public TransactionType OriginalTxnType { get; set; } = TransactionType.PurchaseCash;

		/// <summary>Date. Used for voucher or completion only</summary>
		/// <value>Type: <see cref="DateTime"/><para>The default is null</para></value>
		public DateTime? Date { get; set; } = null;

		/// <summary>Time. Used for voucher or completion only</summary>
		/// <value>Type: <see cref="DateTime"/><para>The default is null</para></value>
		public DateTime? Time { get; set; } = null;

		/// <summary>Determines if the transaction is a training mode transaction.</summary>
		/// <value>Type: <see cref="System.Boolean"/><para>Set to TRUE if the transaction is to be performed in training mode. The default is FALSE.</para></value>
		public bool TrainingMode { get; set; } = false;

		/// <summary>Indicates if the transaction should be tipable.</summary>
		/// <value>Type: <see cref="System.Boolean"/><para>Set to TRUE if tipping is to be enabled for this transaction. The default is FALSE.</para></value>
		public bool EnableTip { get; set; } = false;

		/// <summary>Indicates if the transaction should be tipable.</summary>
		/// <value>Type: <see cref="System.Boolean"/><para>Set to TRUE if tipping is to be enabled for this transaction. The default is FALSE.</para></value>
		[System.Obsolete("Please use EnableTip instead of EnableTipping")]
		public bool EnableTipping { get { return EnableTip; } set { EnableTip = value; } }

		/// <summary>The cash amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for a <see cref="TransactionType.CashOut"></see> transaction type.</remarks>
		public decimal AmtCash { get; set; } = 0;

		/// <summary>The cash amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for a <see cref="TransactionType.CashOut"></see> transaction type.</remarks>
		[System.Obsolete("Please use AmtCash instead of AmountCash")]
		public decimal AmountCash { get { return AmtCash; } set { AmtCash = value; } }

		/// <summary>The purchase amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for all but <see cref="TransactionType.CashOut"></see> transaction types.</remarks>
		public decimal AmtPurchase { get; set; } = 0;

		/// <summary>The purchase amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for all but <see cref="TransactionType.CashOut"></see> transaction types.</remarks>
		[System.Obsolete("Please use AmtPurchase instead of AmountPurchase")]
		public decimal AmountPurchase { get { return AmtPurchase; } set { AmtPurchase = value; } }

		/// <summary>The authorisation number for the transaction.</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		/// <remarks>This property is required for a <see cref="TransactionType.Completion"></see> transaction type.</remarks>
		public int AuthCode { get; set; } = 0;

		/// <summary>The authorisation number for the transaction.</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		/// <remarks>This property is required for a <see cref="TransactionType.Completion"></see> transaction type.</remarks>
		[System.Obsolete("Please use AuthCode instead of AuthNumber")]
		public int AuthNumber { get { return AuthCode; } set { AuthCode = value; } }


		/// <summary>The reference number to attach to the transaction. This will appear on the receipt.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>This property is optional but it usually populated by a unique transaction identifier that can be used for retrieval.</remarks>
		public string TxnRef { get; set; } = "";

		/// <summary>The reference number to attach to the transaction. This will appear on the receipt.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>This property is optional but it usually populated by a unique transaction identifier that can be used for retrieval.</remarks>
		[System.Obsolete("Please use TxnRef instead of ReferenceNumber")]
		public string ReferenceNumber { get { return TxnRef; } set { TxnRef = value; } }



		/// <summary>Indicates the source of the card number.</summary>
		/// <value>Type: <see cref="PanSource"/><para>The default is <see cref="PanSource.Default"></see>.</para></value>
		/// <remarks>Use this property for card not present transactions.</remarks>
		public PanSource PanSource { get; set; } = PanSource.Default;


		/// <summary>Indicates the source of the card number.</summary>
		/// <value>Type: <see cref="PanSource"/><para>The default is <see cref="PanSource.Default"></see>.</para></value>
		/// <remarks>Use this property for card not present transactions.</remarks>
		[System.Obsolete("Please use PanSource instead of CardPANSource")]
		public PanSource CardPANSource { get { return PanSource; } set { PanSource = value; } }

		/// <summary>The card number to use when pan source of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see>.</remarks>
		public string Pan { get; set; } = "";

		/// <summary>The card number to use when pan source of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see>.</remarks>
		[System.Obsolete("Please use Pan instead of CardPAN")]
		public string CardPAN { get { return Pan; } set { Pan = value; } }

		/// <summary>The expiry date of the card when of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/><para>In MMYY format.</para></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the card expiry date to PC-EFTPOS.</remarks>
		public string DateExpiry { get; set; } = "";

		/// <summary>The expiry date of the card when of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/><para>In MMYY format.</para></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the card expiry date to PC-EFTPOS.</remarks>
		[System.Obsolete("Please use DateExpiry instead of ExpiryDate")]
		public string ExpiryDate { get { return DateExpiry; } set { DateExpiry = value; } }

		/// <summary>The track 2 to use when of POS swiped is used.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>Use this property when <see cref="PanSource"></see> is set to <see cref="PanSource.POSSwiped"></see> and passing the full Track2 from the card magnetic stripe to PC-EFTPOS.</remarks>
		public string Track2 { get; set; } = "";

        /// <summary>The account to use for this transaction.</summary>
        /// <value>Type: <see cref="AccountType"/><para>Default is <see cref="AccountType.Default"></see>. Use default to prompt user to enter the account type.</para></value>
        /// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the account type to PC-EFTPOS.</remarks>
        public AccountType AccountType { get; set; } = AccountType.Default;

        /// <summary>The account to use for this transaction.</summary>
        /// <value>Type: <see cref="AccountType"/><para>Default is <see cref="AccountType.Default"></see>. Use default to prompt user to enter the account type.</para></value>
        /// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the account type to PC-EFTPOS.</remarks>
        [System.Obsolete("Please use AccountType instead of CardAccountType")]
        public AccountType CardAccountType { get { return AccountType; } set { AccountType = value; } }

        /// <summary>The retrieval reference number for the transaction.</summary>
        /// <value>Type: <see cref="System.String"/></value>
        /// <remarks>This property is required for a <see cref="TransactionType.TipAdjust"></see> transaction type.</remarks>
        public string RRN { get; set; } = "";

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

        /// <summary>
        /// 
        /// </summary>
        public int CVV { get; set; } = 0;
	}

	/// <summary>A PC-EFTPOS terminal transaction response object.</summary>
	public class EFTTransactionResponse : EFTResponse
	{
		/// <summary>Constructs a default terminal transaction response object.</summary>
		public EFTTransactionResponse() : base(typeof(EFTGetLastTransactionRequest))
		{
		}

		/// <summary>The type of transaction to perform.</summary>
		/// <value>Type: <see cref="TransactionType"/><para>The default is <see cref="TransactionType.PurchaseCash"></see></para></value>
		public TransactionType TxnType { get; set; } = TransactionType.PurchaseCash;

		/// <summary>The type of transaction to perform.</summary>
		/// <value>Type: <see cref="TransactionType"/><para>The default is <see cref="TransactionType.PurchaseCash"></see></para></value>
		[System.Obsolete("Please use TxnType instead")]
		public TransactionType Type { get { return TxnType; } set { TxnType = value; } }

		/// <summary>Two digit merchant code</summary>
		/// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
		public string Merchant { get; set; } = "00";

		/// <summary>Indicates the card type that was used in the transaction.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		/// <remarks><seealso cref="EFTTransactionResponse.CardBIN"/></remarks>
		public string CardType { get; set; } = "";

		/// <summary>Indicates the card type that was used in the transaction.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		/// <remarks><list type="table">
		/// <listheader><term>Card BIN</term><description>Card Type</description></listheader>
		///	<item><term>0</term><description>Unknown</description></item>
		///	<item><term>1</term><description>Debit</description></item>
		///	<item><term>2</term><description>Bankcard</description></item>
		///	<item><term>3</term><description>Mastercard</description></item>
		///	<item><term>4</term><description>Visa</description></item>
		///	<item><term>5</term><description>American Express</description></item>
		///	<item><term>6</term><description>Diner Club</description></item>
		///	<item><term>7</term><description>JCB</description></item>
		///	<item><term>8</term><description>Label Card</description></item>
		///	<item><term>9</term><description>JCB</description></item>
		///	<item><term>11</term><description>JCB</description></item>
		///	<item><term>12</term><description>Other</description></item></list>
		///	</remarks>
		public int CardName { get; set; } = 0;

		/// <summary>Indicates the card type that was used in the transaction.</summary>
		/// <value>Type: <see cref="System.Int32" /></value>
		/// <remarks><list type="table">
		/// <listheader><term>Card BIN</term><description>Card Type</description></listheader>
		///	<item><term>0</term><description>Unknown</description></item>
		///	<item><term>1</term><description>Debit</description></item>
		///	<item><term>2</term><description>Bankcard</description></item>
		///	<item><term>3</term><description>Mastercard</description></item>
		///	<item><term>4</term><description>Visa</description></item>
		///	<item><term>5</term><description>American Express</description></item>
		///	<item><term>6</term><description>Diner Club</description></item>
		///	<item><term>7</term><description>JCB</description></item>
		///	<item><term>8</term><description>Label Card</description></item>
		///	<item><term>9</term><description>JCB</description></item>
		///	<item><term>11</term><description>JCB</description></item>
		///	<item><term>12</term><description>Other</description></item></list>
		///	</remarks>
		[System.Obsolete("Please use CardName instead of CardBIN")]
		public int CardBIN { get { return CardName; } set { CardName = value; } }

		/// <summary>Used to retrieve the transaction from the batch.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		/// <remarks>The retrieval reference number is used when performing a tip adjustment transaction.</remarks>
		public string RRN { get; set; } = "";

        /// <summary>Indicates which settlement batch this transaction will be included in.</summary>
        /// <value>Type: <see cref="System.DateTime" /><para>Settlement date is returned from the bank.</para></value>
        /// <remarks>Use this property to balance POS EFT totals with settlement EFT totals.</remarks
        public DateTime DateSettlement { get; set; } = DateTime.MinValue;

        /// <summary>Indicates which settlement batch this transaction will be included in.</summary>
        /// <value>Type: <see cref="System.DateTime" /><para>Settlement date is returned from the bank.</para></value>
        /// <remarks>Use this property to balance POS EFT totals with settlement EFT totals.</remarks>
        [System.Obsolete("Please use DateSettlement instead of SettlementDate")]
        public DateTime SettlementDate { get { return DateSettlement; } set { DateSettlement = value; } }

        /// <summary>The cash amount for the transaction.</summary>
        /// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
        /// <remarks>This property is mandatory for a <see cref="TransactionType.CashOut"></see> transaction type.</remarks>
        public decimal AmtCash { get; set; } = 0;

		/// <summary>The cash amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for a <see cref="TransactionType.CashOut"></see> transaction type.</remarks>
		[System.Obsolete("Please use AmtCash instead of AmountCash")]
		public decimal AmountCash { get { return AmtCash; } set { AmtCash = value; } }

		/// <summary>The purchase amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for all but <see cref="TransactionType.CashOut"></see> transaction types.</remarks>
		public decimal AmtPurchase { get; set; } = 0;

		/// <summary>The purchase amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal"/><para>The default is 0.</para></value>
		/// <remarks>This property is mandatory for all but <see cref="TransactionType.CashOut"></see> transaction types.</remarks>
		[System.Obsolete("Please use AmtPurchase instead of AmountPurchase")]
		public decimal AmountPurchase { get { return AmtPurchase; } set { AmtPurchase = value; } }

		/// <summary>The tip amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal" /><para>Echoed from the request.</para></value>
		public decimal AmtTip { get; set; } = 0;

		/// <summary>The tip amount for the transaction.</summary>
		/// <value>Type: <see cref="System.Decimal" /><para>Echoed from the request.</para></value>
		[System.Obsolete("Please use AmtTip instead of AmountTip")]
		public decimal AmountTip { get { return AmtTip; } set { AmtTip = value; } }

		/// <summary>The authorisation number for the transaction.</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		/// <remarks>This property is required for a <see cref="TransactionType.Completion"></see> transaction type.</remarks>
		public int AuthCode { get; set; } = 0;

		/// <summary>The authorisation number for the transaction.</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		/// <remarks>This property is required for a <see cref="TransactionType.Completion"></see> transaction type.</remarks>
		[System.Obsolete("Please use AuthCode instead of AuthNumber")]
		public int AuthNumber { get { return AuthCode; } set { AuthCode = value; } }

		/// <summary>The reference number to attach to the transaction. This will appear on the receipt.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>This property is optional but it usually populated by a unique transaction identifier that can be used for retrieval.</remarks>
		public string TxnRef { get; set; } = "";

		/// <summary>The reference number to attach to the transaction. This will appear on the receipt.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>This property is optional but it usually populated by a unique transaction identifier that can be used for retrieval.</remarks>
		[System.Obsolete("Please use TxnRef instead of ReferenceNumber")]
		public string ReferenceNumber { get { return TxnRef; } set { TxnRef = value; } }

		/// <summary>The card number to use when pan source of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see>.</remarks>
		public string Pan { get; set; } = "";

		/// <summary>The card number to use when pan source of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see>.</remarks>
		[System.Obsolete("Please use PAN instead of CardPAN")]
		public string CardPAN { get { return Pan; } set { Pan = value; } }

		/// <summary>The expiry date of the card when of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/><para>In MMYY format.</para></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the card expiry date to PC-EFTPOS.</remarks>
		public string DateExpiry { get; set; } = "";

		/// <summary>The expiry date of the card when of POS keyed is used.</summary>
		/// <value>Type: <see cref="System.String"/><para>In MMYY format.</para></value>
		/// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the card expiry date to PC-EFTPOS.</remarks>
		[System.Obsolete("Please use DateExpiry instead of ExpiryDate")]
		public string ExpiryDate { get { return DateExpiry; } set { DateExpiry = value; } }

		/// <summary>The track 2 data on the magnetic stripe of the card.</summary>
		/// <value>Type: <see cref="System.String" /><para>This property contains the partial track 2 data from the card used in this transaction.</para></value>
		public string Track2 { get; set; } = "";

        /// <summary>The account to use for this transaction.</summary>
        /// <value>Type: <see cref="AccountType"/><para>Default is <see cref="AccountType.Default"></see>. Use default to prompt user to enter the account type.</para></value>
        /// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the account type to PC-EFTPOS.</remarks>
        public AccountType AccountType { get; set; } = AccountType.Default;

        /// <summary>The account to use for this transaction.</summary>
        /// <value>Type: <see cref="AccountType"/><para>Default is <see cref="AccountType.Default"></see>. Use default to prompt user to enter the account type.</para></value>
        /// <remarks>Use this property in conjunction with <see cref="PanSource"></see> when passing the account type to PC-EFTPOS.</remarks>
        [System.Obsolete("Please use AccountType instead of CardAccountType")]
        public AccountType CardAccountType { get { return AccountType; } set { AccountType = value; } }

        /// <summary>Flags that indicate how the transaction was processed.</summary>
        /// <value>Type: <see cref="TxnFlags" /></value>
        public TxnFlags TxnFlags { get; set; } = new TxnFlags();

		/// <summary>Indicates if an available balance is present in the response.</summary>
		/// <value>Type: <see cref="System.Boolean" /></value>
		public bool BalanceReceived { get; set; } = false;

		/// <summary>Balance available on the processed account.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal AvailableBalance { get; set; } = 0;

		/// <summary>Cleared balance on the processed account.</summary>
		/// <value>Type: <see cref="System.Decimal" /></value>
		public decimal ClearedFundsBalance { get; set; } = 0;

		/// <summary>Indicates if the request was successful.</summary>
		/// <value>Type: <see cref="System.Boolean"/></value>
		public bool Success { get; set; } = false;

		/// <summary>The response code of the request.</summary>
		/// <value>Type: <see cref="System.String"/><para>A 2 character response code. "00" indicates a successful response.</para></value>
		public string ResponseCode { get; set; } = "";

		/// <summary>The response text for the response code.</summary>
		/// <value>Type: <see cref="System.String"/></value>
		public string ResponseText { get; set; } = "";

		/// <summary>Date and time of the response returned by the bank.</summary>
		/// <value>Type: <see cref="System.DateTime"/></value>
		public DateTime Date { get; set; } = DateTime.MinValue;

		/// <summary>Date and time of the response returned by the bank.</summary>
		/// <value>Type: <see cref="System.DateTime"/></value>
		[System.Obsolete("Please use Date instead of BankDateTime")]
		public DateTime BankDateTime { get { return Date; } set { Date = value; } }

		/// <summary>Terminal ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string Catid { get; set; } = "";

		/// <summary>Terminal ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use Catid instead of TerminalID")]
		public string TerminalID { get { return Catid; } set { Catid = value; } }

		/// <summary>Merchant ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		public string Caid { get; set; } = "";

		/// <summary>Merchant ID configured in the PIN pad.</summary>
		/// <value>Type: <see cref="System.String" /></value>
		[System.Obsolete("Please use Caid instead of MerchantID")]
		public string MerchantID { get { return Caid; } set { Caid = value; } }

		/// <summary>System Trace Audit Number</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		public int Stan { get; set; } = 0;

		/// <summary>System Trace Audit Number</summary>
		/// <value>Type: <see cref="System.Int32"/></value>
		[System.Obsolete("Please use Stan instead of STAN")]
		public int STAN { get { return Stan; } set { Stan = value; } }

		/// <summary>Additional information sent with the response.</summary>
		/// <value>Type: <see cref="PadField"/></value>
		public PadField PurchaseAnalysisData { get; set; } = new PadField();
	}
}