using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public interface IEFTClientIPAsync : IDisposable
    {
        /// <summary>
        /// Connect to the EFT-Client (async).
        /// </summary>
        /// <param name="hostName">Address of host connection.</param>
        /// <param name="hostPort">Port Id of server connection.</param>
        /// <param name="useSSL">Enables SSL connection.</param>
        /// <param name="useKeepAlive">True to enable TCP keep alives</param>
        /// <returns></returns>
        Task<bool> ConnectAsync(string hostName, int hostPort, bool useSSL = true, bool useKeepAlive = true);

        /// <summary>
        /// Disconnect from the EFT-Client (async).
        /// </summary>
        [System.Obsolete("Please use Disconnect() instead.")]
        bool DisconnectAsync();

        /// <summary>
        /// Disconnect from the EFT-Client
        /// </summary>
        bool Disconnect();

        /// <summary>Sends a request to the EFT-Client</summary>
        /// <param name="request">The <see cref="EFTRequest"/> to send</param>
        /// <param name="member">Used for internal logging. Ignore</param>
        /// <returns>FALSE if an error occurs</returns>
        /// <exception cref="ConnectionException">The socket is closed.</exception>
        Task<bool> WriteRequestAsync(EFTRequest request, [CallerMemberName] string member = "");

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
        Task<T> WriteRequestAndWaitAsync<T>(EFTRequest request, System.Threading.CancellationToken cancellationToken, [CallerMemberName] string member = "") where T : EFTResponse;

        /// <summary> 
        /// Retrieves the next EFT response from the client
        /// </summary>
        /// <returns> An EFTResponse if one could be read, otherwise null </returns>
        /// <exception cref="ConnectionException">The socket is closed.</exception>
        Task<EFTResponse> ReadResponseAsync();

        /// <summary> 
        /// Retrieves the next EFT response from the client
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> An EFTResponse if one could be read, otherwise null </returns>
        /// <exception cref="ConnectionException">The socket is closed.</exception>
        /// <exception cref="TaskCanceledException">The task was cancelled by cancellationToken</exception>
        Task<EFTResponse> ReadResponseAsync(System.Threading.CancellationToken cancellationToken);

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
        Task<T> ReadResponseAsync<T>(System.Threading.CancellationToken cancellationToken) where T : EFTResponse;

        /// <summary>
        /// Closes the connection.
        /// </summary>
        [Obsolete("Please use Disconnect() instead.")]
        void Close();

        /// <summary>
        /// Polls the socket to determine the current connect state
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        Task<bool> CheckConnectStateAsync();

        /// <summary> Event called when log information is avaialble </summary>
        event EventHandler<PCEFTPOS.EFTClient.IPInterface.LogEventArgs> OnLog;

        /// <summary>The IP host name of the PC-EFTPOS IP Client.</summary>
        /// <value>Type: <see cref="System.String" /><para>The IP address or host name of the EFT Client IP interface.</para></value>
        string HostName { get; }

        /// <summary>The IP port of the PC-EFTPOS IP Client.</summary>
        /// <value>Type: <see cref="System.Int32" /><para>The listening port of the EFT Client IP interface.</para></value>
        int HostPort { get; }

        /// <summary>Indicates whether to use SSL encryption.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        bool UseSSL { get; }

        /// <summary>Indicates whether to allow TCP keep-alives.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>Defaults to FALSE.</para></value>
        bool UseKeepAlive { get;  }

        /// <summary>Indicates whether there is a request currently in progress.</summary>
        bool IsRequestInProgress { get; }

        /// <summary>
        /// Returns the connected state as of the last read or write operation. This does not necessarily represent 
        /// the current state of the connection. 
        /// To check the current socket state call <see cref="CheckConnectStateAsync()"/>
        /// </summary>
        bool IsConnected { get; }

        /// <summary> When TRUE, the SynchronizationContext will be captured from requests and used to call events</summary>
        bool UseSynchronizationContextForEvents { get; set; }

        /// <summary> Defines the level of logging that should be passed back in the OnLog event. Default <see cref="LogLevel.Off" />. <para>See <see cref="LogLevel"/></para></summary>
        LogLevel LogLevel { get; set; }

        /// <summary>Implementation of a dialog manager</summary>
        IDialogUIHandlerAsync DialogUIHandlerAsync { get; set; }
    }
}
