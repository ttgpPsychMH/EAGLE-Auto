using System;

namespace ComponentAce.Compression.Libs.zlib
{
	internal sealed class InfCodes
	{
		private const int Z_OK = 0;

		private const int Z_STREAM_END = 1;

		private const int Z_NEED_DICT = 2;

		private const int Z_ERRNO = -1;

		private const int Z_STREAM_ERROR = -2;

		private const int Z_DATA_ERROR = -3;

		private const int Z_MEM_ERROR = -4;

		private const int Z_BUF_ERROR = -5;

		private const int Z_VERSION_ERROR = -6;

		private const int START = 0;

		private const int LEN = 1;

		private const int LENEXT = 2;

		private const int DIST = 3;

		private const int DISTEXT = 4;

		private const int COPY = 5;

		private const int LIT = 6;

		private const int WASH = 7;

		private const int END = 8;

		private const int BADCODE = 9;

		private static readonly int[] inflate_mask = new int[17]
		{
			0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
			1023, 2047, 4095, 8191, 16383, 32767, 65535
		};

		internal int mode;

		internal int len;

		internal int[] tree;

		internal int tree_index;

		internal int need;

		internal int lit;

		internal int get_Renamed;

		internal int dist;

		internal byte lbits;

		internal byte dbits;

		internal int[] ltree;

		internal int ltree_index;

		internal int[] dtree;

		internal int dtree_index;

		internal InfCodes(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, ZStream z)
		{
			mode = 0;
			lbits = (byte)bl;
			dbits = (byte)bd;
			ltree = tl;
			ltree_index = tl_index;
			dtree = td;
			dtree_index = td_index;
		}

		internal InfCodes(int bl, int bd, int[] tl, int[] td, ZStream z)
		{
			mode = 0;
			lbits = (byte)bl;
			dbits = (byte)bd;
			ltree = tl;
			ltree_index = 0;
			dtree = td;
			dtree_index = 0;
		}

		internal int proc(InfBlocks s, ZStream z, int r)
		{
			int num = z.next_in_index;
			int num2 = z.avail_in;
			int num3 = s.bitb;
			int i = s.bitk;
			int num4 = s.write;
			int num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
			while (true)
			{
				switch (mode)
				{
				case 0:
					if (num5 >= 258 && num2 >= 10)
					{
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						r = inflate_fast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, s, z);
						num = z.next_in_index;
						num2 = z.avail_in;
						num3 = s.bitb;
						i = s.bitk;
						num4 = s.write;
						num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
						if (r != 0)
						{
							mode = ((r == 1) ? 7 : 9);
							break;
						}
					}
					need = lbits;
					tree = ltree;
					tree_index = ltree_index;
					mode = 1;
					goto case 1;
				case 2:
				{
					int num6;
					for (num6 = get_Renamed; i < num6; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (z.next_in[num++] & 0xFF) << i;
							continue;
						}
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						return s.inflate_flush(z, r);
					}
					len += num3 & inflate_mask[num6];
					num3 >>= num6;
					i -= num6;
					need = dbits;
					tree = dtree;
					tree_index = dtree_index;
					mode = 3;
					goto case 3;
				}
				case 4:
				{
					int num6;
					for (num6 = get_Renamed; i < num6; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (z.next_in[num++] & 0xFF) << i;
							continue;
						}
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						return s.inflate_flush(z, r);
					}
					dist += num3 & inflate_mask[num6];
					num3 >>= num6;
					i -= num6;
					mode = 5;
					goto case 5;
				}
				case 6:
					if (num5 == 0)
					{
						if (num4 == s.end && s.read != 0)
						{
							num4 = 0;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
						}
						if (num5 == 0)
						{
							s.write = num4;
							r = s.inflate_flush(z, r);
							num4 = s.write;
							num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							if (num4 == s.end && s.read != 0)
							{
								num4 = 0;
								num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							}
							if (num5 == 0)
							{
								s.bitb = num3;
								s.bitk = i;
								z.avail_in = num2;
								z.total_in += num - z.next_in_index;
								z.next_in_index = num;
								s.write = num4;
								return s.inflate_flush(z, r);
							}
						}
					}
					r = 0;
					s.window[num4++] = (byte)lit;
					num5--;
					mode = 0;
					break;
				case 1:
				{
					int num6;
					for (num6 = need; i < num6; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (z.next_in[num++] & 0xFF) << i;
							continue;
						}
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						return s.inflate_flush(z, r);
					}
					int num7 = (tree_index + (num3 & inflate_mask[num6])) * 3;
					num3 = SupportClass.URShift(num3, tree[num7 + 1]);
					i -= tree[num7 + 1];
					int num8 = tree[num7];
					if (num8 == 0)
					{
						lit = tree[num7 + 2];
						mode = 6;
						break;
					}
					if ((num8 & 0x10) != 0)
					{
						get_Renamed = num8 & 0xF;
						len = tree[num7 + 2];
						mode = 2;
						break;
					}
					if ((num8 & 0x40) == 0)
					{
						need = num8;
						tree_index = num7 / 3 + tree[num7 + 2];
						break;
					}
					if ((num8 & 0x20) != 0)
					{
						mode = 7;
						break;
					}
					mode = 9;
					z.msg = "invalid literal/length code";
					r = -3;
					s.bitb = num3;
					s.bitk = i;
					z.avail_in = num2;
					z.total_in += num - z.next_in_index;
					z.next_in_index = num;
					s.write = num4;
					return s.inflate_flush(z, r);
				}
				case 3:
				{
					int num6;
					for (num6 = need; i < num6; i += 8)
					{
						if (num2 != 0)
						{
							r = 0;
							num2--;
							num3 |= (z.next_in[num++] & 0xFF) << i;
							continue;
						}
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						return s.inflate_flush(z, r);
					}
					int num7 = (tree_index + (num3 & inflate_mask[num6])) * 3;
					num3 >>= tree[num7 + 1];
					i -= tree[num7 + 1];
					int num8 = tree[num7];
					if ((num8 & 0x10) != 0)
					{
						get_Renamed = num8 & 0xF;
						dist = tree[num7 + 2];
						mode = 4;
						break;
					}
					if ((num8 & 0x40) == 0)
					{
						need = num8;
						tree_index = num7 / 3 + tree[num7 + 2];
						break;
					}
					mode = 9;
					z.msg = "invalid distance code";
					r = -3;
					s.bitb = num3;
					s.bitk = i;
					z.avail_in = num2;
					z.total_in += num - z.next_in_index;
					z.next_in_index = num;
					s.write = num4;
					return s.inflate_flush(z, r);
				}
				case 5:
				{
					int j;
					for (j = num4 - dist; j < 0; j += s.end)
					{
					}
					while (len != 0)
					{
						if (num5 == 0)
						{
							if (num4 == s.end && s.read != 0)
							{
								num4 = 0;
								num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
							}
							if (num5 == 0)
							{
								s.write = num4;
								r = s.inflate_flush(z, r);
								num4 = s.write;
								num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
								if (num4 == s.end && s.read != 0)
								{
									num4 = 0;
									num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
								}
								if (num5 == 0)
								{
									s.bitb = num3;
									s.bitk = i;
									z.avail_in = num2;
									z.total_in += num - z.next_in_index;
									z.next_in_index = num;
									s.write = num4;
									return s.inflate_flush(z, r);
								}
							}
						}
						s.window[num4++] = s.window[j++];
						num5--;
						if (j == s.end)
						{
							j = 0;
						}
						len--;
					}
					mode = 0;
					break;
				}
				default:
					r = -2;
					s.bitb = num3;
					s.bitk = i;
					z.avail_in = num2;
					z.total_in += num - z.next_in_index;
					z.next_in_index = num;
					s.write = num4;
					return s.inflate_flush(z, r);
				case 7:
					if (i > 7)
					{
						i -= 8;
						num2++;
						num--;
					}
					s.write = num4;
					r = s.inflate_flush(z, r);
					num4 = s.write;
					if (num4 >= s.read)
					{
						_ = s.end;
					}
					else
					{
						_ = s.read;
					}
					if (s.read != s.write)
					{
						s.bitb = num3;
						s.bitk = i;
						z.avail_in = num2;
						z.total_in += num - z.next_in_index;
						z.next_in_index = num;
						s.write = num4;
						return s.inflate_flush(z, r);
					}
					mode = 8;
					goto case 8;
				case 8:
					r = 1;
					s.bitb = num3;
					s.bitk = i;
					z.avail_in = num2;
					z.total_in += num - z.next_in_index;
					z.next_in_index = num;
					s.write = num4;
					return s.inflate_flush(z, r);
				case 9:
					r = -3;
					s.bitb = num3;
					s.bitk = i;
					z.avail_in = num2;
					z.total_in += num - z.next_in_index;
					z.next_in_index = num;
					s.write = num4;
					return s.inflate_flush(z, r);
				}
			}
		}

		internal void free(ZStream z)
		{
		}

		internal int inflate_fast(int bl, int bd, int[] tl, int tl_index, int[] td, int td_index, InfBlocks s, ZStream z)
		{
			int next_in_index = z.next_in_index;
			int num = z.avail_in;
			int num2 = s.bitb;
			int num3 = s.bitk;
			int num4 = s.write;
			int num5 = ((num4 < s.read) ? (s.read - num4 - 1) : (s.end - num4));
			int num6 = inflate_mask[bl];
			int num7 = inflate_mask[bd];
			int num10;
			while (true)
			{
				if (num3 < 20)
				{
					num--;
					num2 |= (z.next_in[next_in_index++] & 0xFF) << num3;
					num3 += 8;
					continue;
				}
				int num8 = num2 & num6;
				int num9;
				if ((num9 = tl[(tl_index + num8) * 3]) == 0)
				{
					num2 >>= tl[(tl_index + num8) * 3 + 1];
					num3 -= tl[(tl_index + num8) * 3 + 1];
					s.window[num4++] = (byte)tl[(tl_index + num8) * 3 + 2];
					num5--;
				}
				else
				{
					while (true)
					{
						num2 >>= tl[(tl_index + num8) * 3 + 1];
						num3 -= tl[(tl_index + num8) * 3 + 1];
						if ((num9 & 0x10) == 0)
						{
							if ((num9 & 0x40) == 0)
							{
								num8 += tl[(tl_index + num8) * 3 + 2];
								num8 += num2 & inflate_mask[num9];
								if ((num9 = tl[(tl_index + num8) * 3]) == 0)
								{
									num2 >>= tl[(tl_index + num8) * 3 + 1];
									num3 -= tl[(tl_index + num8) * 3 + 1];
									s.window[num4++] = (byte)tl[(tl_index + num8) * 3 + 2];
									num5--;
									break;
								}
								continue;
							}
							if ((num9 & 0x20) != 0)
							{
								num10 = z.avail_in - num;
								num10 = ((num3 >> 3 < num10) ? (num3 >> 3) : num10);
								num += num10;
								next_in_index -= num10;
								num3 -= num10 << 3;
								s.bitb = num2;
								s.bitk = num3;
								z.avail_in = num;
								z.total_in += next_in_index - z.next_in_index;
								z.next_in_index = next_in_index;
								s.write = num4;
								return 1;
							}
							z.msg = "invalid literal/length code";
							num10 = z.avail_in - num;
							num10 = ((num3 >> 3 < num10) ? (num3 >> 3) : num10);
							num += num10;
							next_in_index -= num10;
							num3 -= num10 << 3;
							s.bitb = num2;
							s.bitk = num3;
							z.avail_in = num;
							z.total_in += next_in_index - z.next_in_index;
							z.next_in_index = next_in_index;
							s.write = num4;
							return -3;
						}
						num9 &= 0xF;
						num10 = tl[(tl_index + num8) * 3 + 2] + (num2 & inflate_mask[num9]);
						num2 >>= num9;
						for (num3 -= num9; num3 < 15; num3 += 8)
						{
							num--;
							num2 |= (z.next_in[next_in_index++] & 0xFF) << num3;
						}
						num8 = num2 & num7;
						num9 = td[(td_index + num8) * 3];
						while (true)
						{
							num2 >>= td[(td_index + num8) * 3 + 1];
							num3 -= td[(td_index + num8) * 3 + 1];
							if ((num9 & 0x10) != 0)
							{
								break;
							}
							if ((num9 & 0x40) == 0)
							{
								num8 += td[(td_index + num8) * 3 + 2];
								num8 += num2 & inflate_mask[num9];
								num9 = td[(td_index + num8) * 3];
								continue;
							}
							z.msg = "invalid distance code";
							num10 = z.avail_in - num;
							num10 = ((num3 >> 3 < num10) ? (num3 >> 3) : num10);
							num += num10;
							next_in_index -= num10;
							num3 -= num10 << 3;
							s.bitb = num2;
							s.bitk = num3;
							z.avail_in = num;
							z.total_in += next_in_index - z.next_in_index;
							z.next_in_index = next_in_index;
							s.write = num4;
							return -3;
						}
						for (num9 &= 0xF; num3 < num9; num3 += 8)
						{
							num--;
							num2 |= (z.next_in[next_in_index++] & 0xFF) << num3;
						}
						int num11 = td[(td_index + num8) * 3 + 2] + (num2 & inflate_mask[num9]);
						num2 >>= num9;
						num3 -= num9;
						num5 -= num10;
						int num12;
						if (num4 >= num11)
						{
							num12 = num4 - num11;
							if (num4 - num12 > 0 && 2 > num4 - num12)
							{
								s.window[num4++] = s.window[num12++];
								num10--;
								s.window[num4++] = s.window[num12++];
								num10--;
							}
							else
							{
								Array.Copy(s.window, num12, s.window, num4, 2);
								num4 += 2;
								num12 += 2;
								num10 -= 2;
							}
						}
						else
						{
							num12 = num4 - num11;
							do
							{
								num12 += s.end;
							}
							while (num12 < 0);
							num9 = s.end - num12;
							if (num10 > num9)
							{
								num10 -= num9;
								if (num4 - num12 > 0 && num9 > num4 - num12)
								{
									do
									{
										s.window[num4++] = s.window[num12++];
									}
									while (--num9 != 0);
								}
								else
								{
									Array.Copy(s.window, num12, s.window, num4, num9);
									num4 += num9;
									num12 += num9;
								}
								num12 = 0;
							}
						}
						if (num4 - num12 > 0 && num10 > num4 - num12)
						{
							do
							{
								s.window[num4++] = s.window[num12++];
							}
							while (--num10 != 0);
						}
						else
						{
							Array.Copy(s.window, num12, s.window, num4, num10);
							num4 += num10;
							num12 += num10;
						}
						break;
					}
				}
				if (num5 < 258 || num < 10)
				{
					break;
				}
			}
			num10 = z.avail_in - num;
			num10 = ((num3 >> 3 < num10) ? (num3 >> 3) : num10);
			num += num10;
			next_in_index -= num10;
			num3 -= num10 << 3;
			s.bitb = num2;
			s.bitk = num3;
			z.avail_in = num;
			z.total_in += next_in_index - z.next_in_index;
			z.next_in_index = next_in_index;
			s.write = num4;
			return 0;
		}
	}
}
