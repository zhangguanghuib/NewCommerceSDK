using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
	/// <summary>
	/// Encapsulates the PC-EFTPOS TCP/IP interface using the async pattern
	/// </summary>
	public class EFTClientIPAsync : IEFTClientIPAsync
	{
		IMessageParser _parser;
		StringBuilder _recvBuf;

		/// <summary>
		/// Set to TRUE when our receive buffer is awaiting more data, FALSE otherwise.
		/// </summary>
		bool _recvBufWaiting;

		int _recvTickCount;
		EFTRequest _currentRequest;
		EFTRequest _currentStartTxnRequest;
		byte[] _buffer;

		ITcpSocketAsync _clientStream;

		public EFTClientIPAsync()
		{
			Initialise();
		}

		void Initialise()
		{
			_buffer = new byte[8192];
			_recvBuf = new StringBuilder();
			_recvBufWaiting = false;
			_recvTickCount = 0;
			_parser = new DefaultMessageParser();
		}

		public async Task<bool> ConnectAsync(string hostName, int hostPort, bool useSSL, bool useKeepAlive = false)
		{
			try
			{
				// If we already have an existing _clientStream we need to clean it up before creating a new one
				if (_clientStream != null)
				{
					_clientStream.LogLevel -= (int)LogLevel;
					_clientStream.OnLog -= _clientStream_OnLog;
					_clientStream.Close();
					_clientStream = null;
				}

				_hostName = hostName;
				_hostPort = hostPort;
				_useSSL = useSSL;
				_useKeepAlive = useKeepAlive;

				if (useSSL)
				{
					// Check if there are any custom root certificates to load
					var tcp = new TcpSocketSslAsync();
					try
					{
						var extns = new string[] { ".der", ".pem" };
						var certs = from f in Directory.EnumerateFiles(Directory.GetCurrentDirectory())
									where extns.Contains((new FileInfo(f)).Extension)
									select f;
						tcp.CustomeRootCerts = certs?.ToList();
					}
					catch (Exception ex)
					{
						Log(LogLevel.Error, tr => tr.Set($"Failed to get certs: {ex.Message}"));
					}

					_clientStream = tcp;
				}
				else
				{
					_clientStream = new TcpSocketAsync();
				}

				_clientStream.LogLevel = LogLevel;
				_clientStream.OnLog += _clientStream_OnLog;

				var connected = await _clientStream.ConnectAsync(hostName, hostPort, useSSL);
				Log(LogLevel.Info, tr => tr.Set($"Connected state: {connected}"));

				return connected;
			}
			catch (ObjectDisposedException ox)
			{
				Log(LogLevel.Error, tr => tr.Set($"Attempt to access disposed object - {hostName}:{hostPort} with SSL {useSSL}", ox));
				throw new ConnectionException(ox.Message, ox.InnerException);
			}
			catch (AuthenticationException ax)
			{
				Log(LogLevel.Error, tr => tr.Set($"Authentication failed - closing connection to {hostName}:{hostPort} with SSL {useSSL}", ax));
				throw new ConnectionException(ax.Message, ax.InnerException);
			}
			catch (Exception ex)
			{
				Log(LogLevel.Error, tr => tr.Set($"Error connecting to {hostName}:{hostPort} with SSL {useSSL}", ex));
				throw new ConnectionException(ex.Message, ex.InnerException);
			}
		}

		private void _clientStream_OnLog(object sender, LogEventArgs e)
		{
			Log(e.LogLevel, tr => tr.Set(e.Message, e.Exception));
		}


		public bool Disconnect()
		{
			//IsConnected = false;

			Log(LogLevel.Debug, tr => tr.Set("Disconnecting..."));

			try
			{
				if (_clientStream != null)
				{
					_clientStream.Close();
					_clientStream.OnLog -= _clientStream_OnLog;
				}
			}
			catch (Exception ex)
			{
				Log(LogLevel.Error, tr => tr.Set($"Error disconnecting", ex));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Polls the socket to determine the current connect state
		/// </summary>
		/// <returns>True if connected, false otherwise</returns>
		public async Task<bool> CheckConnectStateAsync()
		{
			if (_clientStream == null)
				return false;

			return await _clientStream.CheckConnectStateAsync();
		}


		/// <summary>
		/// Sends a request to the EFT-Client, and waits for the next EFT response of type T from the client. 
		/// All other response types are discarded.. This function is useful for request/response pairs 
		/// where other message types are not being handled (such as SetDialogRequest/SetDialogResponse).
		/// 
		/// Since receipts and dialog messages cannot be handled with this function, it is not 
		/// recommended for use with transaction, logon, settlement ets
		/// </summary>
		/// <remarks>
		/// This function will continue to wait until either a message is received, or an exception is thrown (e.g. socket disconnect, cancellationToken fires).
		/// It is important to ensure cancellationToken is configured correctly.
		/// </remarks>
		/// <typeparam name="T">The type of EFTResponse to wait for</typeparam>
		/// <param name="request">The <see cref="EFTRequest"/> to send</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests</param>
		/// <param name="member">Used for internal logging. Ignore</param>
		/// <returns> An EFTResponse if one could be read, otherwise null </returns>
		/// <exception cref="ConnectionException">The socket is closed.</exception>
		/// <exception cref="TaskCanceledException">The cancellation token was cancelled before the task completed</exception>
		public async Task<T> WriteRequestAndWaitAsync<T>(EFTRequest request, System.Threading.CancellationToken cancellationToken, [CallerMemberName] string member = "") where T : EFTResponse
		{
			if (!await WriteRequestAsync(request, member))
			{
				return null;
			}
			return await ReadResponseAsync<T>(cancellationToken);
		}


		/// <summary>Sends a request to the EFT-Client</summary>
		/// <param name="request">The <see cref="EFTRequest"/> to send</param>
		/// <param name="member">Used for internal logging. Ignore</param>
		/// <returns>FALSE if an error occurs</returns>
		public async Task<bool> WriteRequestAsync(EFTRequest request, [CallerMemberName] string member = "")
		{
			SetCurrentRequest(request);

			string msgString = "";
			try
			{
				msgString = (request is EFTPosAsPinpadRequest) ? _parser.EFTRequestToXMLString(request) : _parser.EFTRequestToString(request);
			}
			catch (Exception e)
			{
				Log(LogLevel.Error, tr => tr.Set($"An error occured parsing the request", e));
				throw;
			}

			Log(LogLevel.Debug, tr => tr.Set($"Tx {msgString}"));

			// Send the request string to the IP client.
			try
			{
				await _clientStream.WriteRequestAsync(msgString);
			}
			catch (Exception e)
			{
				Log(LogLevel.Error, tr => tr.Set($"An error occured sending the request", e));
				Disconnect();
				throw new ConnectionException(e.Message, e.InnerException);
			}
			return true;
		}

		/// <summary>
		/// Stores the current request and current start of transaction
		/// </summary>
		/// <param name="request"></param>
		private void SetCurrentRequest(EFTRequest request)
		{
			// Always set _currentRequest to the last request we send
			_currentRequest = request;

			if (request.GetIsStartOfTransactionRequest())
			{
				_currentStartTxnRequest = request;
			}
		}

		/// <summary> 
		/// Retrieves the next EFT response from the client
		/// </summary>
		/// <returns> An EFTResponse if one could be read, otherwise null </returns>
		/// <exception cref="ConnectionException">The socket is closed.</exception>
		public async Task<EFTResponse> ReadResponseAsync()
		{
			return await ReadResponseAsync(System.Threading.CancellationToken.None);
		}


		/// <summary> 
		/// Retrieves the next EFT response of type T from the client. All other response types are discarded.
		/// </summary>
		/// <remarks>
		/// This function will continue to wait until either a message is received, or an exception is thrown (e.g. socket disconnect, cancellationToken fires).
		/// It is important to ensure cancellationToken is configured correctly.
		/// </remarks>
		/// <typeparam name="T">The type of EFTResponse to wait for</typeparam>
		/// <param name="cancellationToken">The token to monitor for cancellation requests</param>
		/// <returns> An EFTResponse if one could be read, otherwise null </returns>
		/// <exception cref="ConnectionException">The socket is closed.</exception>
		/// <exception cref="TaskCanceledException">The cancellation token was cancelled before the task completed</exception>
		public async Task<T> ReadResponseAsync<T>(System.Threading.CancellationToken cancellationToken) where T : EFTResponse
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var r = await ReadResponseAsync(cancellationToken);
				if (r is T)
				{
					return r as T;
				}
			}
			return null;
		}

		/// <summary> 
		/// Retrieves the next EFT response from the client
		/// </summary>
		/// <param name="cancellationToken">The token to monitor for cancellation requests</param>
		/// <returns> An EFTResponse if one could be read, otherwise null </returns>
		/// <exception cref="ConnectionException">The socket is closed.</exception>
		/// <exception cref="TaskCanceledException">The task was cancelled by cancellationToken</exception>
		public async Task<EFTResponse> ReadResponseAsync(System.Threading.CancellationToken cancellationToken)
		{
			// Clear the receive buffer if we have been waiting for more than 
			// 5 seconds for the remaining parts of the message to arrive
			var tc = System.Environment.TickCount;
			if (_recvBufWaiting && tc - _recvTickCount > 5000 && _recvBuf.Length > 0)
			{
				Log(LogLevel.Debug, tr => tr.Set($"Data is being cleared from the buffer due to a timeout. Content {_recvBuf.ToString()}"));
				_recvBufWaiting = false;
				_recvBuf.Clear();
			}

			// Only try to read more data if our receive buffer is empty
			Log(LogLevel.Debug, tr => tr.Set($"ReadResponseAsync() _recvBuf.Length={_recvBuf.Length} _recvBufWaiting={_recvBufWaiting}"));
			if (_recvBuf.Length == 0 || _recvBufWaiting == true)
			{
				var bytesRead = 0;
				try
				{
					bytesRead = await _clientStream.ReadResponseAsync(_buffer, cancellationToken);
					_recvTickCount = System.Environment.TickCount;
				}
				catch (TaskCanceledException e)
				{
					Log(LogLevel.Debug, tr => tr.Set($"Socket read cancelled", e));
					throw e;
				}
				catch (Exception e)
				{
					Log(LogLevel.Error, tr => tr.Set($"An error occured sending the request", e));
					Disconnect();
					throw new ConnectionException(e.Message, e.InnerException);
				}

				// Check for socket closed
				if (bytesRead <= 0)
				{
					Log(LogLevel.Error, tr => tr.Set("Recv 0 bytes. Socket closed"));
					Disconnect();
					throw new ConnectionException("Socket closed.");
				}

				var msgString = DirectEncoding.DIRECT.GetString(_buffer, 0, bytesRead);
				Log(LogLevel.Info, tr => tr.Set($"Rx {msgString}"));
				// Append receive data to our buffer
				_recvBuf.Append(msgString);
			}

			// Keep parsing until no more characters
			try
			{
				bool isXMLFormatted = false;
				int index = 0;
				while (index < _recvBuf.Length)
				{
					// If the current char isn't #/& then keep cycling through the message until we find one
					if (_recvBuf[index] != (byte)'#' && !(isXMLFormatted = _recvBuf[index] == (byte)'&'))
					{
						index++;
						continue;
					}

					// We have a valid start char, check for length. 
					index++;

					int lengthSize = isXMLFormatted ? 6 : 4;

					// Check that we have enough bytes to validate length, if not wait for more
					if (_recvBuf.Length < index + lengthSize)
					{
						Log(LogLevel.Debug, tr => tr.Set($"Unable to validate message header. Waiting for more data. Length:{_recvBuf.Length} Required:{index + lengthSize + 1}"));
						_recvBufWaiting = true;
						break;
					}

					// Try to get the length of the new message. If it's not a valid length 
					// we might have some corrupt data, keep checking for a valid message
					var lengthStr = _recvBuf.Substring(index, lengthSize);
                    int length;
					if (!int.TryParse(lengthStr, out length) || length <= (lengthSize + 1))
					{
						Log(LogLevel.Error, tr => tr.Set($"Invalid length. Content:{lengthStr}"));
						continue;
					}

					// We have a valid length
					index += lengthSize;

					// If the defined message length is > our current buffer size, wait for more data
					if (_recvBuf.Length < index + length - (lengthSize + 1))
					{
						Log(LogLevel.Debug, tr => tr.Set($"Buffer is less than the indicate length. Waiting for more data. Length:{_recvBuf.Length < index} Required:{length - (lengthSize + 1)}"));
						_recvBuf = _recvBuf.Remove(0, index - (lengthSize + 1));
						_recvBufWaiting = true;
						break;
					}

					// We have a valid response
					_recvBufWaiting = false;
					var response = _recvBuf.Substring(index, length - (lengthSize + 1));
					index += (length - (lengthSize + 1));
					_recvBuf.Remove(0, index);

					// Process the response
					EFTResponse eftResponse = null;
					try
					{
						//if (response.Equals("3M") || response.Equals("3C")) // FOR PREPRINT TIMEOUT
						//if (response.Substring(0,2).Equals("3R")) // FOR PRINT TIMEOUT
						//	System.Threading.Thread.Sleep(61000);
						eftResponse = (isXMLFormatted) ? _parser.XMLStringToEFTResponse(response) : _parser.StringToEFTResponse(response);

						// If we have an EFTResponse we need to return it. 
						if (eftResponse != null)
						{
							// Print requests need a response
							if (eftResponse is EFTReceiptResponse)
							{
								await WriteRequestAsync(new EFTReceiptRequest());
							}
							//DialogUIHandler needs to be notified of display messages
							else if (eftResponse is EFTDisplayResponse && DialogUIHandlerAsync != null)
							{
								await DialogUIHandlerAsync.HandleDisplayResponseAsync(eftResponse as EFTDisplayResponse);
							}
							else if (eftResponse.GetType() == _currentStartTxnRequest?.GetPairedResponseType() && DialogUIHandlerAsync != null)
							{
								await DialogUIHandlerAsync.HandleCloseDisplayAsync();
							}


							return eftResponse;
						}
					}
					catch (Exception e)
					{
						Log(LogLevel.Error, tr => tr.Set("Error parsing response string", e));
					}
				}

				// Clear our buffer if we are all done (this shouldn't happen)
				if (index == _recvBuf.Length)
				{
					_recvBufWaiting = false;
					_recvBuf.Clear();
				}
			}
			catch (Exception ex)
			{
				Log(LogLevel.Error, tr => tr.Set($"Exception (ReceiveEFTResponse): {ex.Message}", ex));
				throw ex;
			}

			return null;
		}

		void Log(LogLevel level, Action<TraceRecord> traceAction, [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
		{
			// Check if this log level is enabled and client is subscribed to the OnLog event
			if (OnLog == null || (int)LogLevel >= (int)level)
			{
				return;
			}

			TraceRecord tr = new TraceRecord() { Level = level };
			traceAction(tr);


			// StringBuild is faster than $"{member}() line {line}: {tr.Message}"
			var sb = new StringBuilder(member.Length + tr.Message.Length + 100);
			sb.Append(member);
			sb.Append("() line ");
			sb.Append(line);
			sb.Append(": ");
			sb.Append(tr.Message);

			OnLog?.Invoke(this, new LogEventArgs() { LogLevel = level, Message = sb.ToString(), Exception = tr.Exception });
		}


		#region Events

		/// <summary>Fired when a logging event occurs.</summary>
		public event EventHandler<PCEFTPOS.EFTClient.IPInterface.LogEventArgs> OnLog;

		#endregion

		#region Properties

		/// <summary>The IP host name of the PC-EFTPOS IP Client.</summary>
		/// <value>Type: <see cref="System.String" /><para>The IP address or host name of the EFT Client IP interface.</para></value>
		/// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
		string _hostName = "127.0.0.1";
		public string HostName => _hostName;

		/// <summary>The IP port of the PC-EFTPOS IP Client.</summary>
		/// <value>Type: <see cref="System.Int32" /><para>The listening port of the EFT Client IP interface.</para></value>
		/// <remarks>The setting of this property is required.<para>See <see cref="EFTClientIP.Connect"></see> example.</para></remarks>
		int _hostPort = 6001;
		public int HostPort => _hostPort;

		/// <summary>Indicates whether to use SSL encryption.</summary>
		/// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
		bool _useSSL = true;
		public bool UseSSL => _useSSL;

		/// <summary>Indicates whether to allow TCP keep-alives.</summary>
		/// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
		bool _useKeepAlive = true;
		public bool UseKeepAlive => _useKeepAlive;

		/// <summary>Indicates whether there is a request currently in progress.</summary>
		public bool IsRequestInProgress { get; }

		/// <summary>
		/// Returns the connected state as of the last 
		/// 
		/// Gets a value that indicates whether a System.Net.Sockets.Socket is connected
		/// to a remote host as of the last Overload:System.Net.Sockets.Socket.Send or Overload:System.Net.Sockets.Socket.Receive
		/// operation.
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return _clientStream?.IsConnected ?? false;
			}
		}

		/// <summary> When TRUE, the SynchronizationContext will be captured from requests and used to call events</summary>
		public bool UseSynchronizationContextForEvents { get; set; } = true;

		/// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
		public LogLevel LogLevel { get; set; } = LogLevel.All;


		IDialogUIHandlerAsync dialogUIHandlerAsync = null;
		/// <summary>
		/// An interface for a UI that implements the PC-EFTPOS dialog messages.
		/// Uses IEFTClientIPAsync to send key press messages to the EFT-Client
		/// </summary>
		public IDialogUIHandlerAsync DialogUIHandlerAsync
		{
			get
			{
				return dialogUIHandlerAsync;
			}
			set
			{
				dialogUIHandlerAsync = value;
				if (dialogUIHandlerAsync.EFTClientIPAsync == null)
				{
					dialogUIHandlerAsync.EFTClientIPAsync = this;
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

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_clientStream?.Close();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~EFTClientIPAsync2() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		#region Obsolete
		[Obsolete("DisconnectAsync() is deprecated, please use Disconnect() instead.")]
		public bool DisconnectAsync()
		{
			return Disconnect();
		}

		[Obsolete("Close() is deprecated, please use Disconnect() instead.")]
		public void Close()
		{
			Disconnect();
		}
		#endregion
	}

}
