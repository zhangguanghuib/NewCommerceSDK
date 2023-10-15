using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
    /// <summary>The style of PC-EFTPOS dialog.</summary>
	public enum DialogType
	{
		/// <summary>The standard PC-EFTPOS dialog.</summary>
		Standard = '0',
		/// <summary>The PC-EFTPOS dialog for touch screens. Has larger buttons.</summary>
		TouchScreen = '1',
		/// <summary>Do not show the PC-EFTPOS dialogs.</summary>
		Hidden = '2'
	}

	/// <summary>The position of the PC-EFTPOS dialog.</summary>
	/// <remarks>Currently not supported.</remarks>
	public enum DialogPosition
	{
		/// <summary>The top left position of the screen.</summary>
		TopLeft,
		/// <summary>The top centre position of the screen.</summary>
		TopCentre,
		/// <summary>The top right position of the screen.</summary>
		TopRight,
		/// <summary>The middle left position of the screen.</summary>
		MiddleLeft,
		/// <summary>The centre position of the screen.</summary>
		Centre,
		/// <summary>The middle right position of the screen.</summary>
		MiddleRight,
		/// <summary>The bottom left position of the screen.</summary>
		BottomLeft,
		/// <summary>The bottom centre position of the screen.</summary>
		BottomCentre,
		/// <summary>The bottom right position of the screen.</summary>
		BottomRight
	}

	/// <summary>A PC-EFTPOS set dialog request object.</summary>
	public class SetDialogRequest : EFTRequest
    {
		/// <summary>Constructs a default set dialog request object.</summary>
		public SetDialogRequest() : base(false, typeof(SetDialogResponse))
    	{
        }

        /// <summary>Indicates the type of PC-EFTPOS dialog to use.</summary>
        /// <value>Type: <see cref="DialogType" /><para>The default is <see cref="DialogType.Standard" />.</para></value>
        public DialogType DialogType { get; set; } = DialogType.Standard;

        /// <summary>Indicates if the type of PC-EFTPOS dialog to use.</summary>
        /// <value>Type: <see cref="DialogType" /><para>The default is <see cref="DialogType.Standard" />.</para></value>
        [System.Obsolete("Please use DialogType instead of Type")]
        public DialogType Type { get { return DialogType; } set { DialogType = value; } }

        /// <summary>Indicates the X position of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int DialogX { get; set; } = 0;

        /// <summary>Indicates the Y position of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="System.Int32" /></value>
        public int DialogY { get; set; } = 0;

        /// <summary>Indicates the position of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="DialogPosition" /><para>The default is <see cref="DialogPosition.Centre" />.</para></value>
        public DialogPosition DialogPosition { get; set; } = DialogPosition.Centre;

        /// <summary>Indicates the position of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="DialogPosition" /><para>The default is <see cref="DialogPosition.Centre" />.</para></value>
        [System.Obsolete("Please use DialogPosition instead of Position")]
        public DialogPosition Position { get { return DialogPosition; } set { DialogPosition = value; } }
        
        /// <summary>Indicates if the PC-EFTPOS dialog is to be on top.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>The default is TRUE.</para></value>
        public bool EnableTopmost { get; set; } = true;

        /// <summary>Indicates if the PC-EFTPOS dialog is to be on top.</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>The default is TRUE.</para></value>
        [System.Obsolete("Please use EnableTopmost instead of TopMost")]
        public bool TopMost { get { return EnableTopmost; } set { EnableTopmost = value; } }


        /// <summary>Set the title of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        public string DialogTitle { get; set; } = "";

        /// <summary>Set the title of the PC-EFTPOS dialog.</summary>
        /// <value>Type: <see cref="System.String" /></value>
        [System.Obsolete("Please use DialogTitle instead of Title")]
        public string Title { get { return DialogTitle; } set { Title = DialogTitle; } }

        /// <summary>Disable all future display events to the POS</summary>
        /// <value>Type: <see cref="System.Boolean" /><para>The default is FALSE.</para></value>
        public bool DisableDisplayEvents { get; set; } = false;
    }

	/// <summary>A PC-EFTPOS set dialog response object.</summary>
	public class SetDialogResponse : EFTResponse
	{
		/// <summary>Constructs a default set dialog response object.</summary>
		public SetDialogResponse() : base(typeof(SetDialogRequest))
		{
		}

        /// <summary>Indicates if the set dialog request was successful.</summary>
        /// <value>Type: <see cref="System.Boolean" /></value>
        public bool Success { get;set; }
	}
}