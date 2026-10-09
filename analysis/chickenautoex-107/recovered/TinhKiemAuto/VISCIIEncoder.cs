using System;
using System.Text;

namespace TinhKiemAuto
{
	public class VISCIIEncoder : Encoder
	{
		private static readonly byte[] VISCIIs;

		protected char HighSurrogate { get; set; }

		static VISCIIEncoder()
		{
			VISCIIs = new byte[7930];
			for (int i = 0; i < VISCII.Unicodes.Length; i++)
			{
				VISCIIs[(uint)VISCII.Unicodes[i]] = (byte)i;
			}
		}

		public override int GetByteCount(char[] chars, int index, int count, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index + count > chars.Length)
			{
				throw new ArgumentOutOfRangeException("chars");
			}
			EncoderFallbackBuffer encoderFallbackBuffer = null;
			char c = HighSurrogate;
			int count2 = 0;
			for (int num = index + count; index < num; index++)
			{
				char c2 = chars[index];
				if (c != 0)
				{
					if (encoderFallbackBuffer == null)
					{
						encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
					}
					if (char.IsLowSurrogate(c2))
					{
						if (encoderFallbackBuffer.Fallback(c, c2, index - 1))
						{
							HandleFallbackCount(encoderFallbackBuffer, ref count2);
						}
						c = '\0';
						continue;
					}
					if (encoderFallbackBuffer.Fallback(c, index - 1))
					{
						HandleFallbackCount(encoderFallbackBuffer, ref count2);
					}
					c = '\0';
				}
				if (c2 < VISCIIs.Length && (VISCIIs[(uint)c2] != 0 || c2 == '\0'))
				{
					count2++;
					continue;
				}
				if (char.IsHighSurrogate(c2))
				{
					c = c2;
					continue;
				}
				if (encoderFallbackBuffer == null)
				{
					encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
				}
				if (encoderFallbackBuffer.Fallback(c2, index))
				{
					HandleFallbackCount(encoderFallbackBuffer, ref count2);
				}
			}
			if (flush && c != 0)
			{
				if (encoderFallbackBuffer == null)
				{
					encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
				}
				if (encoderFallbackBuffer.Fallback(c, index - 1))
				{
					HandleFallbackCount(encoderFallbackBuffer, ref count2);
				}
			}
			return count2;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (charIndex + charCount > chars.Length)
			{
				throw new ArgumentOutOfRangeException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex");
			}
			EncoderFallbackBuffer encoderFallbackBuffer = null;
			char c = HighSurrogate;
			int num = charIndex + charCount;
			int byteIndex2 = byteIndex;
			for (; charIndex < num; charIndex++)
			{
				char c2 = chars[charIndex];
				if (c != 0)
				{
					if (encoderFallbackBuffer == null)
					{
						encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
					}
					if (char.IsLowSurrogate(c2))
					{
						if (encoderFallbackBuffer.Fallback(c, c2, charIndex - 1))
						{
							HandleFallbackWrite(encoderFallbackBuffer, bytes, ref byteIndex2);
						}
						c = '\0';
						continue;
					}
					if (encoderFallbackBuffer.Fallback(c, charIndex - 1))
					{
						HandleFallbackWrite(encoderFallbackBuffer, bytes, ref byteIndex2);
					}
					c = '\0';
				}
				byte b;
				if (c2 < VISCIIs.Length && ((b = VISCIIs[(uint)c2]) != 0 || c2 == '\0'))
				{
					WriteByte(bytes, byteIndex2, b);
					byteIndex2++;
					continue;
				}
				if (char.IsHighSurrogate(c2))
				{
					c = c2;
					continue;
				}
				if (encoderFallbackBuffer == null)
				{
					encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
				}
				if (encoderFallbackBuffer.Fallback(c2, charIndex))
				{
					HandleFallbackWrite(encoderFallbackBuffer, bytes, ref byteIndex2);
				}
			}
			if (flush)
			{
				if (c != 0)
				{
					if (encoderFallbackBuffer == null)
					{
						encoderFallbackBuffer = (base.Fallback ?? EncoderFallback.ReplacementFallback).CreateFallbackBuffer();
					}
					if (encoderFallbackBuffer.Fallback(c, charIndex - 1))
					{
						HandleFallbackWrite(encoderFallbackBuffer, bytes, ref byteIndex2);
					}
				}
			}
			else
			{
				HighSurrogate = c;
			}
			return byteIndex2 - byteIndex;
		}

		protected static void HandleFallbackCount(EncoderFallbackBuffer fallbackBuffer, ref int count)
		{
			while (fallbackBuffer.Remaining > 0)
			{
				char nextChar = fallbackBuffer.GetNextChar();
				if (nextChar >= VISCIIs.Length || (VISCIIs[(uint)nextChar] == 0 && nextChar != 0))
				{
					throw new EncoderFallbackException();
				}
				count++;
			}
		}

		protected static void HandleFallbackWrite(EncoderFallbackBuffer fallbackBuffer, byte[] bytes, ref int byteIndex)
		{
			while (fallbackBuffer.Remaining > 0)
			{
				char nextChar = fallbackBuffer.GetNextChar();
				byte b;
				if (nextChar >= VISCIIs.Length || ((b = VISCIIs[(uint)nextChar]) == 0 && nextChar != 0))
				{
					throw new EncoderFallbackException();
				}
				WriteByte(bytes, byteIndex, b);
				byteIndex++;
			}
		}

		protected static void WriteByte(byte[] bytes, int byteIndex, byte b)
		{
			if (byteIndex == bytes.Length)
			{
				throw new ArgumentException("bytes");
			}
			bytes[byteIndex] = b;
		}
	}
}
