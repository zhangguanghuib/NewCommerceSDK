using System;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
	class TcpSocketAsync : ITcpSocketAsync
	{
		private TcpClient _client = null;

		public async Task<bool> ConnectAsync(string hostName, int hostPort, bool keepAlive)
		{
			try
			{
				_client = new TcpClient();
				_client.Client.SetKeepAlive(keepAlive, 60000UL /*60 seconds*/, 1000UL /*1 sec*/);
				await _client.ConnectAsync(hostName, hostPort);
				return _client.Connected;
			}
			catch
			{
				Close();
				throw;
			}
		}

		public async Task<int> ReadResponseAsync(byte[] buffer, System.Threading.CancellationToken token)
		{
			try
			{
				// If we aren't using a CancellationToken we can just call ReadAsync directly
				if (token == System.Threading.CancellationToken.None)
				{
					return await _client.GetStream().ReadAsync(buffer, 0, buffer.Length, token);
				}
				// Else we need to jump through some hoops as ReadAsync doesn't return when token is cancelled
				else
				{
					var readTask = _client.GetStream().ReadAsync(buffer, 0, buffer.Length);
					var timeoutTask = Task.Delay(int.MaxValue, token); // Task.Delay handles CancellationToken correctly
					return await Task.Factory.ContinueWhenAny<int>(new Task[] { readTask, timeoutTask }, (completedTask) =>
					{
						if (completedTask == timeoutTask) //the timeout task was the first to complete
						{
							// Visual Studio keeps saying this exception is unhandled. Looks to me like it's handled in the EFTClientIPAsync ReadResponse method?
							throw new TaskCanceledException();
							//return -1
						}
						else //the readTask completed
						{
							return readTask.Result;
						}
					});
				}
			}
			catch
			{
				Close();
				throw;
			}
		}

		/// <summary>
		/// Polls the socket to determine the current connect state
		/// </summary>
		/// <returns>True if connected, false otherwise</returns>
		public async Task<bool> CheckConnectStateAsync()
		{
			// TcpClient.Connected returns the state of the last send/recv operation. It doesn't accurately 
			// reflect the current socket state. To get the current state we need to send a packet
			try
			{
				// Check if the state is disconnected based on the last operation
				if (_client?.Connected != true || _client?.GetStream() == null)
					return false;

				// Otherwise the socket was connected the last time we used it, send 0 byte packet to see if it still is...
				if (_client?.GetStream() != null)
				{
					await _client?.GetStream().WriteAsync(new byte[1], 0, 0);
				}
			}
			catch (System.IO.IOException e)
			{
				// 10035 == WSAEWOULDBLOCK
				if (e.InnerException != null && e.InnerException is SocketException && (e.InnerException as SocketException).NativeErrorCode == 10035)
					return true;

				return false;
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> WriteRequestAsync(string request)
		{
			// Build request
			var requestBytes = DirectEncoding.DIRECT.GetBytes(request);

			// Send the request string to the IP client.
			try
			{
				await _client.GetStream().WriteAsync(requestBytes, 0, requestBytes.Length);
			}
			catch
			{
				Close();
				throw;
			}
			return true;
		}

		public void Close()
		{
            if (_client?.Client != null)
            {
                _client?.Close();
            }
		}

		/// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
		public LogLevel LogLevel { get; set; }

		/// <summary> The log event to be called </summary>
		public event EventHandler<LogEventArgs> OnLog;

		void Log(LogLevel level, Action<TraceRecord> traceAction)
		{
			// Check if this log level is enabled and client is subscribed to the OnLog event
			if (OnLog == null || this.LogLevel >= level)
			{
				return;
			}

			TraceRecord tr = new TraceRecord() { Level = level };
			traceAction(tr);
			OnLog?.Invoke(this, new LogEventArgs() { LogLevel = level, Message = tr.Message, Exception = tr.Exception });
		}

		/// <summary>
		/// Returns the connected state as of the last read or write operation. This does not necessarily represent 
		/// the current state of the connection. 
		/// To check the current socket state call <see cref="CheckConnectStateAsync()"/>
		/// </summary>
		public bool IsConnected => _client?.Client != null && (_client?.Connected ?? false);

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_client?.Close();
					_client = null;
				}
				disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}

}
