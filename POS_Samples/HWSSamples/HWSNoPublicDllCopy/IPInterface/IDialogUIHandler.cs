using System;
using System.Collections.Generic;
using System.Text;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>
    /// An interface for a UI that implements the PC-EFTPOS dialog messages
    /// Uses IEFTClientIP to send key press messages to the EFT-Client
    /// </summary>
    public interface IDialogUIHandler
    {
        /// <summary>
        /// An instance to the IEFTClientIP. Used to send key press requests
        /// </summary>
        IEFTClientIP EFTClientIP { get; set; }

        /// <summary>
        /// Called by the EFTClientIP when a EFTDisplayResponse is received
        /// </summary>
        void HandleDisplayResponse(EFTDisplayResponse eftDisplayResponse);

        /// <summary>
        /// Called by the EFTClientIP when the dialog needs to be closed
        /// </summary>
        void HandleCloseDisplay();
    }
}
