using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCEFTPOS.EFTClient.IPInterface.Slave
{
	public enum CommandCode
	{
		NullCommand = ' ', SlaveMode = 'S', Status = 'N', DisplayMode = 'Z', Display = 'D', Print = 'P', MagneticCardRead = 'J', Audio = 'A', Key = 'K', Input = 'E', Storage = 'M',
		Connection = 'C', External = '+', SendReceive = 'X', ICC = 'I',
	}
	public enum SlaveStatusType { Version = '0', DateTime = '1', SerialNumber = '2' }
	public enum LightStatus { NoChange = ' ', Off = '0', On = '1', Auto = 'A' }
	public enum TextAlignment { Left, Centre, Right }
	public enum CardReadOptions { Once = '1', Off = '0', Multiple = '2' }
	public enum KeyOptions { Once = '1', Off = '0', Multiple = '2' }
	public enum InputPromptMode { Default = ' ', AmountEntry = '$', Apha = 'A', Password = '*' }
	public enum KeyPressed { Enter = 'E', Cancel = 'C', Clear = 'B', ClearAll = 'D', Function = 'F', Alpha = 'A', Cheque = 'X', Savings = 'Y', Credit = 'Z', F0 = 'a', F1 = 'b', F2 = 'c', F3 = 'd', F4 = 'e', F5 = 'f', F6 = 'g', F7 = 'h', F8 = 'i', F9 = 'j', Zero = '0', One = '1', Two = '2', Three = '3', Four = '4', Five = '5', Six = '6', Seven = '7', Eight = '8', Nine = '9' }

    public enum SlaveMode { Exit = '0', Enter = '1' }
    public enum ComPortSpeed { NoChange = ' ', Baud9600 = '0', Baud19200 = '1', Baud38400 = '2', Baud115200 = '3' }

	[Flags]
	public enum KeyMask { Cancel = 0x01, Enter = 0x02, Clear = 0x04, NumberKeys = 0x08, FunctionKeys = 0x10, AccountKeys = 0x20, Function = 0x40, OtherKeys = 0x80 }
	[Flags]
	public enum TrackFlags { Track1 = 0x01, Track2 = 0x02, Track3 = 0x04 }

    /// <summary>
    /// Base class for all slave request commands
    /// </summary>
    public class SlaveCommandRequest
    {
    }

    /// <summary>
    /// Enter/exit slave mode
    /// </summary>
    public class SlaveCommandRequestMode : SlaveCommandRequest
    {
        public SlaveMode SlaveMode { get; set; } = SlaveMode.Exit;

        /// <summary>
        /// Seconds to stay in slave mode(‘000’ is infinite). Maximum = 999
        /// </summary>
        int _timeout = 60;
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                if (value < 0)
                    _timeout = 0;
                else if (value > 999)
                    _timeout = 999;
                else
                    _timeout = value;
            }
        }

        public ComPortSpeed ComPortSpeed { get; set; } = ComPortSpeed.NoChange;
    }

    public class SlaveCommandRequestStatus: SlaveCommandRequest
    {
    }





    public class SlaveHelper
	{
		public static SlaveCommandResponse ResponseHelper( EFTSlaveResponse Response )
		{
			string response = Response.Response;
			int index = 0;
			if( response[index++] != '*' )
				return new SlaveCommandResponse() { ResponseCode = "FF", ResponseText = "Invalid response" };
			char applicationID = response[index++];
			char deviceCode = response[index++];
			CommandCode eventCode = (CommandCode)response[index++];
			string responseCode = response.Substring( index, 2 ); index += 2;

			SlaveCommandResponse commandResponse = new SlaveCommandResponse();
			if( response.Length > index )
			{
				int length = 0;
				string data = "";
				try
				{
					length = int.Parse( response.Substring( index, 3 ) );
					index += 3;
					data = response.Substring( index, length );
					index += length;
				}
				catch
				{
					return new SlaveCommandResponse() { ResponseCode = "FF", ResponseText = "Invalid response" };
				}

				if( data.Length > 0 )
				{
					int dataIndex = 0;
					switch( eventCode )
					{
						case CommandCode.Status:
							commandResponse = new StatusResponse();
							break;
						case CommandCode.MagneticCardRead:
							TrackFlags tracksAvailable = (TrackFlags)( data[dataIndex++] - '0' );
							int trackDataLength = 0;
							if( ( tracksAvailable & TrackFlags.Track2 ) > 0 ) trackDataLength += 40;
							if( ( tracksAvailable & TrackFlags.Track1 ) > 0 ) trackDataLength += 80;
							if( ( tracksAvailable & TrackFlags.Track3 ) > 0 ) trackDataLength += 80;
							if( data.Length - dataIndex < trackDataLength )
								return new SlaveCommandResponse() { ResponseCode = "FF", ResponseText = "Invalid response" };
							commandResponse = new MagneticCardReadResponse( tracksAvailable, data.Substring( dataIndex, trackDataLength ) );
							break;
						case CommandCode.Key:
							commandResponse = new KeyResponse( data[dataIndex++] );
							break;
						case CommandCode.Input:
							commandResponse = new InputResponse( data );
							break;
					}
					commandResponse.ResponseString = data;
				}
			}
			commandResponse.ApplicationID = applicationID;
			commandResponse.DeviceCode = deviceCode;
			commandResponse.EventCode = eventCode;
			commandResponse.ResponseCode = responseCode;
			commandResponse.ResponseText = GetResponseText( responseCode );
			return commandResponse;
		}

		static string GetResponseText( string ResponseCode )
		{
			if( ResponseCode == "00" ) return "OK";
			if( ResponseCode == "TM" ) return "Operation Cancelled";
			if( ResponseCode == "TO" ) return "Operation Timeout";
			if( ResponseCode == "OM" ) return "Out of Storage Memory";
			if( ResponseCode == "NF" ) return "Not Found";
			if( ResponseCode == "NR" ) return "Not Ready";
			if( ResponseCode == "F0" ) return "Invalid Request";
			if( ResponseCode == "F1" ) return "Operation Failed";
			if( ResponseCode == "F2" ) return "System Error";
			if( ResponseCode == "F3" ) return "Terminal Busy";
			if( ResponseCode == "Z0" ) return "Modem Error";
			if( ResponseCode == "Z1" ) return "No Dial Tone";
			if( ResponseCode == "Z2" ) return "No Answer";
			if( ResponseCode == "Z3" ) return "Line Busy";
			if( ResponseCode == "Z4" ) return "Number Missing";
			if( ResponseCode == "Z6" ) return "No Carrier";
			return "System Error";
		}
	}

	public class SlaveCommandBuilder
	{
		const char STX = (char)0x02;
		const char ETX = (char)0x03;
		const char DLE = (char)0x10;

		List<string> commands = new List<string>();
		char applicationID = '@';
		char deviceCode = '1';

		#region Slave Mode
    	public void AddEnterSlaveModeRequest()
		{
			AddEnterSlaveModeRequest( 0 );
		}
		public void AddEnterSlaveModeRequest( int Timeout )
		{
			string command = ( (char)CommandCode.SlaveMode ).ToString() + '1';
			command += "004";
			command += Timeout.ToString( "000" );
			command += " ";
			commands.Add( command );
		}
		public void AddExitSlaveModeRequest()
		{
			string command = ( (char)CommandCode.SlaveMode ).ToString() + '0';
			command += "000";
			commands.Add( command );
		}

		#endregion

		#region Status

		public void AddStatusRequest( StatusType Type )
		{
			string command = ( (char)CommandCode.Status ).ToString();
			command += ( (char)Type ).ToString();
			command += "000";
			commands.Add( command );
		}

		#endregion

		#region Display Mode

		public void AddDisplayModeRequest( int NumLines, int NumColumns, bool ClearDisplay )
		{
			AddDisplayModeRequest( NumLines, NumColumns, ClearDisplay, LightStatus.NoChange );
		}
		public void AddDisplayModeRequest( int NumLines, int NumColumns, bool ClearDisplay, LightStatus LightStatus )
		{
			string command = ( (char)CommandCode.DisplayMode ).ToString();
			command += ' ';
			command += "006";
			command += NumLines.ToString( "00" );
			command += NumColumns.ToString( "00" );
			command += (char)LightStatus;
			command += ClearDisplay ? '1' : '0';
			commands.Add( command );
		}

		#endregion

		#region Display

		public void AddDisplayTextRequest( int DisplayLine, int DisplayColumn, string DisplayText )
		{
			string displayText = DisplayText.Length > 30 ? DisplayText.Substring( 0, 30 ) : DisplayText;
			int displayLine = DisplayLine < 0 || DisplayLine > 7 ? 0 : DisplayLine - 1;
			int displayColumn = DisplayColumn < 0 || DisplayColumn > 99 ? 0 : DisplayColumn - 1;
			string command = ( (char)CommandCode.Display ).ToString();
			command += ' ';
			command += ( displayText.Length + 4 ).ToString( "000" );
			command += displayLine.ToString( "00" );
			command += DisplayColumn.ToString( "00" );
			command += displayText;
			commands.Add( command );
		}
		public void AddDisplayTextRequest( int DisplayLine, int DisplayColumn, string DisplayText, TextAlignment Alignment, int LineWidth )
		{
			string displayText = DisplayText.Length > 30 ? DisplayText.Substring( 0, 30 ) : DisplayText;
			if( Alignment == TextAlignment.Centre )
				displayText = displayText.PadLeft( displayText.Length + ( LineWidth - displayText.Length ) / 2 );
			else if( Alignment == TextAlignment.Right )
				displayText = displayText.PadLeft( LineWidth );
			int displayLine = DisplayLine < 0 || DisplayLine > 7 ? 0 : DisplayLine - 1;
			int displayColumn = DisplayColumn < 0 || DisplayColumn > 99 ? 0 : DisplayColumn - 1;
			string command = ( (char)CommandCode.Display ).ToString();
			command += ' ';
			command += ( displayText.Length + 4 ).ToString( "000" );
			command += displayLine.ToString( "00" );
			command += DisplayColumn.ToString( "00" );
			command += displayText;
			commands.Add( command );
		}
		public void AddDisplayImageRequest( int DisplayLine, int DisplayColumn, int ImageID )
		{
			int displayLine = DisplayLine < 0 || DisplayLine > 7 ? 0 : DisplayLine - 1;
			int displayColumn = DisplayColumn < 0 || DisplayColumn > 99 ? 0 : DisplayColumn - 1;
			string command = ( (char)CommandCode.Display ).ToString();
			command += ' ';
			command += "006";
			command += displayLine.ToString( "00" );
			command += displayColumn.ToString( "00" );
			command += ImageID.ToString( "00" ); ;
			commands.Add( command );
		}

		#endregion

		#region Magnetic Card Read

		public void AddMagneticCardReadRequest(CardReadOptions Options)
		{
			string command = ( (char)CommandCode.MagneticCardRead ).ToString();
			command += (char)Options;
			command += "000";
			commands.Add( command );
		}
		
		#endregion

		#region Audio

		public void AddAudioRequest( string AudioPattern )
		{
			string command = ( (char)CommandCode.Audio ).ToString();
			command += AudioPattern.Length.ToString( "000" );
			command += AudioPattern;
			commands.Add( command );
		}

		#endregion

		#region Key

		public void AddKeyRequest( KeyOptions Options, KeyMask KeyMask )
		{
			string command = ( (char)CommandCode.Key ).ToString();
			command += (char)Options;
			command += "008";
			command += ( (byte)KeyMask & 0x01 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x02 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x04 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x08 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x10 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x20 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x40 ) > 0 ? '1' : '0';
			command += ( (byte)KeyMask & 0x80 ) > 0 ? '1' : '0';
			commands.Add( command );
		}
		
		#endregion

		#region Input

		public void AddInputRequest( InputPromptMode Mode, int Timeout, int MinLength, int MaxLength, int EntryLine, string DefaultText )
		{
			int entryLine = EntryLine < 0 || EntryLine > 9 ? 0 : EntryLine - 1;
			string command = ( (char)CommandCode.Input ).ToString();
			command += (char)Mode;
			command += ( 8 + DefaultText.Length ).ToString( "000" );
			command += Timeout.ToString( "000" );
			command += MinLength.ToString( "00" );
			command += MaxLength.ToString( "00" );
			command += EntryLine.ToString( "0" );
			command += DefaultText;
			commands.Add( command );
		}
		
		#endregion

		string CreateSlaveCommandString()
		{
            var sb = new StringBuilder();
            sb.Append("*");
            sb.Append(applicationID);
            sb.Append(deviceCode);
            sb.Append(commands.Count.ToString("00"));
            foreach(var c in commands)
            {
                sb.Append(c);
            }
            return sb.ToString();
		}

		public string CommandString { get { return CreateSlaveCommandString(); } }
		public char ApplicationID
		{
			get { return applicationID; }
			set { applicationID = value; }
		}
		public char DeviceCode
		{
			get { return deviceCode; }
			set { deviceCode = value; }
		}
	}

	public class SlaveCommandResponse
	{
		public bool Success { get { return ResponseCode == "00"; } }
		public string ResponseCode { get; set; }
		public string ResponseString { get; set; }
		public string ResponseText { get; set; }
		public char ApplicationID { get; set; }
		public char DeviceCode { get; set; }
		public CommandCode EventCode { get; set; }
	}

	#region Status

	public class StatusResponse : SlaveCommandResponse
	{
	}

	#endregion

	#region Magnetic Card Read

	public class MagneticCardReadResponse : SlaveCommandResponse
	{
		public MagneticCardReadResponse( TrackFlags TracksAvailable, string TrackData )
		{
			int index = 0;
			if( ( TracksAvailable & TrackFlags.Track2 ) > 0 )
			{
				Track2 = TrackData.Substring( index, 40 );
				index += 40;
			}
			if( ( TracksAvailable & TrackFlags.Track1 ) > 0 )
			{
				Track1 = TrackData.Substring( index, 80 );
				index += 80;
			}
			if( ( TracksAvailable & TrackFlags.Track3 ) > 0 )
				Track3 = TrackData.Substring( index, 80 );
		}

		public string Track1 { get; private set; }
		public string Track2 { get; private set; }
		public string Track3 { get; private set; }
	}

	#endregion

	#region Key

	public class KeyResponse : SlaveCommandResponse
	{
		public KeyResponse( char Key )
		{
			this.Key = (KeyPressed)Key;
		}

		public KeyPressed Key { get; private set; }
	}

	#endregion

	#region Input

	public class InputResponse : SlaveCommandResponse
	{
		public InputResponse( string Input )
		{
			this.Input = Input;
		}

		public string Input { get; private set; }
	}
	
	#endregion
}
