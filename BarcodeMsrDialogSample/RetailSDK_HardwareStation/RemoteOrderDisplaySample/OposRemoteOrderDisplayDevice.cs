/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.HardwareStation.RemoteOrderDisplaySample
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics.CodeAnalysis;
        using System.Threading;
        using Interop.OPOSRemoteOrderDisplay;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The remote order display device of OPOS type.
        /// </summary>
        public class OposRemoteOrderDisplayDevice : INamedRequestHandler, IDisposable
        {
            private const string RemoteOrderDisplayInstanceNameFormat = "OposCoinDispenser_{0}";

            private IOPOSRemoteOrderDisplay oposRemoteOrderDisplay;
            private string remoteOrderDisplayInstanceName;
            private readonly object asyncLock = new object();

            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName
            {
                get { return PeripheralType.Opos; }
            }

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(OpenRemoteOrderDisplayDeviceRequest),
                        typeof(DisplayMessageRemoteOrderDisplayDeviceRequest),
                        typeof(CloseRemoteOrderDisplayDeviceRequest)
                    };
                }
            }

            /// <summary>
            /// Represents the entry point for the remote order display device request handler.
            /// </summary>
            /// <param name="request">The incoming request message.</param>
            /// <returns>The outgoing response message.</returns>
            public Response Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if (requestType == typeof(OpenRemoteOrderDisplayDeviceRequest))
                {
                    var openRequest = (OpenRemoteOrderDisplayDeviceRequest)request;

                    this.Open(openRequest.DeviceName);
                }
                else if (requestType == typeof(DisplayMessageRemoteOrderDisplayDeviceRequest))
                {
                    var displayMessageRequest = (DisplayMessageRemoteOrderDisplayDeviceRequest)request;

                    this.DisplayMessage(displayMessageRequest.Data);
                }
                else if (requestType == typeof(CloseRemoteOrderDisplayDeviceRequest))
                {
                    this.Close();
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", requestType));
                }

                return new NullResponse();
            }

            /// <summary>
            /// Disposes the remote order display and mutex object if the remote order display is not closed correctly.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);

                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Disposes the remote order display objects if <paramref name="disposing"/> is set to <c>true</c>>.
            /// </summary>
            /// <param name="disposing">Disposing flag set to true.</param>
            [SuppressMessage(
                "Microsoft.Usage",
                "CA2213:DisposableFieldsShouldBeDisposed",
                MessageId = "oposRemoteOrderDisplayMutex",
                Justification = "The call this.Close() actually disposes the oposRemoteOrderDisplayMutex object in finally block.")]
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            /// <summary>
            /// Opens a peripheral.
            /// </summary>
            /// <param name="peripheralName">Name of the peripheral.</param>
            private void Open(string peripheralName)
            {
                this.remoteOrderDisplayInstanceName = string.Format(RemoteOrderDisplayInstanceNameFormat, peripheralName);

                // OPOS Service objects are not thread safe.
                // We will acquire the lock on currently opened coin dispenser to avoid multithread operations on same SO.
                DeviceLockContainer.ExecuteOpos(this.asyncLock, this.remoteOrderDisplayInstanceName, OposHelper.CurrentThreadId, () =>
                {
                    this.oposRemoteOrderDisplay = OPOSDeviceManager<IOPOSRemoteOrderDisplay>.Instance.AcquireDeviceHandle<OPOSRemoteOrderDisplayClass>();

                    // Open
                    this.oposRemoteOrderDisplay.Open(peripheralName);
                    OposHelper.CheckResultCode(this, this.oposRemoteOrderDisplay.ResultCode);

                    // Claim
                    this.oposRemoteOrderDisplay.ClaimDevice(OposHelper.ClaimTimeOut);
                    OposHelper.CheckResultCode(this, this.oposRemoteOrderDisplay.ResultCode);

                    // Enable
                    this.oposRemoteOrderDisplay.DeviceEnabled = true;
                    OposHelper.CheckResultCode(this, this.oposRemoteOrderDisplay.ResultCode);
                });
            }

            /// <summary>
            /// Close the remote order display device.
            /// </summary>
            private void Close()
            {
                try
                {
                    DeviceLockContainer.ExecuteOpos(this.asyncLock, this.remoteOrderDisplayInstanceName, OposHelper.CurrentThreadId, () =>
                    {
                        this.oposRemoteOrderDisplay = OPOSDeviceManager<IOPOSRemoteOrderDisplay>.Instance.AcquireDeviceHandle<OPOSRemoteOrderDisplayClass>();

                        // Close the remote order display. 
                        if (this.oposRemoteOrderDisplay != null)
                        {
                            // Disabled
                            this.oposRemoteOrderDisplay.DeviceEnabled = false;

                            // Release
                            this.oposRemoteOrderDisplay.ReleaseDevice();

                            // Close
                            this.oposRemoteOrderDisplay.Close();

                            this.oposRemoteOrderDisplay = null;
                        }
                    });
                }
                finally
                {
                    // We have contract with Remote order display controller to call close for sure when done interacting with remote order display.
                    // This way we ensure to release the lock.
                    if (this.oposRemoteOrderDisplay != null)
                    {
                        OPOSDeviceManager<IOPOSRemoteOrderDisplay>.Instance.ReleaseDeviceHandle(this.oposRemoteOrderDisplay);
                    }
                }
            }

            /// <summary>
            /// Display an array of messages on the Remote Order Display device.
            /// </summary>
            /// <param name="data">Array of messages to display.</param>
            private void DisplayMessage(string[] data)
            {
                ThrowIf.Null(data, "data");

                DeviceLockContainer.ExecuteOpos(this.asyncLock, this.remoteOrderDisplayInstanceName, OposHelper.CurrentThreadId, () =>
                {
                    this.oposRemoteOrderDisplay = OPOSDeviceManager<IOPOSRemoteOrderDisplay>.Instance.AcquireDeviceHandle<OPOSRemoteOrderDisplayClass>();

                    // Send the cash change into the coin dispenser.
                    if (this.oposRemoteOrderDisplay != null && this.oposRemoteOrderDisplay.DeviceEnabled)
                    {
                        const int StartColumn = 1;

                        // Clear the device before showing the messages.
                        this.oposRemoteOrderDisplay.ClearVideo(OposHelper.VideoUnitsForClearVideoOperation, OposHelper.DeviceVideoAttribute);

                        int startRow = 1;
                        int maxCharactersInARow = OposHelper.ColumnsInBox - 2;

                        // For each message in the data string array, draw a box and display the message.
                        foreach (string message in data)
                        {
                            // Compute the number of rows required to showcase the message.
                            int rowsRequiredToShowcaseMessage = 1 + (message.Length / maxCharactersInARow);

                            // Draw a box.
                            this.oposRemoteOrderDisplay.DrawBox(OposHelper.VideoUnitsForDisplayDataAndDrawBoxOperations, startRow, StartColumn, rowsRequiredToShowcaseMessage, OposHelper.ColumnsInBox, OposHelper.DeviceVideoAttribute, (int)OposHelper.BorderType.SolidBorder);

                            // Display the message data inside the box.
                            this.oposRemoteOrderDisplay.DisplayData(OposHelper.VideoUnitsForDisplayDataAndDrawBoxOperations, startRow + 1, StartColumn + 1, OposHelper.DeviceVideoAttribute, message);

                            // Update the starting row for displaying next message.
                            startRow += rowsRequiredToShowcaseMessage + OposHelper.InterMessageDistance + 2;
                        }
                    }
                    else
                    {
                        throw new PeripheralException("Must open RemoteOrderDisplay before display message.");
                    }
                });
            }
        }
    }
}