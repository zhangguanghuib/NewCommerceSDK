using System;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <exclude/>
    class TcpSocketException : Exception
    {
        TcpSocketExceptionType _exceptionType;

        /// <exclude/>
        public TcpSocketException(TcpSocketExceptionType ExceptionType, string Message)
            : base(Message)
        {
            _exceptionType = ExceptionType;
        }

        /// <exclude/>
        public TcpSocketExceptionType ExceptionType
        {
            get { return _exceptionType; }
        }
    }

    /// <exclude/>
    enum TcpSocketExceptionType
    {
        /// <exclude/>
        ConnectException,
        /// <exclude/>
        GeneralException,
        /// <exclude/>
        SendException,
        /// <exclude/>
        ReceiveException
    }

    /// <exclude/>
    class TcpSocketEventArgs : EventArgs
    {
        TcpSocketExceptionType _ExceptionType;
        string _Error;
        byte[] _Bytes;
        string _Message;

        /// <exclude/>
        public string Error { get { return _Error; } set { _Error = value; } }
        /// <exclude/>
        public TcpSocketExceptionType ExceptionType { get { return _ExceptionType; } set { _ExceptionType = value; } }
        /// <exclude/>
        public byte[] Bytes { get { return _Bytes; } set { _Bytes = value; } }
        /// <exclude/>
        public string Message { get { return _Message; } set { _Message = value; } }
    }

    /// <exclude/>
    delegate void TcpSocketEventHandler(object sender, TcpSocketEventArgs e);

    /// <summary>
    /// Defines the socket interface used by EFTClientIP
    /// </summary>
    interface ITcpSocket: IDisposable
    {
        event TcpSocketEventHandler OnDataWaiting;
        event TcpSocketEventHandler OnTerminated;
        event TcpSocketEventHandler OnError;
        event TcpSocketEventHandler OnSend;

        bool Start();
        void Stop();
        bool Send(string message);
        string HostName { get; set; }
        int HostPort { get; set; }
        bool UseSSL { get; set; }
        bool UseKeepAlive { get; set; }

        /// <summary>
        /// Polls the socket to determine the current connect state
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        bool CheckConnectState();

        /// <summary>
        /// Returns the connected state as of the last read or write operation. This does not necessarily represent 
        /// the current state of the connection. 
        /// To check the current socket state call <see cref="CheckConnectState()"/>
        /// </summary>
        bool IsConnected { get; }

    }
}
