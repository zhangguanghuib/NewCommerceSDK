using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>
    /// An interface for a UI that implements the PC-EFTPOS dialog messages.
    /// Uses IEFTClientIPAsync to send key press messages to the EFT-Client
    /// </summary>
    public interface IDialogUIHandlerAsync
    {
        /// <summary>
        /// An instance to the IEFTClientIP. Used to send key press requests
        /// </summary>
        IEFTClientIPAsync EFTClientIPAsync { get; set; }

        /// <summary>
        /// Called by the EFTClientIPAsync when a EFTDisplayResponse is received
        /// </summary>
        Task HandleDisplayResponseAsync(EFTDisplayResponse eftDisplayResponse);

        /// <summary>
        /// Called by the EFTClientIPAsync when the dialog needs to be closed
        /// </summary>
        Task HandleCloseDisplayAsync();
    }
}
