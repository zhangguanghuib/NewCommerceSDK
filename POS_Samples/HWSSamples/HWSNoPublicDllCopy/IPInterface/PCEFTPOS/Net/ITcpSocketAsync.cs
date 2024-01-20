using System;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public interface ITcpSocketAsync: IDisposable
    {
        /// <summary>
        /// Connect to the EFT-Client (async) via TCP/IP.
        /// </summary>
        /// <param name="hostName">Address of host connection.</param>
        /// <param name="hostPort">Port Id of server connection.</param>
        /// <param name="keepAlive">True if keepalive should be enabled on the socket, false otherwise</param>
        /// <returns>Success or Fail</returns>
        Task<bool> ConnectAsync(string hostName, int hostPort, bool keepAlive);

        /// <summary>
        /// Sends a request to the EFT-Client.
        /// </summary>
        /// <param name="request">The message to send in string format.</param>
        /// <returns>Success or Fail</returns>
        Task<bool> WriteRequestAsync(string request);

        /// <summary>
        /// Reads a response from the EFT-Client.
        /// </summary>
        /// <param name="buffer">Used to store the read data.</param>
        /// <param name="token">THe <see cref="System.Threading.CancellationToken"/> object.</param>
        /// <returns></returns>
        Task<int> ReadResponseAsync(byte[] buffer, System.Threading.CancellationToken token);

        /// <summary>
        /// Closes the stream.
        /// </summary>
        void Close();

        /// <summary>
        /// Polls the socket to determine the current connect state
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        Task<bool> CheckConnectStateAsync();

        /// <summary>
        /// Returns the connected state as of the last read or write operation. This does not necessarily represent 
        /// the current state of the connection. 
        /// To check the current socket state call <see cref="CheckConnectStateAsync()"/>
        /// </summary>
        bool IsConnected { get; }

        /// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
        LogLevel LogLevel { get; set; }

        /// <summary> The log event to be called </summary>
        event EventHandler<LogEventArgs> OnLog;
    }

}
