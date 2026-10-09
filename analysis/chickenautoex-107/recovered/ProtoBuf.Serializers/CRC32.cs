using System;

namespace ProtoBuf.Serializers
{
	internal class CRC32
	{
		private uint crc;

		private static uint[] crcTable = makeCrcTable();

		private static uint[] makeCrcTable()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				int num2 = 8;
				while (--num2 >= 0)
				{
					num = (((num & 1) == 0) ? (num >> 1) : (0xEDB88320u ^ (num >> 1)));
				}
				array[i] = num;
			}
			return array;
		}

		public uint getValue()
		{
			return crc & 0xFFFFFFFFu;
		}

		public void reset()
		{
			crc = 0u;
		}

		public void update(byte[] buf)
		{
			uint num = 0u;
			int num2 = buf.Length;
			uint num3 = ~crc;
			while (--num2 >= 0)
			{
				num3 = crcTable[(uint)(UIntPtr)((num3 ^ buf[(uint)(UIntPtr)(num++)]) & 0xFF)] ^ (num3 >> 8);
			}
			crc = ~num3;
		}

		public void update(byte[] buf, int off, int len)
		{
			uint num = ~crc;
			while (--len >= 0)
			{
				num = crcTable[(uint)(UIntPtr)((num ^ buf[off++]) & 0xFF)] ^ (num >> 8);
			}
			crc = ~num;
		}
	}
}
