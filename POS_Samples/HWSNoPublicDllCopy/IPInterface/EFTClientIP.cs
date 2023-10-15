using PCEFTPOS.EFTClient.IPInterface.Slave;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PCEFTPOS.EFTClient.IPInterface
{
    #region EFTEventArgs
    /// <summary>
    /// EFT Event object. Sent when an EFT event occurs
    /// </summary>
    /// <typeparam name="TEFTResponse"></typeparam>
    public class EFTEventArgs<TEFTResponse> where TEFTResponse : EFTResponse
    {
        // public event EventHandler<>
        public EFTEventArgs(TEFTResponse response)
        {
            Response = response;
        }

        public TEFTResponse Response { get; set; }
    }

    #endregion

    #region SocketEventArgs

    /// <summary>EFT Client IP error types.</summary>
    public enum EFTClientIPErrorType
    {
        /// <summary>A socket connect error occurred.</summary>
        Socket_ConnectError,
        /// <summary>A socket receive error occurred.</summary>
        Socket_ReceiveError,
        /// <summary>A socket send error occurred.</summary>
        Socket_SendError,
        /// <summary>A general socket error occurred.</summary>
        Socket_GeneralError,
        /// <summary>An error occured while parsing a received message..</summary>
        Client_ParseError
    }

    /// <summary>EFT Client IP event object. Sent when an event occurs.</summary>
    /// <remarks>Note that only one response property is valid per event type.</remarks>
    public class SocketEventArgs : EventArgs
    {
        /// <summary>The error message describing the error that occurred.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        /// <remarks>Valid for a OnSocketFail.</remarks>
        public string ErrorMessage { get; set; }

        /// <summary>The TCP/IP message that was sent or received.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        /// <remarks>Valid for a OnTcpSend and OnTcpReceive.</remarks>
        public string TcpMessage { get; set; }

        /// <summary>The type of error that occurred.</summary>
        /// <value>Type: <see cref="EFTClientIPErrorType" /></value>
        /// <remarks>Valid for a OnSocketFail.</remarks>
        public EFTClientIPErrorType ErrorType { get; set; }
    }

    #endregion

    /// <summary>
    /// Encapsulates the PC-EFTPOS TCP/IP interface using a request/event pattern
    /// <remarks>Where possible use <see cref="EFTClientIPAsync"/></remarks>
    /// </summary>
    public class EFTClientIP : IEFTClientIP
    {
        #region Data

        SynchronizationContext syncContext;
        IMessageParser _parser;
        ITcpSocket socket;
        EFTRequest currentRequest;
        AutoResetEvent hideDialogEvent;
        bool gotResponse;
        string recvBuf;
        int recvTickCount;
        bool requestInProgess;

        #endregion

        #region Constructors

        /// <summary>Construct an EFTClientIP object.</summary>
        public EFTClientIP()
        {
            Initialise();
        }

        public void Dispose()
        {
            socket?.Dispose();
        }

        #endregion

        #region Public Methods

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
        public bool Connect()
        {
            socket.HostName = HostName;
            socket.HostPort = HostPort;
            socket.UseKeepAlive = UseKeepAlive;
            socket.UseSSL = UseSSL;
            return socket.Start();
        }

        /// <summary>Disconnect from the PC-EFTPOS client IP interface.</summary>
        public void Disconnect()
        {
            socket.Stop();
        }

        /// <summary>Sends a request to the EFT-Client</summary>
        /// <param name="request">The <see cref="EFTRequest"/> to send</param>
        /// <param name="member">Used for internal logging. Ignore</param>
        /// <returns>FALSE if an error occurs</returns>
        public bool DoRequest(EFTRequest request, [CallerMemberName] string member = "")
        {
            SetCurrentRequest(request);
            Log(LogLevel.Info, tr => tr.Set($"Request via {member}"));

            // Save the current synchronization context so we can use it to send events 
            syncContext = System.Threading.SynchronizationContext.Current;

            if (requestInProgess)
            {
                Log(LogLevel.Error, tr => tr.Set("Ignored, request already in progress"));
                return false;
            }

            if (!IsConnected)
            {
                Log(LogLevel.Info, tr => tr.Set($"Not connected in {member} request, trying to connect now..."));
                if (!Connect())
                {
                    Log(LogLevel.Error, tr => tr.Set("Connect failed"));
                    return false;
                }
            }

            var r = SendIPClientRequest(request);
            requestInProgess = r;
            return r;
        }

        /// <summary>Hide the PC-EFTPOS dialogs.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoHideDialogs()
        {
            hideDialogEvent.Reset();

            if (!DoRequest(new SetDialogRequest() { DialogType = DialogType.Hidden }))
                return false;

            var r = hideDialogEvent.WaitOne(2000);
            requestInProgess = false;
            return r;
        }


        /// <summary>Initiate a PC-EFTPOS logon using default values.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoLogon()
        {
            return DoRequest(new EFTLogonRequest());
        }

        /// <summary>Initiate a PC-EFTPOS logon.</summary>
        /// <param name="request">An <see cref="EFTLogonRequest" /> object.</param>
        /// <returns></returns>
        public bool DoLogon(EFTLogonRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Initiate a PC-EFTPOS transaction.</summary>
        /// <param name="request">An <see cref="EFTTransactionRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoTransaction(EFTTransactionRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Initiate a PC-EFTPOS get last transaction.</summary>
        /// <param name="request">An <see cref="EFTGetLastTransactionRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoGetLastTransaction(EFTGetLastTransactionRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Initiate a PC-EFTPOS duplicate receipt request.</summary>
        /// <param name="request">An <see cref="EFTReprintReceiptRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoDuplicateReceipt(EFTReprintReceiptRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a key to PC-EFTPOS.</summary>
        /// <param name="request">An <see cref="EFTSendKeyRequest" />.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoSendKey(EFTSendKeyRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a key to PC-EFTPOS.</summary>
        /// <param name="key">An <see cref="EFTPOSKey" />.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoSendKey(EFTPOSKey key)
        {
            return DoRequest(new EFTSendKeyRequest() { Key = key });
        }

        /// <summary>Send entry data to PC-EFTPOS.</summary>
        /// <param name="data">Entry data collected by the POS.</param>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("DoSendEntryData is deprecated, please use DoSendKey(EFTPOSKey.Authorise, data) instead.")]
        public bool DoSendEntryData(string data)
        {
            return DoRequest(new EFTSendKeyRequest() { Key = EFTPOSKey.Authorise, Data = data });
        }

        /// <summary>Send a request to PC-EFTPOS to open the control panel.</summary>
        /// <param name="request">An <see cref="ControlPanelRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoDisplayControlPanel(EFTControlPanelRequest request)
        {
            return DoRequest(request);
        }

#pragma warning disable CS0618
        /// <summary>Send a request to PC-EFTPOS to open the control panel.</summary>
        /// <param name="request">An <see cref="ControlPanelRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("Please use bool DoDisplayControlPanel(EFTControlPanelRequest request)")]
        public bool DoDisplayControlPanel(ControlPanelRequest request)
        {
            return DoRequest(request);
        }
#pragma warning restore CS0618

        /// <summary>Send a request to PC-EFTPOS to initiate a settlement.</summary>
        /// <param name="request">An <see cref="EFTSettlementRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoSettlement(EFTSettlementRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a request to PC-EFTPOS for a PIN pad status.</summary>
        /// <param name="request">An <see cref="EFTStatusRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoStatus(EFTStatusRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a request to PC-EFTPOS for a cheque authorization.</summary>
        /// <param name="request">An <see cref="EFTChequeAuthRequest" /> object.</param>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoChequeAuth(EFTChequeAuthRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a request to PC-EFTPOS for a query card.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoQueryCard(EFTQueryCardRequest request)
        {
            return DoRequest(request);
        }

#pragma warning disable CS0618
        /// <summary>Send a request to PC-EFTPOS for a query card.</summary>
        /// <returns>FALSE if an error occured.</returns>
        [Obsolete("Please use bool DoQueryCard(EFTQueryCardRequest request)")]
        public bool DoQueryCard(QueryCardRequest request)
        {
            return DoRequest(request);
        }
#pragma warning restore CS0618

        /// <summary>Send a request to PC-EFTPOS to get password entry from the PIN pad.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoGetPassword(EFTGetPasswordRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a request to PC-EFTPOS to pass a slave cmd to the PIN pad.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoSlaveCommand(string command)
        {
            return DoRequest(new EFTSlaveRequest() { RawCommand = command });
        }

        /// <summary>Send a request to PC-EFTPOS for a merchant config.</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoConfigMerchant(EFTConfigureMerchantRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Send a cloud logon request to PC-EFTPOS .</summary>
        /// <returns>FALSE if an error occured.</returns>
        public bool DoCloudLogon(EFTCloudLogonRequest request)
        {
            return DoRequest(request);
        }

        /// <summary>Clears the request in progress flag.</summary>
        /// <returns></returns>
        public void ClearRequestInProgress()
        {
            requestInProgess = false;
        }

        #endregion

        #region Methods

        void Log(LogLevel level, Action<TraceRecord> traceAction, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            // Check if this log level is enabled and client is subscribed to the OnLog event
            if (OnLog == null || this.LogLevel >= level)
            {
                return;
            }

            TraceRecord tr = new TraceRecord() { Level = level };
            traceAction(tr);
            string message = $"{member}() line {line}: {tr.Message}";

            if (UseSynchronizationContextForEvents && syncContext != null && syncContext != SynchronizationContext.Current)
            {
                syncContext?.Post(o => OnLog?.Invoke(this, new LogEventArgs() { LogLevel = level, Message = message, Exception = tr.Exception }), null);
            }
            else
            {
                OnLog?.Invoke(this, new LogEventArgs() { LogLevel = level, Message = message, Exception = tr.Exception });
            }
        }

        void Initialise()
        {
            recvBuf = "";
            recvTickCount = 0;
            _parser = new DefaultMessageParser();

            socket = new TcpSocket(HostName, HostPort);
            socket.OnTerminated += new TcpSocketEventHandler(_OnTerminated);
            socket.OnDataWaiting += new TcpSocketEventHandler(_OnDataWaiting);
            socket.OnError += new TcpSocketEventHandler(_OnError);
            socket.OnSend += new TcpSocketEventHandler(_OnSend);

            hideDialogEvent = new AutoResetEvent(false);
        }

        bool WaitForIPClientResponse(AutoResetEvent ResetEvent)
        {
            gotResponse = false;
            ResetEvent.WaitOne(500, false);
            requestInProgess = false;
            return gotResponse;
        }

        void ProcessEFTResponse(EFTResponse response)
        {
            try
            {


                if (response == null)
                {
                    Log(LogLevel.Error, tr => tr.Set($"Unable to handle null param {nameof(response)}"));

                }
                else if (response is SetDialogResponse)
                {
                    if (currentRequest is SetDialogRequest)
                    {
                        gotResponse = true;
                        hideDialogEvent.Set();
                    }

                }
                else if (response is EFTReceiptResponse)
                {
                    SendReceiptAcknowledgement();
                    Log(LogLevel.Info, tr => tr.Set($"IsPrePrint={((EFTReceiptResponse)response).IsPrePrint}"));

                    if ((response as EFTReceiptResponse).IsPrePrint == false)
                    {
                        FireClientResponseEvent(nameof(OnReceipt), OnReceipt, new EFTEventArgs<EFTReceiptResponse>(response as EFTReceiptResponse));
                    }

                }
                else if (response is EFTDisplayResponse)
                {
                    //DialogUIHandler.HandleDisplayResponse(r);
                    FireClientResponseEvent(nameof(OnDisplay), OnDisplay, new EFTEventArgs<EFTDisplayResponse>(response as EFTDisplayResponse));

                }
                else if (response is EFTLogonResponse)
                {
                    FireClientResponseEvent(nameof(OnLogon), OnLogon, new EFTEventArgs<EFTLogonResponse>(response as EFTLogonResponse));
                }
                else if (response is EFTCloudLogonResponse)
                {
                    FireClientResponseEvent(nameof(OnCloudLogon), OnCloudLogon, new EFTEventArgs<EFTCloudLogonResponse>(response as EFTCloudLogonResponse));
                }
                else if (response is EFTTransactionResponse)
                {
                    FireClientResponseEvent(nameof(OnTransaction), OnTransaction, new EFTEventArgs<EFTTransactionResponse>(response as EFTTransactionResponse));
                }
                else if (response is EFTGetLastTransactionResponse)
                {
                    FireClientResponseEvent(nameof(OnGetLastTransaction), OnGetLastTransaction, new EFTEventArgs<EFTGetLastTransactionResponse>(response as EFTGetLastTransactionResponse));
                }
                else if (response is EFTReprintReceiptResponse)
                {
                    FireClientResponseEvent(nameof(OnDuplicateReceipt), OnDuplicateReceipt, new EFTEventArgs<EFTReprintReceiptResponse>(response as EFTReprintReceiptResponse));
                }
                else if (response is EFTControlPanelResponse)
                {
                    FireClientResponseEvent(nameof(OnDisplayControlPanel), OnDisplayControlPanel, new EFTEventArgs<EFTControlPanelResponse>(response as EFTControlPanelResponse));
                }
                else if (response is EFTSettlementResponse)
                {
                    FireClientResponseEvent(nameof(OnSettlement), OnSettlement, new EFTEventArgs<EFTSettlementResponse>(response as EFTSettlementResponse));
                }
                else if (response is EFTStatusResponse)
                {
                    FireClientResponseEvent(nameof(OnStatus), OnStatus, new EFTEventArgs<EFTStatusResponse>(response as EFTStatusResponse));
                }
                else if (response is EFTQueryCardResponse)
                {
                    FireClientResponseEvent(nameof(OnQueryCard), OnQueryCard, new EFTEventArgs<EFTQueryCardResponse>(response as EFTQueryCardResponse));
                }
                else if (response is EFTChequeAuthResponse)
                {
                    FireClientResponseEvent(nameof(OnChequeAuth), OnChequeAuth, new EFTEventArgs<EFTChequeAuthResponse>(response as EFTChequeAuthResponse));
                }
                else if (response is EFTGetPasswordResponse)
                {
                    FireClientResponseEvent(nameof(OnGetPassword), OnGetPassword, new EFTEventArgs<EFTGetPasswordResponse>(response as EFTGetPasswordResponse));
                }
                else if (response is EFTSlaveResponse)
                {
                    FireClientResponseEvent(nameof(OnSlave), OnSlave, new EFTEventArgs<EFTSlaveResponse>(response as EFTSlaveResponse));
                }
                else if (response is EFTConfigureMerchantResponse)
                {
                    FireClientResponseEvent(nameof(OnConfigMerchant), OnConfigMerchant, new EFTEventArgs<EFTConfigureMerchantResponse>(response as EFTConfigureMerchantResponse));
                }
                else if (response is EFTClientListResponse)
                {
                    FireClientResponseEvent(nameof(OnClientList), OnClientList, new EFTEventArgs<EFTClientListResponse>(response as EFTClientListResponse));
                }
                else
                {
                    Log(LogLevel.Error, tr => tr.Set($"Unknown response type", response));
                }
            }
            catch (Exception Ex)
            {
                Log(LogLevel.Error, tr => tr.Set($"Unhandled error in {nameof(ProcessEFTResponse)}", Ex));
            }
        }

        void SendReceiptAcknowledgement()
        {
            socket.Send("#00073 ");
        }



        bool SendIPClientRequest(EFTRequest eftRequest)
        {
            // Store current request.
            this.currentRequest = eftRequest;

            // Build request
            var requestString = "";

            try
            {
                requestString = _parser.EFTRequestToString(eftRequest);
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, tr => tr.Set($"An error occured parsing the request", e));
                throw;
            }

            Log(LogLevel.Debug, tr => tr.Set($"Tx {requestString}"));

            // Send the request string to the IP client.
            return socket.Send(requestString);
        }

        private void SetCurrentRequest(EFTRequest request)
        {
            // Always set _currentRequest to the last request we send
            currentRequest = request;

            if (request.GetIsStartOfTransactionRequest())
            {
                _currentStartTxnRequest = request;
            }
        }
        #endregion

        #region Parse response

        bool ReceiveEFTResponse(byte[] data)
        {
            // Clear the receive buffer if 5 seconds has lapsed since the last message
            var tc = System.Environment.TickCount;
            if (tc - recvTickCount > 5000)
            {
                Log(LogLevel.Debug, tr => tr.Set($"Data is being cleared from the buffer due to a timeout. Content {recvBuf.ToString()}"));
                recvBuf = "";
            }
            recvTickCount = System.Environment.TickCount;

            // Append receive data to our buffer
            recvBuf += System.Text.Encoding.ASCII.GetString(data, 0, data.Length);

            // Keep parsing until no more characters
            try
            {
                int index = 0;
                while (index < recvBuf.Length)
                {
                    // Look for start character
                    if (recvBuf[index] == (byte)'#')
                    {
                        // Check that we have enough bytes to validate length, if not wait for more
                        if (recvBuf.Length < index + 5)
                        {
                            recvBuf = recvBuf.Substring(index);
                            break;
                        }

                        // We have enough bytes to check for length
                        index++;

                        // Try to get the length of the new message. If it's not a valid length 
                        // we might have some corrupt data, keep checking for a valid message
                        int length;
                        if (!int.TryParse(recvBuf.Substring(index, 4), out length) || length <= 5)
                        {
                            continue;
                        }

                        // We have a valid length
                        index += 4;

                        // If our buffer doesn't contain enough data, wait for more 
                        if (recvBuf.Length < index + length - 5)
                        {
                            recvBuf = recvBuf.Substring(index - 5);
                            continue;
                        }

                        // We have a valid response
                        var response = recvBuf.Substring(index, length - 5);
                        FireOnTcpReceive(response);

                        // Process the response
                        EFTResponse eftResponse = null;
                        try
                        {
                            eftResponse = _parser.StringToEFTResponse(response);
                            ProcessEFTResponse(eftResponse);
                            if (eftResponse.GetType() == _currentStartTxnRequest?.GetPairedResponseType())
                            {
                                dialogUIHandler.HandleCloseDisplay();
                            }
                        }
                        catch (ArgumentException argumentException)
                        {
                            Log(LogLevel.Error, tr => tr.Set("Error parsing response string", argumentException));
                        }


                        index += length - 5;
                    }

                    // Clear our buffer if we are all done
                    if (index == recvBuf.Length)
                    {
                        recvBuf = "";
                    }
                }
            }
            catch (Exception ex)
            {
                // Fail gracefully.
                FireOnSocketFailEvent(EFTClientIPErrorType.Client_ParseError, ex.Message);
                Log(LogLevel.Error, tr => tr.Set($"Exception (ReceiveEFTResponse): {ex.Message}", ex));
                return false;
            }

            return true;
        }

        #endregion

        #region Event Handlers

        void _OnError(object sender, TcpSocketEventArgs e)
        {
            EFTClientIPErrorType errorType = EFTClientIPErrorType.Socket_GeneralError;

            Log(LogLevel.Error, tr => tr.Set($"OnError: {e.Error}"));
            switch (e.ExceptionType)
            {
                case TcpSocketExceptionType.ConnectException:
                    errorType = EFTClientIPErrorType.Socket_ConnectError;
                    break;
                case TcpSocketExceptionType.GeneralException:
                    errorType = EFTClientIPErrorType.Socket_GeneralError;
                    break;
                case TcpSocketExceptionType.ReceiveException:
                    errorType = EFTClientIPErrorType.Socket_ReceiveError;
                    break;
                case TcpSocketExceptionType.SendException:
                    errorType = EFTClientIPErrorType.Socket_SendError;
                    break;
            }

            FireOnSocketFailEvent(errorType, e.Error);
        }
        void _OnDataWaiting(object sender, TcpSocketEventArgs e)
        {
            Log(LogLevel.Debug, tr => tr.Set($"Rx>>{System.Text.ASCIIEncoding.ASCII.GetString(e.Bytes)}<<"));
            ReceiveEFTResponse(e.Bytes);
        }
        void _OnTerminated(object sender, TcpSocketEventArgs e)
        {
            FireOnTerminatedEvent(e.Error);
        }
        void _OnSend(object sender, TcpSocketEventArgs e)
        {
            FireOnTcpSend(e.Message);
        }

        #endregion

        #region Event Firers

        void FireClientResponseEvent<TEFTResponse>(string name, EventHandler<EFTEventArgs<TEFTResponse>> eventHandler, EFTEventArgs<TEFTResponse> args) where TEFTResponse : EFTResponse
        {
            Log(LogLevel.Info, tr => tr.Set($"Handle {name}"));
            requestInProgess = false;

            var tmpEventHandler = eventHandler;
            if (tmpEventHandler != null)
            {
                if (UseSynchronizationContextForEvents && syncContext != null && syncContext != SynchronizationContext.Current)
                {
                    syncContext.Post(o => tmpEventHandler.Invoke(this, args), null);
                }
                else
                {
                    tmpEventHandler.Invoke(this, args);
                }
            }
            else
            {
                throw (new Exception($"There is no event handler defined for {name}"));
            }
        }

        void FireOnTcpSend(string message)
        {
            OnTcpSend?.Invoke(this, new SocketEventArgs() { TcpMessage = message });
        }
        void FireOnTcpReceive(string message)
        {
            OnTcpReceive?.Invoke(this, new SocketEventArgs() { TcpMessage = message });
        }
        void FireOnTerminatedEvent(string message)
        {
            Log(LogLevel.Info, tr => tr.Set($"OnTerminated: {message}"));
            OnTerminated?.Invoke(this, new SocketEventArgs() { ErrorMessage = message, ErrorType = EFTClientIPErrorType.Socket_GeneralError });
        }
        void FireOnSocketFailEvent(EFTClientIPErrorType errorType, string message)
        {
            Log(LogLevel.Error, tr => tr.Set($"OnSocketFail: {message}"));
            OnSocketFail?.Invoke(this, new SocketEventArgs() { ErrorMessage = message, ErrorType = EFTClientIPErrorType.Socket_GeneralError });
        }

        /// <summary>
        /// Returns the connected state as of the last read or write operation. This does not necessarily represent 
        /// the current state of the connection. 
        /// To check the current socket state call <see cref="CheckConnectState()"/>
        /// </summary>
        public bool CheckConnectState()
        {
            if (socket == null)
                return false;
            return socket.CheckConnectState();
        }

        #endregion

        #region Properties

        /// <summary>The IP host name of the PC-EFTPOS IP Client.</summary>
        /// <value>Type: <see cref="System.String" /><para>The IP address or host name of the EFT Client IP interface.</para></value>
        /// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
        public string HostName { get; set; } = "127.0.0.1";

        /// <summary>The IP port of the PC-EFTPOS IP Client.</summary>
        /// <value>Type: <see cref="System.Int32" /><para>The listening port of the EFT Client IP interface.</para></value>
        /// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
        public int HostPort { get; set; } = 2011;

        /// <summary>Indicates whether to use SSL encryption.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        public bool UseSSL { get; set; } = false;

        /// <summary>Indicates whether to allow TCP keep-alives.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        public bool UseKeepAlive { get; set; } = false;

        /// <summary>Indicates whether there is a request currently in progress.</summary>
        public bool IsRequestInProgress { get { return requestInProgess; } }

        /// <summary>Indicates whether EFT Client is currently connected.</summary>
        public bool IsConnected { get { return socket.IsConnected; } }

        /// <summary> When TRUE, the SynchronizationContext will be captured from requests and used to call events</summary>
        public bool UseSynchronizationContextForEvents { get; set; } = true;

        /// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Off;

        IDialogUIHandler dialogUIHandler = null;
        private EFTRequest _currentStartTxnRequest;

        public IDialogUIHandler DialogUIHandler
        {
            get
            {
                return dialogUIHandler;
            }
            set
            {
                dialogUIHandler = value;
                if (dialogUIHandler.EFTClientIP == null)
                {
                    dialogUIHandler.EFTClientIP = this;
                }
            }
        }

        public IMessageParser Parser
        {
            get
            {
                return _parser;
            }
            set
            {
                _parser = value;
            }
        }


        #endregion

        #region Events

        /// <summary>Fired when a client socket is terminated.</summary>
        public event EventHandler<SocketEventArgs> OnTerminated;
        /// <summary>Fired when a socket error occurs.</summary>
        public event EventHandler<SocketEventArgs> OnSocketFail;
        /// <summary>Fired when a get config merchant result is received.</summary>
        public event EventHandler<SocketEventArgs> OnTcpSend;
        /// <summary>Fired when a get config merchant result is received.</summary>
        public event EventHandler<SocketEventArgs> OnTcpReceive;
        /// <summary>Fired when a logging event occurs.</summary>
        public event EventHandler<LogEventArgs> OnLog;

        /// <summary>Fired when a display is received.</summary>
        public event EventHandler<EFTEventArgs<EFTDisplayResponse>> OnDisplay;
        /// <summary>Fired when a receipt is received.</summary>
        public event EventHandler<EFTEventArgs<EFTReceiptResponse>> OnReceipt;
        /// <summary>Fired when a logon result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTLogonResponse>> OnLogon;
        /// <summary>Fired when a cloud logon result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTCloudLogonResponse>> OnCloudLogon;
        /// <summary>Fired when a transaction result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTTransactionResponse>> OnTransaction;
        /// <summary>Fired when a get last transaction result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTGetLastTransactionResponse>> OnGetLastTransaction;
        /// <summary>Fired when a duplicate receipt result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTReprintReceiptResponse>> OnDuplicateReceipt;
        /// <summary>Fired when a display control panel result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTControlPanelResponse>> OnDisplayControlPanel;
        /// <summary>Fired when a settlement result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTSettlementResponse>> OnSettlement;
        /// <summary>Fired when a status result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTStatusResponse>> OnStatus;
        /// <summary>Fired when a cheque authorization result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTChequeAuthResponse>> OnChequeAuth;
        /// <summary>Fired when a query card result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTQueryCardResponse>> OnQueryCard;
        /// <summary>Fired when a get password result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTGetPasswordResponse>> OnGetPassword;
        /// <summary>Fired when a get slave result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTSlaveResponse>> OnSlave;
        /// <summary>Fired when a get config merchant result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTConfigureMerchantResponse>> OnConfigMerchant;
        /// <summary>Fired whan a get client list result is received.</summary>
        public event EventHandler<EFTEventArgs<EFTClientListResponse>> OnClientList;
        #endregion
    }
}