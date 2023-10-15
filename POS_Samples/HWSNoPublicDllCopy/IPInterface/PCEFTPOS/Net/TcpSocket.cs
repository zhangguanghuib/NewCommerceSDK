using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <exclude/>
    class TcpSocket : ITcpSocket
    {
        Socket rawIpSocket;
        SslStream sslStream;
        AutoResetEvent connectEvent;

        /// <exclude/>
        AutoResetEvent ConnectEvent { get { return connectEvent; } }

        class StateObject
        {
            Socket _RawIPSocket;
            SslStream _SslStream;
            TcpSocket _Client;
            bool _UseSSL;
            int _BufferSize;
            byte[] _Buffer;

            public StateObject(Socket socket, TcpSocket client, bool useSSL)
            {
                _RawIPSocket = socket;
                _Client = client;
                _UseSSL = useSSL;
            }

            public StateObject(int bufferSize, SslStream sslStream, Socket socket, TcpSocket client, bool useSSL)
            {
                _RawIPSocket = socket;
                _BufferSize = bufferSize;
                _Buffer = new byte[bufferSize];
                _SslStream = sslStream;
                _Client = client;
                _UseSSL = useSSL;
            }

            public Socket RawIpSocket { get { return _RawIPSocket; } }
            public SslStream SslStream { get { return _SslStream; } set { _SslStream = value; } }
            public bool UseSSL { get { return _UseSSL; } }
            public int BufferSize { get { return _BufferSize; } }
            public byte[] Buffer { get { return _Buffer; } }
            public TcpSocket Client { get { return _Client; } }
        }

        /// <exclude/>
        public event TcpSocketEventHandler OnDataWaiting;
        /// <exclude/>
        public event TcpSocketEventHandler OnTerminated;
        /// <exclude/>
        public event TcpSocketEventHandler OnError;
        /// <exclude/>
        public event TcpSocketEventHandler OnSend;

        /// <exclude/>
        public TcpSocket(string hostName, int hostPort)
        {
            this.HostName = hostName;
            this.HostPort = hostPort;
            connectEvent = new AutoResetEvent(false);
        }

        /// <exclude/>
        public bool Start()
        {
            try
            {
                return Connect();
            }
            catch (TcpSocketException Exception)
            {
                FireOnErrorEvent(Exception.ExceptionType, Exception.Message);
                return false;
            }
        }

        /// <exclude/>
        public void Stop()
        {
            if (rawIpSocket != null)
            {
                lock (rawIpSocket)
                {
                    if (rawIpSocket != null)
                    {
                        if (rawIpSocket.Connected)
                        {
                            try
                            {
                                rawIpSocket.Shutdown(SocketShutdown.Both);
                                rawIpSocket.Close();
                                rawIpSocket = null;
                            }
                            catch (Exception ex)
                            {
                                FireOnErrorEvent(TcpSocketExceptionType.GeneralException, "Could not disconnect socket: " + ex.Message);
                            }
                        }
                    }
                }
            }
        }

        /// <exclude/>
        public bool Send(string message)
        {
            FireOnSend(message);
            return Send(DirectEncoding.DIRECT.GetBytes(message));
        }

        /// <summary>
        /// Polls the socket to determine the current connect state
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        public bool CheckConnectState()
        {
            // TcpClient.Connected returns the state of the last send/recv operation. It doesn't accurately 
            // reflect the current socket state. To get the current state we need to send a packet
            try
            {
                // Check if the state is disconnected based on the last operation
                if (rawIpSocket?.Connected != true)
                    return false;

                // Otherwise the socket was connected the last time we used it, send 0 byte packet to see if it still is...
                rawIpSocket.Send(new byte[1]);
            }
            catch (SocketException se)
            {
                // 10035 == WSAEWOULDBLOCK
                if (!se.NativeErrorCode.Equals(10035))
                    return false;
            }

            return true;
        }

        bool Send(byte[] bytes)
        {
            if (rawIpSocket == null || !rawIpSocket.Connected)
            {
                FireOnErrorEvent(TcpSocketExceptionType.SendException, "IP socket is not connected.");
                return false;
            }

            try
            {
                if (UseSSL)
                    sslStream.Write(bytes, 0, bytes.Length);
                else
                    rawIpSocket.Send(bytes);
            }
            catch (Exception ex)
            {
                FireOnErrorEvent(TcpSocketExceptionType.SendException, String.Format("Error sending. {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
                return false;
            }
            return true;
        }

        public async Task<bool> SendAsync(string message)
        {
            FireOnSend(message);
            return await SendAsync(DirectEncoding.DIRECT.GetBytes(message));
        }

        async Task<bool> SendAsync(byte[] bytes)
        {
            if (rawIpSocket == null || !rawIpSocket.Connected)
            {
                FireOnErrorEvent(TcpSocketExceptionType.SendException, "IP socket is not connected.");
                return false;
            }

            try
            {
                if (UseSSL)
                    await sslStream.WriteAsync(bytes, 0, bytes.Length);
                else
                    rawIpSocket.Send(bytes);
            }
            catch (Exception ex)
            {
                FireOnErrorEvent(TcpSocketExceptionType.SendException, String.Format("Error sending. {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace));
                return false;
            }
            return true;
        }

        bool Connect()
        {
            // Resolve listening end point.
            IPEndPoint remoteEP;
            try
            {
                IPAddress address = null;
                if (!IPAddress.TryParse(HostName, out address))
                {
                    foreach (IPAddress addr in Dns.GetHostEntry(HostName).AddressList)
                    {
                        if (addr.AddressFamily == AddressFamily.InterNetwork)
                            address = addr;
                    }
                }
                remoteEP = new IPEndPoint(address, HostPort);
            }
            catch
            {
                throw (new TcpSocketException(TcpSocketExceptionType.ConnectException, "DNS resolution failed: " + HostName));
            }

            // Being connect sequence.
            rawIpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (UseKeepAlive)
            {
                try
                {
                    rawIpSocket.SetKeepAlive(true, 60000UL /*60 seconds*/, 1000UL /*1 second*/);
                }
                catch (Exception e)
                {
                    FireOnErrorEvent(TcpSocketExceptionType.SendException, String.Format("Error setting socket options. {0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace));
                }
            }

            StateObject state = new StateObject(rawIpSocket, this, UseSSL);

            try
            {
                rawIpSocket.BeginConnect(remoteEP.Address, remoteEP.Port, new AsyncCallback(ConnectCallback), state);
            }
            catch (Exception ex)
            {
                throw (new TcpSocketException(TcpSocketExceptionType.ConnectException, "BeginConnect error: Code = " + ex.Message));
            }

            if (!connectEvent.WaitOne(5000, false))
            {
                Thread.Sleep(100);
                throw (new TcpSocketException(TcpSocketExceptionType.ConnectException, "Socket connect failed - timeout."));
            }

            Thread.Sleep(100);

            if (UseSSL)
            {
                sslStream = state.SslStream;
                if (sslStream == null || !(sslStream.CanRead && sslStream.CanWrite))
                    return false;
                //throw ( new IPClientException( IPClientExceptionType.ConnectException, "SSL connect failed." ) );
            }
            else
            {
                if (rawIpSocket == null || !rawIpSocket.Connected)
                    return false;
                //throw ( new IPClientException( IPClientExceptionType.ConnectException, "Socket connect failed." ) );
            }

            return true;
        }

        static void ConnectCallback(IAsyncResult result)
        {
            StateObject state = (StateObject)result.AsyncState;
            try
            {
                state.RawIpSocket.EndConnect(result);
            }
            catch (Exception ex)
            {
                if (state.Client != null)
                    state.Client.FireOnErrorEvent(TcpSocketExceptionType.ConnectException, "EndConnect error: Code = " + ex.Message);
                state.Client.ConnectEvent.Set();
                return;
            }

            if (state.UseSSL)
            {
                try
                {
                    state.SslStream = state.Client.Authenticate(new NetworkStream(state.RawIpSocket));
                }
                catch (Exception ex)
                {
                    if (state.Client != null)
                    {
                        state.RawIpSocket.Close();
                        state.Client.FireOnErrorEvent(TcpSocketExceptionType.ConnectException, "SSL error: " + ex.Message);
                    }
                    state.Client.ConnectEvent.Set();
                    return;
                }
            }
            // State receive sequence.
            try
            {
                StateObject receiveState = new StateObject(4096, state.SslStream, state.RawIpSocket, state.Client, state.UseSSL);
                if (state.UseSSL)
                    state.SslStream.BeginRead(receiveState.Buffer, 0, receiveState.BufferSize, new AsyncCallback(ReadCallback), receiveState);
                else
                    state.RawIpSocket.BeginReceive(receiveState.Buffer, 0, receiveState.BufferSize, 0, new AsyncCallback(ReadCallback), receiveState);
            }
            catch (Exception ex)
            {
                state.Client.FireOnErrorEvent(TcpSocketExceptionType.ConnectException, "BeginReceive error: Code = " + ex.Message);
            }

            state.Client.ConnectEvent.Set();
        }

        static void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            if (state.UseSSL)
            {
                if (!state.SslStream.CanRead)
                    return;
            }
            else
            {
                if (!state.RawIpSocket.Connected)
                    return;
            }

            byte[] readBytes;

            int bytesRead = 0;
            try
            {
                if (state.UseSSL)
                    bytesRead = state.SslStream.EndRead(ar);
                else
                    bytesRead = state.RawIpSocket.EndReceive(ar);
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    if (((SocketException)ex).ErrorCode == 10054)
                        bytesRead = -1;
                    state.Client.FireOnTerminatedEvent(((SocketException)ex).ErrorCode.ToString());
                }
                else
                {
                    state.Client.FireOnErrorEvent(TcpSocketExceptionType.ReceiveException, "EndReceive error: Code = " + ex.Message);
                    state.Client.FireOnTerminatedEvent(ex.Message);
                }
            }

            if (bytesRead > 0)
            {
                readBytes = new byte[bytesRead];
                System.Array.Copy(state.Buffer, readBytes, bytesRead);

                state.Client.FireOnDataWaitingEvent(readBytes);

                try
                {
                    if (state.UseSSL)
                        state.SslStream.BeginRead(state.Buffer, 0, state.BufferSize, new AsyncCallback(ReadCallback), state);
                    else
                        state.RawIpSocket.BeginReceive(state.Buffer, 0, state.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                catch (Exception ex)
                {
                    state.Client.FireOnErrorEvent(TcpSocketExceptionType.ReceiveException, "BeginReceive error: Code = " + ex.Message);
                    state.Client.FireOnTerminatedEvent(ex.Message);
                }
            }
            else if (bytesRead == 0)
            {
                state.Client.FireOnTerminatedEvent("bytesRead = 0");
            }
        }

        /// <exclude/>
        public SslStream Authenticate(Stream stream)
        {
            SslStream sslStream = new SslStream(stream, false, ValidateServerCertificate);
            sslStream.AuthenticateAsClient(HostName);
            return sslStream;
        }

        /// <exclude/>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        void FireOnTerminatedEvent(string message)
        {
            if (rawIpSocket != null)
                rawIpSocket.Close();

            TcpSocketEventArgs evArgs = new TcpSocketEventArgs() { Error = message };
            if (OnTerminated != null)
            {
                OnTerminated(this, evArgs);
            }
            else
            {
                Exception ex = new TcpSocketException(TcpSocketExceptionType.GeneralException, "There is no event handler defined for OnTerminated.");
                throw (ex);
            }
        }

        void FireOnDataWaitingEvent(byte[] bytes)
        {
            TcpSocketEventArgs evArgs = new TcpSocketEventArgs();
            if (OnDataWaiting != null)
            {
                evArgs.Bytes = bytes;
                OnDataWaiting(this, evArgs);
            }
            else
            {
                Exception ex = new TcpSocketException(TcpSocketExceptionType.GeneralException, "There is no event handler defined for OnDataWaiting.");
                throw (ex);
            }
        }

        void FireOnErrorEvent(TcpSocketExceptionType exceptionType, string errorMessage)
        {
            TcpSocketEventArgs evArgs = new TcpSocketEventArgs();
            if (OnError != null)
            {
                evArgs.Error = errorMessage;
                OnError(this, evArgs);
            }
            else
            {
                Exception ex = new TcpSocketException(TcpSocketExceptionType.GeneralException, "There is no event handler defined for OnError.");
                throw (ex);
            }
        }

        void FireOnSend(string message)
        {
            TcpSocketEventArgs evArgs = new TcpSocketEventArgs();
            if (OnSend != null)
            {
                evArgs.Message = message;
                OnSend(this, evArgs);
            }
        }

        /// <exclude/>
        public string HostName { get; set; }

        /// <exclude/>
        public int HostPort { get; set; }

        /// <exclude/>
        public bool UseSSL { get; set; }

        /// <exclude/>
        public bool UseKeepAlive { get; set; }

        /// <exclude/>
        public bool IsConnected => rawIpSocket?.Connected ?? false;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop(); // shutdown socket & release
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