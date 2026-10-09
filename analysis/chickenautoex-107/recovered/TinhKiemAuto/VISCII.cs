using System.Text;

namespace TinhKiemAuto
{
	internal class VISCII : Encoding
	{
		public static readonly char[] Unicodes = new char[256]
		{
			'\0', '\u0001', 'Ẳ', '\u0003', '\u0004', 'Ẵ', 'Ẫ', '\a', '\b', '\t',
			'\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013',
			'Ỷ', '\u0015', '\u0016', '\u0017', '\u0018', 'Ỹ', '\u001a', '\u001b', '\u001c', '\u001d',
			'Ỵ', '\u001f', ' ', '!', '"', '#', '$', '%', '&', '\'',
			'(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
			'2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
			'<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E',
			'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
			'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
			'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c',
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
			'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', 'Ạ', 'Ắ',
			'Ằ', 'Ặ', 'Ấ', 'Ầ', 'Ẩ', 'Ậ', 'Ẽ', 'Ẹ', 'Ế', 'Ề',
			'Ể', 'Ễ', 'Ệ', 'Ố', 'Ồ', 'Ổ', 'Ỗ', 'Ộ', 'Ợ', 'Ớ',
			'Ờ', 'Ở', 'Ị', 'Ỏ', 'Ọ', 'Ỉ', 'Ủ', 'Ũ', 'Ụ', 'Ỳ',
			'Õ', 'ắ', 'ằ', 'ặ', 'ấ', 'ầ', 'ẩ', 'ậ', 'ẽ', 'ẹ',
			'ế', 'ề', 'ể', 'ễ', 'ệ', 'ố', 'ồ', 'ổ', 'ỗ', 'Ỡ',
			'Ơ', 'ộ', 'ờ', 'ở', 'ị', 'Ự', 'Ứ', 'Ừ', 'Ử', 'ơ',
			'ớ', 'Ư', 'À', 'Á', 'Â', 'Ã', 'Ả', 'Ă', 'ẳ', 'ẵ',
			'È', 'É', 'Ê', 'Ẻ', 'Ì', 'Í', 'Ĩ', 'ỳ', 'Đ', 'ứ',
			'Ò', 'Ó', 'Ô', 'ạ', 'ỷ', 'ừ', 'ử', 'Ù', 'Ú', 'ỹ',
			'ỵ', 'Ý', 'ỡ', 'ư', 'à', 'á', 'â', 'ã', 'ả', 'ă',
			'ữ', 'ẫ', 'è', 'é', 'ê', 'ẻ', 'ì', 'í', 'ĩ', 'ỉ',
			'đ', 'ự', 'ò', 'ó', 'ô', 'õ', 'ỏ', 'ọ', 'ụ', 'ù',
			'ú', 'ũ', 'ủ', 'ý', 'ợ', 'Ữ'
		};

		private VISCIIDecoder decoder;

		private VISCIIEncoder encoder;

		protected VISCIIDecoder Decoder
		{
			get
			{
				VISCIIDecoder vISCIIDecoder = decoder;
				if (vISCIIDecoder == null)
				{
					vISCIIDecoder = (decoder = new VISCIIDecoder());
				}
				DecoderFallback decoderFallback = base.DecoderFallback;
				if (decoderFallback != null && decoderFallback != vISCIIDecoder.Fallback)
				{
					vISCIIDecoder.Fallback = decoderFallback;
				}
				return vISCIIDecoder;
			}
		}

		protected VISCIIEncoder Encoder
		{
			get
			{
				VISCIIEncoder vISCIIEncoder = encoder;
				if (vISCIIEncoder == null)
				{
					vISCIIEncoder = (encoder = new VISCIIEncoder());
				}
				EncoderFallback encoderFallback = base.EncoderFallback;
				if (encoderFallback != null && encoderFallback != vISCIIEncoder.Fallback)
				{
					vISCIIEncoder.Fallback = encoderFallback;
				}
				return vISCIIEncoder;
			}
		}

		public override string BodyName => "viscii-simple";

		public override string EncodingName => BodyName;

		public override bool IsSingleByte => true;

		public override object Clone()
		{
			VISCII obj = (VISCII)base.Clone();
			obj.decoder = null;
			obj.encoder = null;
			return obj;
		}

		public override Decoder GetDecoder()
		{
			return new VISCIIDecoder();
		}

		public override Encoder GetEncoder()
		{
			return new VISCIIEncoder();
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return Encoder.GetByteCount(chars, index, count, flush: true);
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return Encoder.GetBytes(chars, charIndex, charCount, bytes, byteIndex, flush: true);
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return Decoder.GetCharCount(bytes, index, count, flush: true);
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return Decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex, flush: true);
		}

		public override int GetMaxByteCount(int charCount)
		{
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
		}
	}
}
