using System;
using System.Text;

namespace PCEFTPOS.EFTClient.IPInterface
{
	/// <summary>
	/// Summary description for DirectEncoding.
	/// </summary>
	public class DirectEncoding: System.Text.Encoding
	{
        private static volatile DirectEncoding instance;
        private static object syncRoot = new Object();

        private DirectEncoding()
		{			
		}

		public static DirectEncoding DIRECT
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DirectEncoding();
                    }
                }
                return instance;
            }
        }

		public override int GetByteCount(char[] chars)
		{
			return chars.Length;
		}

		public override int GetByteCount(string s)
		{
			return s.Length;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return count;
		}

		public override byte[] GetBytes(char[] chars)
		{
			return GetBytes(chars, 0, chars.Length);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			for(int i=charIndex, j=byteIndex; i<charCount; i++, j++)
				bytes[j] = (byte)chars[i];
			return charCount;
		}

		public override byte[] GetBytes(char[] chars, int index, int count)
		{
			byte[] bytes = new byte[count];
			GetBytes(chars, index, count, bytes, 0);
			return bytes;						
		}

		public override byte[] GetBytes(string s)
		{
			return GetBytes(s.ToCharArray());
		}

		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return GetBytes(s.ToCharArray(), charIndex, charCount, bytes, byteIndex);			
		}

		public override int GetCharCount(byte[] bytes)
		{
			return bytes.Length;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return count;
		}

		public override char[] GetChars(byte[] bytes, int index, int count)
		{
			char[] chars = new char[count];
			GetChars(bytes, index, count, chars, 0);
			return chars;		
		}

		public override char[] GetChars(byte[] bytes)
		{
			return GetChars(bytes, 0, bytes.Length);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			for(int i=byteIndex, j=charIndex; i<byteCount; i++, j++)
				chars[j] = (char)bytes[i];
			return byteCount;
        }

        public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}

		public override string GetString(byte[] bytes)
		{
			return GetString(bytes, 0, bytes.Length);
		}

		public override string GetString(byte[] bytes, int index, int count)
		{			
			return new String(this.GetChars(bytes, index, count));
		}

		private void Test()
		{
			int i;
			string s = "";
			byte[] bytes;
			char[] chars = new char[255];


			for(i=0; i<255; i++) s += (char)i;
			bytes = DirectEncoding.DIRECT.GetBytes(s);

			for(i=0; i<255; i++) chars[i]=(char)i;
			bytes = DirectEncoding.DIRECT.GetBytes(chars);

			for(i=0; i<255; i++) chars[i]=(char)i;
			s = new String(chars, 0, chars.Length);
			bytes = DirectEncoding.DIRECT.GetBytes(s);
		}
	
		

	}
}
