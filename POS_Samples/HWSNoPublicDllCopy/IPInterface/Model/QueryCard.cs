using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>Indicates the type of logon to perform.</summary>
    public enum QueryCardType
    {
        /// <summary>Read card only</summary>
        ReadCard = '0',
        /// <summary>Read card + select account</summary>
        ReadCardAndSelectAccount = '1',
        /// <summary>Select account only</summary>
        SelectAccount = '5',
        /// <summary>Pre-swipe</summary>
        /// <remarks>Do not use</remarks>
        [Filter("WW")]
        PreSwipe = '7',
        /// <summary>Pre-swipe special</summary>
        /// <remarks>Do not use</remarks>
        [Filter("WW")]
        PreSwipeSpecial = '8',
        /// <summary>Pre-swipe special 2</summary>
        /// <remarks>Do not use</remarks>
        [Filter("WW")]
        PreSwipeSpecial2 = '8'
    }


    /// <summary>Indicates what tracks are available in the response.</summary>
    [Flags()]
	public enum TrackFlags
	{
		/// <summary>Track 1 is available.</summary>
		Track1,
		/// <summary>Track 2 is available.</summary>
		Track2,
		/// <summary>Track 3 is available.</summary>
		Track3
	}

#pragma warning disable CS0618
    /// <summary>A PC-EFTPOS terminal query card request object.</summary>
    public class EFTQueryCardRequest : QueryCardRequest
    {
    }
#pragma warning restore CS0618


    /// <summary>
    /// QueryCardRequest is obsolete. Please use EFTQueryCardRequest
    /// </summary>
    [Obsolete("QueryCardRequest is obsolete. Please use EFTQueryCardRequest")]
    public class QueryCardRequest : EFTRequest
	{
		/// <summary>Constructs a default terminal query card request object.</summary>
		public QueryCardRequest() : base(true, typeof(EFTQueryCardResponse))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>Query card type.</summary>
        /// <value>Type: <see cref="QueryCardType" /><para>The default is <see cref="QueryCardType.ReadCard" />.</para></value>
        public QueryCardType QueryCardType { get; set; } = QueryCardType.ReadCard;

        /// <summary>Additional information sent with the request.</summary>
        /// <value>Type: <see cref="PadField"/></value>
        public PadField PurchaseAnalysisData { get; set; } = new PadField();

        /// <summary>Indicates where the request is to be sent to. Should normally be EFTPOS.</summary>
        /// <value>Type: <see cref="TerminalApplication"/><para>The default is <see cref="TerminalApplication.EFTPOS"/>.</para></value>
        public TerminalApplication Application { get; set; } = TerminalApplication.EFTPOS;
    }

	/// <summary>A PC-EFTPOS terminal query card response object.</summary>
	public class EFTQueryCardResponse : EFTResponse
	{
		/// <summary>Constructs a default terminal logon response object.</summary>
		public EFTQueryCardResponse()
			: base(typeof(EFTQueryCardRequest))
		{
		}

        /// <summary>Two digit merchant code</summary>
        /// <value>Type: <see cref="string"/><para>The default is "00"</para></value>
        public string Merchant { get; set; } = "00";

        /// <summary>Flags indicating tracks present in the response.</summary>
        /// <value>Type: <see cref="TrackFlags" /></value>
        public TrackFlags TrackFlags { get; set; }

        /// <summary>Data encoded on Track1 of the card.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string Track1 { get; set; } = "";

        /// <summary>Data encoded on Track2 of the card.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string Track2 { get; set; } = "";

        /// <summary>Data encoded on Track3 of the card.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string Track3 { get; set; } = "";

        /// <summary>BIN number of the card swiped.</summary>
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

        /// <summary>BIN number of the card swiped.</summary>
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
        public int CardBin { get { return CardName; } set { CardName = value; } }

        /// <summary>Account type selected.</summary>
        /// <value>Type: <see cref="AccountType" /></value>
        public AccountType AccountType { get; set; }

        /// <summary>Indicates if the request was successful.</summary>
        /// <value>Type: <see cref="System.Boolean"/></value>
        public bool Success { get; set; } = false;

        /// <summary>The response code of the request.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 2 character response code. "00" indicates a successful response.</para></value>
        public string ResponseCode { get; set; } = "";

        /// <summary>The response text for the response code.</summary>
        /// <value>Type: <see cref="System.String"/></value>
        public string ResponseText { get; set; } = "";

        /// <summary>Additional information sent with the response.</summary>
        /// <value>Type: <see cref="PadField"/></value>
        public PadField PurchaseAnalysisData { get; set; } = new PadField();
    }
}