using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public enum ActionType { Unknown = -1, DisplayOptions, DisplayAndWait, DisplayWithButtons, GetNumericInput, GetAmountInput };
    public enum EntryType { Unknown = -1, Text, Numeric, Amount };
    public enum EntryPointType { Undefined, Default, Button = Default, PaymentMenu, AdminMenu, DiagnosticsMenu, SettingsMenu, Settlement, Logon, AppMenu }

    /// <summary>
    /// A PC-EFTPOS POS as pinpad request object.
    /// </summary>
    [XmlRoot("PCEFTPOS_BRIDGE")]
    public class EFTPosAsPinpadRequest : EFTRequest
    {
        /// <summary>Constructs a default POS as pinpad request object.</summary>
        public EFTPosAsPinpadRequest() : base(false, typeof(EFTPosAsPinpadResponse))
        {
        }

        [XmlElement("PCEFTPOS_SOURCE")]
        public string Source { get; set; } = "XML-POS"; // string.Empty;

        [XmlElement("PCEFTPOS_DESTINATION")]
        public string Destination { get; set; } = "PCEFTPOS_DLL_HANDLER"; //string.Empty;

        [XmlElement("PCEFTPOS_METHOD", IsNullable = false)]
        public PCEFTPOSMethod MethodName { get; set; }

        [XmlElement("PCEFTPOS_EVENT", IsNullable = false)]
        public PCEFTPOSEvent EventName { get; set; }
    }

    /// <summary>
    /// A PC-EFTPOS POS as pinpad response object.
    /// </summary>
    [XmlRoot("PCEFTPOS_BRIDGE")]
    public class EFTPosAsPinpadResponse : EFTResponse
    {
        /// <summary>Constructs a default POS as pinpad response object.</summary>
        public EFTPosAsPinpadResponse() : base(typeof(EFTPosAsPinpadRequest))
        {
        }

        [XmlElement("PCEFTPOS_SOURCE")]
        public string Source { get; set; } = "XML-POS"; // string.Empty;

        [XmlElement("PCEFTPOS_DESTINATION")]
        public string Destination { get; set; } = "PCEFTPOS_DLL_HANDLER"; //string.Empty;

        [XmlElement("PCEFTPOS_EVENT")]
        public PCEFTPOSEvent EventName { get; set; }

        [XmlElement("PCEFTPOS_METHOD", IsNullable = false)]
        public PCEFTPOSMethod MethodName { get; set; }
    }

    public enum MethodType { GetEntryPoints, DisplayOnPinpad, SetKeyMaskOnPinpad, EndSession };

    public class PCEFTPOSMethod
    {
        public PCEFTPOSMethod()
        {
        }

        public PCEFTPOSMethod(MethodType type)
        {
            switch (type)
            {
                case MethodType.GetEntryPoints: EntryPoints = new List<EntryPoint>(); break;
                case MethodType.DisplayOnPinpad: DisplayOnPos = new DisplayOnPos(); break;
                case MethodType.EndSession: EndSession = new EndSession(); break;
                default: break;
            }
        }

        [XmlElement(IsNullable = false)]
        public DisplayOnPos DisplayOnPos { get; set; }

        [XmlElement("InitialiseEvent", IsNullable = false)]
        public IntialiseEvent Initialised { get; set; } = null;

        [XmlArray("ApplicationStatusList", IsNullable = false)]
        public List<DllApplicationStatusUpdate> ApplicationStatusList { get; set; } = null;

        [XmlArray("EntryPointList", IsNullable = false)]
        public List<EntryPoint> EntryPoints { get; set; }

        [XmlElement(IsNullable = false)]
        public EndSession EndSession { get; set; }

        [XmlElement(IsNullable = false)]
        public DisplayOnPinpad DisplayOnPinpad { get; set; }

        [XmlElement("SetKeyMaskOnPinpad", IsNullable = false)]
        public KeyMask KeyMaskOnPinpad { get; set; }

        [XmlElement("GetInputDataFromPinpad", IsNullable = false)]
        public Input InputDataFromPinpad { get; set; }

        [XmlElement("GetOptionFromPinpadList", IsNullable = false)]
        public DropdownOptions OptionFromPinpad { get; set; }
    }

    public enum EventType { Initialise, InitialiseEvent, GetApplicationStatus, GetEntryPoints, GetResourceList, InitiateEntryPoint, DisplayOnPos, InputData };

    public class PCEFTPOSEvent
    {
        public PCEFTPOSEvent()
        {
        }

        public PCEFTPOSEvent(EventType type)
        {
            switch (type)
            {
                case EventType.Initialise: Initialise = new PinpadRegistration(); break;
                case EventType.GetApplicationStatus: GetApplicationStatus = new object(); break;
                case EventType.GetEntryPoints: EntryPoints = new List<EntryPoint>(); break;
                case EventType.GetResourceList: Resources = new object(); break;
                case EventType.InitiateEntryPoint: EntryPointEvent = new EntryPoint(); break;
                case EventType.DisplayOnPos: DisplayOnPos = new DisplayOnPos(); break;
                default: break;
            }
        }

        [XmlElement(IsNullable = false)]
        public DisplayOnPos DisplayOnPos { get; set; }

        [XmlElement(IsNullable = false)]
        public PinpadRegistration Initialise { get; set; }

        [XmlElement("InitialiseEvent", IsNullable = false)]
        public object Initialised { get; set; } = null;

        [XmlElement("GetApplicationStatusList", IsNullable = false)]
        public object GetApplicationStatus { get; set; } = null;

        [XmlElement("ApplicationStatusList", IsNullable = false)]
        public object ApplicationStatus { get; set; } = null;

        [XmlArray("GetEntryPointList", IsNullable = false)]
        public List<EntryPoint> EntryPoints { get; set; } = null;

        [XmlElement("ResourceList", IsNullable = false)]
        public object Resources { get; set; } = null;

        [XmlElement("EntryPointEvent", IsNullable = false)]
        public EntryPoint EntryPointEvent { get; set; } = null;

        [XmlElement("PinpadKeyPressEvent", IsNullable = false)]
        public KeyPress PinpadKeyPressEvent { get; set; } = null;

        [XmlElement("GetInputDataFromPinpadEvent", IsNullable = false)]
        public AcquiredInput InputData { get; set; } = null;

        [XmlElement("GetOptionFromPinpadListEvent", IsNullable = false)]
        public AcquiredOption PinpadOption { get; set; } = null;
    }

    public class IntialiseEvent
    {
        public string ResponseCode { get; set; } = string.Empty;
    }

    public class PinpadRegistration
    {
        public int NumberOfRows { get; set; } = 30;
        public int NumberOfColumns { get; set; } = 40;
    }

    public class DllApplicationStatusUpdate
    {
        [XmlElement("AppName")]
        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        [XmlElement("AppType")]
        public string Type { get; set; } = string.Empty;
        public string DllName { get; set; } = string.Empty;
    }

    public class EntryPoint
    {
        /// <summary>
        /// Name to return 
        /// This name is put in the METHOD and the function that was associated with this call is called
        /// when the METHOD is sent into the DLL.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Text to display on the button. If "", Name is used.
        /// </summary>
        public string ButtonLabel { get; set; } = null;

        /// <summary>
        /// // Type of node this is. This determines if its in a default menu or not. // default is ""
        /// "PaymentMenu" - option put in the Payment Menu of Master GUI
        /// "SettingsMenu" - option put in the Settings Menu of Master GUI
        /// "AdminMenu" - option put in the Admin Menu of Master GUI
        /// "DiagnosticsMenu" - option put in the Admin Menu of Master GUI
        /// "Logon"     - logon function that is called as a part of a Groupled Logon.
        /// "Settlement" - individual function that is called if doing a group settlement
        ///              - The master GUI will call all Dll's that expose this option in order. The DLL
        ///                 can do a settlement, print and then return.
        /// "Button" - inidividual button that performs something. Same as ""
        /// "PreSale" - PreSale Request. May be called after amount collected. The Master GUI will let the
        ///             user decide to choose which Pre-Sale to call.
        /// "PostSale" - PostSale Request. May be called after a sale ends if the user selects this app
        ///             from its list of PostSale apps.
		/// "CardReadAtIdle" - If a card is read outside a sale at the idle prompt, this will be called.
        ///                     - The Master GUI will use the "CardRangeStart", "CardRangeEnd" and "CardType" properties match a card.
        ///                     - The DLL will receive an event with["CardNumber"] if this fires.
        /// "PreSaleMandatory" - This key will be called before the payment is processed (after amount entry)
        ///                     - The input node will be the same as in DoPreSaleCheck()
        ///                     - This event is called after the Pre-Sale menu and this entry is not included in the user list to choose.
        ///                     - designed for tru-rating and tipping.
        /// "PostSaleMandatory" - Called after the "TransactionEvent" is issued and bfore its sent to the POS.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        EntryPointType entryPointType = EntryPointType.Undefined;
        public EntryPointType EntryPointType
        {
            get
            {
                if (entryPointType == EntryPointType.Undefined)
                {
                    var type = EntryPointType.Undefined;
                    if (Enum.TryParse<EntryPointType>(Type, out type))
                    {
                        entryPointType = type;
                    }
                    else
                    {
                        entryPointType = EntryPointType.Default;
                    }
                }

                return entryPointType;
            }
        }

        public string OwnerType { get; set; } = string.Empty;

        /// <summary>
        /// Top level button. Else its the "Name" of the EntryPoint that this is below.
        /// </summary>
        public string Owner { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;

        public string DllName { get; set; } = string.Empty;
        public string MenuHeader { get; set; } = string.Empty;
        [XmlElement("AppName")]
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Category for what this entry point relates to. This allows other apps to know what sort of thing this entry point does.
        /// - "Discount" - applies a discount to a sale
        /// - "Loyalty" - 
        /// - "Payment" - provides a way to do a payment
        /// - "SaleModifier" - modifies a sale(tipping etc)
        /// - "DataCollector" - collects data from the customer
        /// </summary>
        [XmlElement("AppType")]
        public string ApplicationType { get; set; } = string.Empty;

        /// <summary>
        /// The color of the button to display.
        /// </summary>
        //public Color ButtonColour { get; set; } = Color.DarkGray;

        ///<summary>
        ///URL to go to if this is pressed. Never returns to the DLL.
        ///Must use http/https.
        ///</summary>
        public string Hyperlink { get; set; } = string.Empty;

        /// <summary>
        /// Name of icon to display on this button.
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Detailed info about what this button does.
        /// </summary>
        public string Help { get; set; } = string.Empty;

        // TODO: implement other fields.

        public string CategoryMain { get; set; } = string.Empty;

        public string CategorySub { get; set; } = string.Empty;

        public int Priority { get; set; } = 0;

        public string PasswordProtect { get; set; } = string.Empty;

        public string FunctionNumber { get; set; } = string.Empty;

        public string MenuPosLine1 { get; set; } = string.Empty;

        public string MenuPosLine2 { get; set; } = string.Empty;

        public string ToolTip { get; set; } = string.Empty;

        [XmlElement("FROM_GUI")]
        public InputDescriptionNode InputNode { get; set; } = null;

        [XmlElement("TO_GUI")]
        public InputDescriptionNode OutputNode { get; set; } = null;

        /* ["CategoryMain"] = ""  // suggested top level category eg "Settings"
			   ["CategorySub"] = ""; // Sub category 
			   ["Priority"] = 0-99; // priority level of this setting. 99 is highest
			   ["PasswordProtect"] = ""; // password that is entered before you can call this
			   ["FunctionNumber"] = ""; # to use for simple pinpads to call this node.
         * ["MenuPosLine1"] = ""; // line1 of pos display if the menu is presented on POS dialog
			   ["MenuPosLine2"] = ""; // line2 of pos display if the menu is presented on POS dialog
		["ToolTip"] = ""; // tip to display if hovered over?
		["Help"] = "";  // detailed info about what this button does.
		["HyperLink"] = ""; // URL to go to if this is pressed. Never returns to the DLL.
        // Input Description Node.
			   // - Used to describe what this call will accept as inputs. The GUI can set these varaibale in the "FROM_GUI" node.
			   // - This gives the GUI an idea of what it could get and start the sale with 
			   //   so that the extension will skip its input.
			   // - Example: A GUI could collect the Purchase Amount and start the sale with it already set. The Extension
			   //   will see that its set and skip the entry routine.
			   "TO_GUI".Name = "Variables"
					- ["AmtPurchase"] = "";
					- ["AmtCash"] = "";
					- ["AmtRefund"] = "100"; // $1.00
					- ["AmtTip"] = "100"; // $1.00
					- ["Type"] = "";
					- ["TxnRef"] = "";       // unique ref number.
					- ["Track2"] = ""; // Card number of the sale if has been read.
                    */

        public EntryPoint()
        {
        }

    }

    public class InputDescriptionNode
    {
        [XmlElement("AmtPurchase", IsNullable = false)]
        public int AmountPurchase { get; set; } = 0;

        [XmlElement("AmtCash", IsNullable = false)]
        public int AmountCash { get; set; } = 0;

        [XmlElement("AmtRefund", IsNullable = false)]
        public int AmountRefund { get; set; } = 0;

        [XmlElement("AmtTip", IsNullable = false)]
        public int AmountTip { get; set; } = 0;

        [XmlElement(IsNullable = false)]
        public string Type { get; set; } = string.Empty;

        [XmlElement("TxnRef", IsNullable = false)]
        public string TransactionReference { get; set; } = string.Empty;

        [XmlElement(IsNullable = false)]
        public string Track2 { get; set; } = string.Empty;
    }

    /// <summary>
    /// Display details on the POS.
    /// </summary>
    public class DisplayOnPos // TODO: confirm whether this needs to be supported here.
    {
        [XmlElement("line1")]
        public string Line1 { get; set; }
        [XmlElement("line2")]
        public string Line2 { get; set; } // TODO: up to line99... cleaner way to do this?
        [XmlElement("line3")]
        public string Line3 { get; set; } // TODO: up to line99... cleaner way to do this?
        public int GraphicCode { get; set; } // From ActiveX documentation: 0: Processing, 1: Verify, 2: Question, 3: Card, 4: Account, 5: PIN, 6: Finished
        public int DialogId { get; set; }
        public string DialogTitle { get; set; }
        public int DialogType { get; set; }
        public int DialogX { get; set; }
        public int DialogY { get; set; }
        public bool DialogTopMost { get; set; }
        public int ClearKeyMask { get; set; } = 1; // default to clear the keys. Set to 0 if you want to keep whats there. 
        //public string InputTarget { get; set; } // Pinpad, "XML_POS"
    }

    public class DisplayOnPinpad
    {
        [XmlElement("line1")]
        public string Line1 { get; set; }
        [XmlElement("line2")]
        public string Line2 { get; set; } // TODO: up to lineN... cleaner way to do this? what's N?
        [XmlElement("line3")]
        public string Line3 { get; set; }

        public int NumberOfRows { get; set; } = 8;
        public int NumberOfColumns { get; set; } = 20;
        public bool ClearDisplay { get; set; } = true;

        public int ImageIndex { get; set; } = -1;
        public int ImageLine { get; set; } = -1;
        public int ImageColumn { get; set; } = -1;
        public string QrCodeData { get; set; }
        public int QrCodeVersion { get; set; } = 7; // we support 1-11 ( 21x21 to 61x61)
        public int QrErrorCorrection { get; set; } // 0= low, 1 = medium, 2= quartile, 3 =high, what is this?
        public int QrStartPositionX { get; set; } // x = 0-15 in bytes
        public int QrStartPositionY { get; set; } // y = 0-64 in pixels
        public string InputTarget { get; set; }
        public int GraphicCode { get; set; } = -1;
        public string Avi { get; set; }
    }

    public class KeyMask
    {
        [XmlElement("KeyMask")]
        public string Keymask { get; set; } = string.Empty;
        public int Timeout { get; set; } = -1;
    }

    public class KeyPress
    {
        [XmlElement("PinpadKeyPress")]
        public string KeyPressed { get; set; }
        public string Result { get; set; }
    }

    public class EndSession
    {
        public bool StayInSlaveMode { get; set; }
    }

    public enum InputMode { Numeric, Amount, AlphaNumeric, Password, Date, Time, Email, IPAddress, Slider }

    public class Input
    {
        public int Timeout { get; set; }
        public bool ResetTimeoutonKeyPress { get; set; }
        public bool CancelOnPOS { get; set; }
        public string InputMode { get; set; }
        public InputMode InputModeType { get { return (string.IsNullOrEmpty(InputMode) ? IPInterface.InputMode.AlphaNumeric : (InputMode)Enum.Parse(typeof(InputMode), InputMode)); } }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int InputLine { get; set; }
        public string DefaultValue { get; set; }
        public string InputBoxTitle { get; set; }

        [XmlElement("InputBoxCaption")]
        public string InputBoxPlaceholder { get; set; }
        public int GraphicCode { get; set; }
        public string Avi { get; set; }

    }

    public class AcquiredInput
    {
        public string InputData { get; set; }
        public string Result { get; set; }
        public bool PosCancelled { get; set; }
        public string ResultText { get; set; }
    }

    public class DropdownOptions
    {
        public string HeadingLine1 { get; set; } = null;
        public string HeadingLine2 { get; set; } = null;
        public string HeadingLine3 { get; set; } = null;
        public string HeadingLine4 { get; set; } = null;
        public int Timeout { get; set; } = 60;
        /// <summary>
        /// resets Timeout on every key action. If 0, you have x seconds in total. default is 1
        /// </summary>
        public bool ResetTimeoutonKeyPress { get; set; } = true;
        public bool CancelOnPOS { get; set; } = false;
        public bool CancelOnPinpad { get; set; } = true;

        /// <summary>
        /// [val=text] on each line. Else[text] on each line. 
        /// default is false.
        /// </summary>
        public bool LineTextOnly { get; set; } = false;
        /// <summary>
        /// if present, it will use the stored graphic for the display insead
        /// of text. But... no numeric Keys are allowed as options.
        /// Only ENTER, CLEAR, CANCEL, FUNC, F0-f9
        /// </summary>
        public int GraphicScreenId { get; set; } = -1;
        /// <summary>
        /// (default) = "menu", "vertical" = scroll menu. Some pinpads will only do menu.
        /// </summary>
        public string DrawingMode { get; set; } = "menu";
        /// <summary>
        /// value of key to use if the ENTER key is pressed and not defined.
        /// </summary>
        public string EnterKeyDefault { get; set; }
        /// <summary>
        /// if 1, CLEAR will be returned if pressed without displaying anything.
        /// no selectedtext will be returned though..
        /// </summary>
        public bool ReturnOnClearKey { get; set; } = false;
        /// <summary>
        /// Only on XML_POS. SelectedOption will be delimited.?????
        /// </summary>
        public bool MultiSelect { get; set; } = false;
        /// <summary>
        /// OptionList["0"] = "text for option 0"; "0" to "9"
        /// OptionList["ENTER"] = "text for ENTER key";  // "ENTER", "CLEAR", "CANCEL", "FUNC"
		///	OptionList["F0"] = "text for F0 key";   // "F0" to "F9"
        /// </summary>
        [XmlElement("OptionList")]
        public Option Options { get; set; } = new Option();

    }

    public class Option
    {
        // TODO: find a way to make the index (0 to n) options dynamic, find out max number of options.
        [XmlElement("Option0", IsNullable = false)]
        public string Option1 { get; set; } = string.Empty;
        [XmlElement("Option1", IsNullable = false)]
        public string Option2 { get; set; } = string.Empty;
        [XmlElement("Option2", IsNullable = false)]
        public string Option3 { get; set; } = string.Empty;
        [XmlElement("Option3", IsNullable = false)]
        public string Option4 { get; set; } = string.Empty;
        [XmlElement("Option4", IsNullable = false)]
        public string Option5 { get; set; } = string.Empty;
        [XmlElement("Option5", IsNullable = false)]
        public string Option6 { get; set; } = string.Empty;
        [XmlElement("Option6", IsNullable = false)]
        public string Option7 { get; set; } = string.Empty;
        [XmlElement("ENTER", IsNullable = false)] // TODO: how to use this? conflicting with DisplayOnPos message, I think. 
        public string EnterText { get; set; }
        [XmlElement("CLEAR", IsNullable = false)] // TODO: how to use this? conflicting with DisplayOnPos message, I think. 
        public string ClearText { get; set; }
        [XmlElement("CANCEL", IsNullable = false)] // TODO: how to use this? conflicting with DisplayOnPos message, I think. 
        public string CancelText { get; set; }
        [XmlElement("FUNC", IsNullable = false)] // TODO: how to use this? conflicting with DisplayOnPos message, I think. 
        public string FuncText { get; set; }
        [XmlElement("F0", IsNullable = false)] // TODO: how to use this? conflicting with DisplayOnPos message, I think. 
        public string F0Text { get; set; }
        [XmlElement("F1", IsNullable = false)]
        public string F1Text { get; set; }
        [XmlElement("F2", IsNullable = false)]
        public string F2Text { get; set; }
        [XmlElement("F3", IsNullable = false)]
        public string F3Text { get; set; }
        [XmlElement("F4", IsNullable = false)]
        public string F4Text { get; set; }
        [XmlElement("F5", IsNullable = false)]
        public string F5Text { get; set; }
        [XmlElement("F6", IsNullable = false)]
        public string F6Text { get; set; }
        [XmlElement("F7", IsNullable = false)]
        public string F7Text { get; set; }
        [XmlElement("F8", IsNullable = false)]
        public string F8Text { get; set; }
        [XmlElement("F9", IsNullable = false)]
        public string F9Text { get; set; }
    }

    public class AcquiredOption
    {
        public string Result { get; set; }
        public string ResultText { get; set; }
        public bool PinpadCancelled { get; set; }
        public bool PosCancelled { get; set; }
        public string SelectedOption { get; set; }
        public string SelectedText { get; set; }
    }

}
