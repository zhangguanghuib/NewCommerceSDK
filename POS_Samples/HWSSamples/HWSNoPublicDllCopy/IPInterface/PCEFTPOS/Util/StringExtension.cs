namespace PCEFTPOS.EFTClient.IPInterface
{
	public static class StringExtension
	{
		/// <summary>
		/// Pad or cut a string so that the length is equal to totalWidth
		/// </summary>
		/// <param name="v"></param>
		/// <param name="totalWidth">The width of the string to return</param>
		/// <returns>A string of length totalWidth</returns>
		public static string PadRightAndCut(this string v, int totalWidth)
		{
			if (v.Length == totalWidth)
				return v;
			else if (v.Length < totalWidth)
				return v.PadRight(totalWidth);
			else
				return v.Substring(0, totalWidth);
		}

		/// <summary>
		/// Pad or cut a string so that the length is equal to totalWidth
		/// </summary>
		/// <param name="v"></param>
		/// <param name="totalWidth">The width of the string to return</param>
		/// <param name="paddingChar">The char to use for padding if required</param>/// 
		/// <returns>A string of length totalWidth</returns>
		/// <summary>
		public static string PadRightAndCut(this string v, int totalWidth, char paddingChar)
		{
			if (v.Length == totalWidth)
				return v;
			else if (v.Length < totalWidth)
				return v.PadRight(totalWidth, paddingChar);
			else
				return v.Substring(0, totalWidth);
		}

		/// <summary>
		/// Returns a substring of totalWidth length, or if input length is less than, leaves it alone
		/// </summary>
		/// <param name="v"></param>
		/// <param name="totalWidth"></param>
		/// <returns></returns>
		public static string CutAndLeave(this string v, int totalWidth)
		{
			if (v.Length > totalWidth)
				return v.Substring(0, totalWidth);
			else
				return v;
		}
	}
}
