using PCEFTPOS.EFTClient.IPInterface.Slave;
using System;
using System.Runtime.CompilerServices;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public interface IEFTClientIP: IDisposable
    {
        #region Properties

        /// <summary>The IP host name of the PC-EFTPOS IP Client</summary>
        /// <value>Type: <see cref="System.String" /><para>The IP address or host name of the EFT Client IP interface.</para></value>
        /// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
        string HostName { get; set; }

        /// <summary>The IP port of the PC-EFTPOS IP Client.</summary>
        /// <value>Type: <see cref="System.Int32" /><para>The listening port of the EFT Client IP interface.</para></value>
        /// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
        int HostPort { get; set; }

        /// <summary>Indicates whether to use SSL encryption.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        bool UseSSL { get; set; }

        /// <summary>Indicates whether to allow TCP keep-alives.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        bool UseKeepAlive { get; set; }

        /// <summary>Indicates whether there is a request currently in progress.</summary>
        bool IsRequestInProgress { get; }

        /// <summary>
        /// Returns the connected state as of the last read or write operation. This does not necessarily represent 
        /// the current state of the connection. 
        /// To check the current socket state call <see cref="CheckConnectState()"/>
        /// </summary>
        bool IsConnected { get; }

        /// <summary> When TRUE, the SynchronizationContext will be captured from requests and used to call events</summary>
        bool UseSynchronizationContextForEvents { get; set; }

        /// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
        PCEFTPOS.EFTClient.IPInterface.LogLevel LogLevel { get; set; }

        /// <summary>Implementation of a dialog manager</summary>
        IDialogUIHandler DialogUIHandler { get; set; }
        #endregion

        #region Methods

        /// <summary>Connect to the PC-EFTPOS IP Client.</summary>
        /// <returns>TRUE if connected successfully.</returns>
        /// <example>This example code shows how to connect to the EFT Client IP interface using the <see cref="EFTClientIP" /> class.
        /// <code>
        ///	eftClientIP.HostName = "127.0.0.1";
        ///	eftClientIP.HostPort = 6001;
        ///	
        ///	if( !eftClientIP.Connect() )
        ///	{
        ///	    MessageBox.Show( "Couldn't connect to the PC-EFTPOS IP Client at " + eftClientIP.HostName + ":" + eftClientIP.HostPort.ToString(),
        ///	        "PC-EFTPOS IP Client Test POS", MessageBoxButtons.OK, MessageBoxIcon.Error );
        ///	    return false;
        ///	}
        /// </code>
        /// </example>
        bool Connect();

        /// <summary>Disconnect from the PC-EFTPOS client IP interface.</summary>
        void Disconnect();

        /// <summary>
        /// Polls the socket to determine the current connect state
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        bool CheckConnectState();

        /// <summary>Sends a request to the EFT-Client</summary>
        /// <param name="request">The <see cref="EFTRequest"/> to send</param>
        /// <param name="member">Used for internal logging. Ignore</param>
        /// <returns>FALSE if an error occurs</returns>
        bool DoRequest(EFTRequest request, [CallerMemberName] string member = "");

        /// <summary>Hide the PC-EFTPOS dialogs.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoHideDialogs();

        /// <summary>Initiate a PC-EFTPOS logon using default values.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoLogon();

        /// <summary>Initiate a PC-EFTPOS logon.</summary>
        /// <param name="request">An <see cref="EFTLogonRequest" /> object.</param>
        /// <returns></returns>
        bool DoLogon(EFTLogonRequest request);

        /// <summary>Initiate a PC-EFTPOS transaction.</summary>
        /// <param name="request">An <see cref="EFTTransactionRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoTransaction(EFTTransactionRequest request);

        /// <summary>Initiate a PC-EFTPOS get last transaction.</summary>
        /// <param name="request">An <see cref="EFTGetLastTransactionRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoGetLastTransaction(EFTGetLastTransactionRequest request);

        /// <summary>Initiate a PC-EFTPOS duplicate receipt request.</summary>
        /// <param name="request">An <see cref="EFTReprintReceiptRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoDuplicateReceipt(EFTReprintReceiptRequest request);

        /// <summary>Send a key to PC-EFTPOS.</summary>
        /// <param name="request">An <see cref="EFTSendKeyRequest" />.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoSendKey(EFTSendKeyRequest request);

        /// <summary>Send a key to PC-EFTPOS.</summary>
        /// <param name="key">An <see cref="EFTPOSKey" />.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoSendKey(EFTPOSKey key);

        /// <summary>Send entry data to PC-EFTPOS.</summary>
        /// <param name="data">Entry data collected by the POS.</param>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("DoSendEntryData is deprecated, please use DoSendKey(EFTPOSKey.Authorise, data) instead.")]
        bool DoSendEntryData(string data);

#pragma warning disable CS0618
        /// <summary>Send a request to PC-EFTPOS to open the control panel.</summary>
        /// <param name="request">An <see cref="ControlPanelRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("Please use DoDisplayControlPanel(EFTControlPanelRequest request)")]
        bool DoDisplayControlPanel(ControlPanelRequest request);
#pragma warning restore CS0618

        /// <summary>Send a request to PC-EFTPOS to open the control panel.</summary>
        /// <param name="request">An <see cref="ControlPanelRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoDisplayControlPanel(EFTControlPanelRequest request);

        /// <summary>Send a request to PC-EFTPOS to initiate a settlement.</summary>
        /// <param name="request">An <see cref="EFTSettlementRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoSettlement(EFTSettlementRequest request);

        /// <summary>Send a request to PC-EFTPOS for a PIN pad status.</summary>
        /// <param name="request">An <see cref="EFTStatusRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoStatus(EFTStatusRequest request);

        /// <summary>Send a request to PC-EFTPOS for a cheque authorization.</summary>
        /// <param name="request">An <see cref="EFTChequeAuthRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        bool DoChequeAuth(EFTChequeAuthRequest request);

        /// <summary>Send a request to PC-EFTPOS for a query card.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoQueryCard(EFTQueryCardRequest request);

#pragma warning disable CS0618
        /// <summary>Send a request to PC-EFTPOS for a query card.</summary>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("Please use DoQueryCard(EFTQueryCardRequest request)")]
        bool DoQueryCard(QueryCardRequest request);
#pragma warning restore CS0618

        /// <summary>Send a request to PC-EFTPOS to get password entry from the PIN pad.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoGetPassword(EFTGetPasswordRequest request);

        /// <summary>Send a request to PC-EFTPOS to pass a slave cmd to the PIN pad.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoSlaveCommand(string command);

        /// <summary>Send a request to PC-EFTPOS for a merchant config.</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoConfigMerchant(EFTConfigureMerchantRequest request);

        /// <summary>Send a cloud logon request to PC-EFTPOS .</summary>
        /// <returns>FALSE if an error occured.</returns>
        bool DoCloudLogon(EFTCloudLogonRequest request);

        /// <summary>Clears the request in progress flag.</summary>
        /// <returns></returns>
        void ClearRequestInProgress();
        #endregion

        #region Events

        /// <summary>Fired when a client socket is terminated.</summary>
        event EventHandler<SocketEventArgs> OnTerminated;
        /// <summary>Fired when a socket error occurs.</summary>
        event EventHandler<SocketEventArgs> OnSocketFail;
        /// <summary>Fired when a get config merchant result is received.</summary>
        event EventHandler<SocketEventArgs> OnTcpSend;
        /// <summary>Fired when a get config merchant result is received.</summary>
        event EventHandler<SocketEventArgs> OnTcpReceive;
        /// <summary>Fired when a logging event occurs.</summary>
        event EventHandler<PCEFTPOS.EFTClient.IPInterface.LogEventArgs> OnLog;

        /// <summary>Fired when a display is received.</summary>
        event EventHandler<EFTEventArgs<EFTDisplayResponse>> OnDisplay;
        /// <summary>Fired when a receipt is received.</summary>
        event EventHandler<EFTEventArgs<EFTReceiptResponse>> OnReceipt;
        /// <summary>Fired when a logon result is received.</summary>
        event EventHandler<EFTEventArgs<EFTLogonResponse>> OnLogon;
        /// <summary>Fired when a transaction result is received.</summary>
        event EventHandler<EFTEventArgs<EFTTransactionResponse>> OnTransaction;
        /// <summary>Fired when a get last transaction result is received.</summary>
        event EventHandler<EFTEventArgs<EFTGetLastTransactionResponse>> OnGetLastTransaction;
        /// <summary>Fired when a duplicate receipt result is received.</summary>
        event EventHandler<EFTEventArgs<EFTReprintReceiptResponse>> OnDuplicateReceipt;
        /// <summary>Fired when a display control panel result is received.</summary>
        event EventHandler<EFTEventArgs<EFTControlPanelResponse>> OnDisplayControlPanel;
        /// <summary>Fired when a settlement result is received.</summary>
        event EventHandler<EFTEventArgs<EFTSettlementResponse>> OnSettlement;
        /// <summary>Fired when a status result is received.</summary>
        event EventHandler<EFTEventArgs<EFTStatusResponse>> OnStatus;
        /// <summary>Fired when a cheque authorization result is received.</summary>
        event EventHandler<EFTEventArgs<EFTChequeAuthResponse>> OnChequeAuth;
        /// <summary>Fired when a query card result is received.</summary>
        event EventHandler<EFTEventArgs<EFTQueryCardResponse>> OnQueryCard;
        /// <summary>Fired when a get password result is received.</summary>
        event EventHandler<EFTEventArgs<EFTGetPasswordResponse>> OnGetPassword;
        /// <summary>Fired when a get slave result is received.</summary>
        event EventHandler<EFTEventArgs<EFTSlaveResponse>> OnSlave;
        /// <summary>Fired when a get config merchant result is received.</summary>
        event EventHandler<EFTEventArgs<EFTConfigureMerchantResponse>> OnConfigMerchant;
        #endregion
    }
}