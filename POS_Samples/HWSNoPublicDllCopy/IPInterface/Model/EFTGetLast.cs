using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>A PC-EFTPOS get last transaction request object.</summary>
	public class EFTGetLastTransactionRequest : EFTRequest
    {
        /// <summary>Constructs a default EFTGetLastTransactionRequest object.</summary>
        public EFTGetLastTransactionRequest() : base(true, typeof(EFTGetLastTransactionResponse))
        {
        }

        /// <summary>Constructs an EFTGetLastTransactionRequest with a TxnRef parameter to look up transaction by.</summary>
        public EFTGetLastTransactionRequest(string TxnRef) : base(true, typeof(EFTGetLastTransactionResponse))
        {
            this.TxnRef = TxnRef ?? "";
        }

        /// <summary>The transaction reference to look up a past transaction by (optional when passed into constructor)</summary>
        public string TxnRef { get; private set; } = "";

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
        /// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
        public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;
	}

	/// <summary>A PC-EFTPOS get last transaction response object.</summary>
	public class EFTGetLastTransactionResponse : EFTResponse
	{
		/// <summary>Constructs a default terminal transaction response object.</summary>
		public EFTGetLastTransactionResponse()
			: base(typeof(EFTGetLastTransactionRequest))
		{ }

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>The type of EFTPOS transaction that was requested.</summary>
        /// <value>Type: <see cref="TransactionType" /><para>Echoed from the request.</para></value>
        public TransactionType TxnType { get; set; } = TransactionType.PurchaseCash;

        /// <summary>The type of EFTPOS transaction that was requested.</summary>
        /// <value>Type: <see cref="TransactionType" /><para>Echoed from the request.</para></value>
        [System.Obsolete("Please use TxnType instead of Type")]
        public TransactionType Type { get { return TxnType; } set { TxnType = value; } }

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

        /// <summary>Indicates if the last transaction was successful.</summary>
        /// <value>Type: <see cref="System.Boolean" /></value>
        public bool LastTransactionSuccess { get; set; } = false;

		/// <summary>Flags that indicate how the transaction was processed.</summary>
		/// <value>Type: <see cref="TxnFlags" /></value>
		public TxnFlags TxnFlags { get; set; } = new TxnFlags();

        /// <summary>Indicates if an available balance is present in the response.</summary>
        /// <value>Type: <see cref="System.Boolean" /></value>
        public bool BalanceReceived { get; set; } = false;

        /// <summary>Balance available on the processed account.</summary>
        /// <value>Type: <see cref="System.Decimal" /></value>
        public decimal AvailableBalance { get; set; } = 0;

        /// <summary>Indicates if the transaction was perform as a training mode transaction.</summary>
        /// <value>Type: <see cref="System.Boolean" /></value>
        public bool IsTrainingMode { get; set; } = false;

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
        public DateTime BankDateTime { get; set; } = DateTime.MinValue;

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