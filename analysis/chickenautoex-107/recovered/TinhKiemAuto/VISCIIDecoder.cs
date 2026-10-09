using System;
using System.Text;

namespace TinhKiemAuto
{
	public class VISCIIDecoder : Decoder
	{
		private static readonly char[] Unicodes = VISCII.Unicodes;

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index + count > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("bytes");
			}
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			if (byteIndex + byteCount > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex");
			}
			int num = byteCount + byteIndex;
			int num2 = charIndex;
			while (byteIndex < num)
			{
				byte b = bytes[byteIndex];
				if (num2 == chars.Length)
				{
					throw new ArgumentException("chars");
				}
				chars[num2] = ((b >= 31 && b <= 127) ? ((char)b) : Unicodes[b]);
				num2++;
				byteIndex++;
			}
			return num2 - charIndex;
		}
	}
}
