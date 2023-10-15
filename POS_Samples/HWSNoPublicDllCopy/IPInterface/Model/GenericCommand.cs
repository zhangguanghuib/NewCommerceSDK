using System;

namespace PCEFTPOS.EFTClient.IPInterface
{
	public enum CommandType
	{
		Display = '0',
		Print = '1',
		GetPassword = '2',
		Slave = 'Z',
        PayAtTable = '@',
        BasketData = '%'
    }

	public enum DisplayLocation
	{
		PINPad = '0',
		POS = '1'
	}

	[Flags()]
	public enum PINPadKeyMap
	{
		EnterKey = 0x01,
		ClearKey = 0x02
	}

	[Flags()]
	public enum POSKeyMap
	{
		OKKey = 0x01,
		CancelKey = 0x02
	}

	public enum PrinterLocation
	{
		NPT = 0x01
	}

	public enum PasswordDisplay
	{
		Enter_Code = '1'
	}

	public class EFTGetPasswordRequest : EFTRequest
	{
        public EFTGetPasswordRequest() : base(true, typeof(EFTGetPasswordResponse))
        {
        }
            
        public int MinPasswordLength { get; set; }
		public int MaxPassworkLength { get; set; }
		public int Timeout { get; set; }
		public PasswordDisplay PasswordDisplay { get; set; }
	}

	public class EFTGetPasswordResponse : EFTResponse
	{
        public EFTGetPasswordResponse() : base(typeof(EFTGetPasswordRequest))
        {
        }

        public string Password { get; set; }

        /// <summary>Indicates if the request was successful.</summary>
        /// <value>Type: <see cref="System.Boolean"/></value>
        public bool Success { get; set; } = false;

        /// <summary>The response code of the request.</summary>
        /// <value>Type: <see cref="System.String"/><para>A 2 character response code. "00" indicates a successful response.</para></value>
        public string ResponseCode { get; set; } = "";

        /// <summary>The response text for the response code.</summary>
        /// <value>Type: <see cref="System.String"/></value>
        public string ResponseText { get; set; } = "";
    }

    public class EFTPayAtTableRequest : EFTRequest
    {
        public EFTPayAtTableRequest(): base(false, typeof(EFTPayAtTableResponse))
        {
        }

        public string Header { get; set; }
        public string Content { get; set; }
    }

    public class EFTPayAtTableResponse : EFTResponse
    {
        public EFTPayAtTableResponse() : base(typeof(EFTPayAtTableResponse))
        {

        }

        public string Header { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}