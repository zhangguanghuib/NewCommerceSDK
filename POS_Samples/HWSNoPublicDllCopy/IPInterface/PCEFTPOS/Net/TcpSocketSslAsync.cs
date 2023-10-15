using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
	class TcpSocketSslAsync : ITcpSocketAsync
	{
		TcpClient _client = null;
		SslStream _clientStream = null;

		public List<string> CustomeRootCerts { get; set; } = null;

		public async Task<bool> ConnectAsync(string hostName, int hostPort, bool keepAlive = true)
		{
			try
			{
				// Connect client 
				_client = new TcpClient();
				_client.Client.SetKeepAlive(keepAlive, 60000UL /*60 secconds*/, 1000UL /*1 sec*/);
				await _client.ConnectAsync(hostName, hostPort);

				// If we are using SSL, create the SSL stream
				_clientStream = new SslStream(_client.GetStream(), true, RemoteCertificateValidationCallback);
				await _clientStream.AuthenticateAsClientAsync(hostName);

				if (_clientStream.IsAuthenticated && _clientStream.IsEncrypted && _clientStream.IsSigned)
				{
					return true;
				}
				else
				{
					throw new AuthenticationException("Server and client's security protocol doesn't match.");
				}
			}
			catch
			{
				Close();
				throw;
			}
		}

		void LogRemoteCertificateFailure(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Log(LogLevel.Error, tr =>
			{
				var msg = new System.Text.StringBuilder();
				msg.AppendLine($"SslPolicyErrors={sslPolicyErrors.ToString()}");
				msg.AppendLine($"{chain.ChainElements.Count} certificates in chain");
				int i = 0;
				foreach (var c in chain.ChainElements)
				{
					msg.AppendLine($"Idx:\"{i++}\", Subject:\"{c.Certificate.Subject}\", Issuer:\"{c.Certificate.Issuer}\", Serial:\"{c.Certificate.SerialNumber}\", Before:\"{c.Certificate.NotBefore}\", After:\"{c.Certificate.NotAfter}\", Thumbprint:\"{c.Certificate.Thumbprint}\"");
				}

				tr.Message = msg.ToString();
			});
		}

		/// <summary>
		/// Validate the PC-EFTPOS Cloud server certificate. 
		/// </summary>
		/// <returns>TRUE if the certificate is valid, FALSE otherwise</returns>
		bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// Certificate chain is valid via a commercial 3rd party chain 
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				Log(LogLevel.Info, tr => tr.Set("Remote certificate validated successfull by installed CA"));
				return true;
			}

			// Certificate has an invalid CN or isn't available from the server
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable || sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				LogRemoteCertificateFailure(certificate, chain, sslPolicyErrors);
				return false;
			}

			// The certificate is invalid due to an invalid chain. If we have included custom certificates we can attempt to validate here
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors && CustomeRootCerts?.Count > 0)
			{
				// Load custom certificates
				var x509Certificates = new List<X509Certificate2>();
				foreach (var certFilename in CustomeRootCerts)
				{
					try
					{
						x509Certificates.Add(new X509Certificate2(certFilename));
					}
					catch (System.Security.Cryptography.CryptographicException e)
					{
						Log(LogLevel.Error, tr => tr.Set($"Error loading certificate ({certFilename})", e));
						return false;
					}
				}

				var c = new X509Chain();
				try
				{
					c.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
					c.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;
					c.ChainPolicy.ExtraStore.AddRange(x509Certificates.ToArray());
					// Check if the chain is valid
					if (c.Build((X509Certificate2)certificate))
					{
						Log(LogLevel.Info, tr => tr.Set($"Remote certificate validated successfull by custom cert chain"));
						return true;
					}
					// The chain may not be valid, but if the only fault is an UntrustedRoot we can check if we have the custom root
					else
					{
						if (c?.ChainStatus?.Length > 0 && c?.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot)
						{
							var root = c.ChainElements[c.ChainElements.Count - 1];
							if (x509Certificates.Find(x509Certificate => x509Certificate.Thumbprint == root.Certificate.Thumbprint) != null)
							{
								Log(LogLevel.Info, tr => tr.Set($"Remote certificate validated successfull by custom cert chain and root"));
								return true;
							}
						}
					}
				}
				finally
				{
					c.Reset();
				}
			}

			LogRemoteCertificateFailure(certificate, chain, sslPolicyErrors);
			return false;
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
				if (_client?.Connected != true || _clientStream == null)
					return false;

				// Otherwise the socket was connected the last time we used it, send 0 byte packet to see if it still is...
				if (_clientStream != null)
				{
					await _clientStream.WriteAsync(new byte[1], 0, 0);
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


		public async Task<bool> WriteRequestAsync(string msgString)
		{
			// Build request
			var requestBytes = DirectEncoding.DIRECT.GetBytes(msgString);

			try
			{
				// Send the request string to the IP client.
				await _clientStream.WriteAsync(requestBytes, 0, requestBytes.Length);
			}
			catch
			{
				Close();
				throw;
			}
			return true;

		}

		public async Task<int> ReadResponseAsync(byte[] buffer, System.Threading.CancellationToken token)
		{
			try
			{
				// If we aren't using a CancellationToken we can just call ReadAsync directly
				if (token == System.Threading.CancellationToken.None)
				{
					return await _clientStream.ReadAsync(buffer, 0, buffer.Length, token);
				}
				// Else we need to jump through some hoops as ReadAsync doesn't return when token is cancelled
				else
				{
					var readTask = _clientStream.ReadAsync(buffer, 0, buffer.Length);
					var timeoutTask = Task.Delay(int.MaxValue, token); // Task.Delay handles CancellationToken correctly
					return await Task.Factory.ContinueWhenAny<int>(new Task[] { readTask, timeoutTask }, (completedTask) =>
					{
						if (completedTask == timeoutTask) //the timeout task was the first to complete
						{
							throw new TaskCanceledException();
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

		public void Close()
		{
			_clientStream?.Close();
            if (_client?.Client != null)
            {
                _client?.Close();
            }
		}
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

        /// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
        public LogLevel LogLevel { get; set; }

		/// <summary> The log event to be called </summary>
		public event EventHandler<LogEventArgs> OnLog;

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Close();
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
