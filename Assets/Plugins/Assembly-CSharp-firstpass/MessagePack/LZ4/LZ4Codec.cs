using System;
using System.Diagnostics;

namespace MessagePack.LZ4
{
	public static class LZ4Codec
	{
		internal static class HashTablePool
		{
			[ThreadStatic]
			private static ushort[] ushortPool;

			[ThreadStatic]
			private static uint[] uintPool;

			[ThreadStatic]
			private static int[] intPool;

			public static ushort[] GetUShortHashTablePool()
			{
				if (ushortPool == null)
				{
					ushortPool = new ushort[2048];
				}
				else
				{
					Array.Clear(ushortPool, 0, ushortPool.Length);
				}
				return ushortPool;
			}

			public static uint[] GetUIntHashTablePool()
			{
				if (uintPool == null)
				{
					uintPool = new uint[1024];
				}
				else
				{
					Array.Clear(uintPool, 0, uintPool.Length);
				}
				return uintPool;
			}

			public static int[] GetIntHashTablePool()
			{
				if (intPool == null)
				{
					intPool = new int[1024];
				}
				else
				{
					Array.Clear(intPool, 0, intPool.Length);
				}
				return intPool;
			}
		}

		private const int MEMORY_USAGE = 12;

		private const int NOTCOMPRESSIBLE_DETECTIONLEVEL = 6;

		private const int MINMATCH = 4;

		private const int SKIPSTRENGTH = 6;

		private const int COPYLENGTH = 8;

		private const int LASTLITERALS = 5;

		private const int MFLIMIT = 12;

		private const int MINLENGTH = 13;

		private const int MAXD_LOG = 16;

		private const int MAXD = 65536;

		private const int MAXD_MASK = 65535;

		private const int MAX_DISTANCE = 65535;

		private const int ML_BITS = 4;

		private const int ML_MASK = 15;

		private const int RUN_BITS = 4;

		private const int RUN_MASK = 15;

		private const int STEPSIZE_64 = 8;

		private const int STEPSIZE_32 = 4;

		private const int LZ4_64KLIMIT = 65547;

		private const int HASH_LOG = 10;

		private const int HASH_TABLESIZE = 1024;

		private const int HASH_ADJUST = 22;

		private const int HASH64K_LOG = 11;

		private const int HASH64K_TABLESIZE = 2048;

		private const int HASH64K_ADJUST = 21;

		private const int HASHHC_LOG = 15;

		private const int HASHHC_TABLESIZE = 32768;

		private const int HASHHC_ADJUST = 17;

		private static readonly int[] DECODER_TABLE_32 = new int[8] { 0, 3, 2, 3, 0, 0, 0, 0 };

		private static readonly int[] DECODER_TABLE_64 = new int[8] { 0, 0, 0, -1, 0, 1, 2, 3 };

		private static readonly int[] DEBRUIJN_TABLE_32 = new int[32]
		{
			0, 0, 3, 0, 3, 1, 3, 0, 3, 2,
			2, 1, 3, 2, 0, 1, 3, 3, 1, 2,
			2, 2, 2, 0, 3, 1, 2, 0, 1, 0,
			1, 1
		};

		private static readonly int[] DEBRUIJN_TABLE_64 = new int[64]
		{
			0, 0, 0, 0, 0, 1, 1, 2, 0, 3,
			1, 3, 1, 4, 2, 7, 0, 2, 3, 6,
			1, 5, 3, 5, 1, 3, 4, 4, 2, 5,
			6, 7, 7, 0, 1, 2, 3, 3, 4, 6,
			2, 6, 5, 5, 3, 4, 5, 6, 7, 1,
			2, 4, 6, 4, 4, 5, 7, 2, 6, 5,
			7, 6, 7, 7
		};

		private const int MAX_NB_ATTEMPTS = 256;

		private const int OPTIMAL_ML = 18;

		private const int BLOCK_COPY_LIMIT = 16;

		public static int Encode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			if (IntPtr.Size == 4)
			{
				return Encode32Safe(input, inputOffset, inputLength, output, outputOffset, outputLength);
			}
			return Encode64Safe(input, inputOffset, inputLength, output, outputOffset, outputLength);
		}

		public static int Decode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			if (IntPtr.Size == 4)
			{
				return Decode32Safe(input, inputOffset, inputLength, output, outputOffset, outputLength);
			}
			return Decode64Safe(input, inputOffset, inputLength, output, outputOffset, outputLength);
		}

		[Conditional("DEBUG")]
		private static void Assert(bool condition, string errorMessage)
		{
			if (!condition)
			{
				throw new ArgumentException(errorMessage);
			}
		}

		internal static void Poke2(byte[] buffer, int offset, ushort value)
		{
			buffer[offset] = (byte)value;
			buffer[offset + 1] = (byte)(value >> 8);
		}

		internal static ushort Peek2(byte[] buffer, int offset)
		{
			return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
		}

		internal static uint Peek4(byte[] buffer, int offset)
		{
			return (uint)(buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
		}

		private static uint Xor4(byte[] buffer, int offset1, int offset2)
		{
			uint num = (uint)(buffer[offset1] | (buffer[offset1 + 1] << 8) | (buffer[offset1 + 2] << 16) | (buffer[offset1 + 3] << 24));
			uint num2 = (uint)(buffer[offset2] | (buffer[offset2 + 1] << 8) | (buffer[offset2 + 2] << 16) | (buffer[offset2 + 3] << 24));
			return num ^ num2;
		}

		private static ulong Xor8(byte[] buffer, int offset1, int offset2)
		{
			ulong num = buffer[offset1] | ((ulong)buffer[offset1 + 1] << 8) | ((ulong)buffer[offset1 + 2] << 16) | ((ulong)buffer[offset1 + 3] << 24) | ((ulong)buffer[offset1 + 4] << 32) | ((ulong)buffer[offset1 + 5] << 40) | ((ulong)buffer[offset1 + 6] << 48) | ((ulong)buffer[offset1 + 7] << 56);
			ulong num2 = buffer[offset2] | ((ulong)buffer[offset2 + 1] << 8) | ((ulong)buffer[offset2 + 2] << 16) | ((ulong)buffer[offset2 + 3] << 24) | ((ulong)buffer[offset2 + 4] << 32) | ((ulong)buffer[offset2 + 5] << 40) | ((ulong)buffer[offset2 + 6] << 48) | ((ulong)buffer[offset2 + 7] << 56);
			return num ^ num2;
		}

		private static bool Equal2(byte[] buffer, int offset1, int offset2)
		{
			if (buffer[offset1] != buffer[offset2])
			{
				return false;
			}
			return buffer[offset1 + 1] == buffer[offset2 + 1];
		}

		private static bool Equal4(byte[] buffer, int offset1, int offset2)
		{
			if (buffer[offset1] != buffer[offset2])
			{
				return false;
			}
			if (buffer[offset1 + 1] != buffer[offset2 + 1])
			{
				return false;
			}
			if (buffer[offset1 + 2] != buffer[offset2 + 2])
			{
				return false;
			}
			return buffer[offset1 + 3] == buffer[offset2 + 3];
		}

		private static void Copy4(byte[] buf, int src, int dst)
		{
			buf[dst + 3] = buf[src + 3];
			buf[dst + 2] = buf[src + 2];
			buf[dst + 1] = buf[src + 1];
			buf[dst] = buf[src];
		}

		private static void Copy8(byte[] buf, int src, int dst)
		{
			buf[dst + 7] = buf[src + 7];
			buf[dst + 6] = buf[src + 6];
			buf[dst + 5] = buf[src + 5];
			buf[dst + 4] = buf[src + 4];
			buf[dst + 3] = buf[src + 3];
			buf[dst + 2] = buf[src + 2];
			buf[dst + 1] = buf[src + 1];
			buf[dst] = buf[src];
		}

		private static void BlockCopy(byte[] src, int src_0, byte[] dst, int dst_0, int len)
		{
			if (len >= 16)
			{
				Buffer.BlockCopy(src, src_0, dst, dst_0, len);
				return;
			}
			while (len >= 8)
			{
				dst[dst_0] = src[src_0];
				dst[dst_0 + 1] = src[src_0 + 1];
				dst[dst_0 + 2] = src[src_0 + 2];
				dst[dst_0 + 3] = src[src_0 + 3];
				dst[dst_0 + 4] = src[src_0 + 4];
				dst[dst_0 + 5] = src[src_0 + 5];
				dst[dst_0 + 6] = src[src_0 + 6];
				dst[dst_0 + 7] = src[src_0 + 7];
				len -= 8;
				src_0 += 8;
				dst_0 += 8;
			}
			while (len >= 4)
			{
				dst[dst_0] = src[src_0];
				dst[dst_0 + 1] = src[src_0 + 1];
				dst[dst_0 + 2] = src[src_0 + 2];
				dst[dst_0 + 3] = src[src_0 + 3];
				len -= 4;
				src_0 += 4;
				dst_0 += 4;
			}
			while (len-- > 0)
			{
				dst[dst_0++] = src[src_0++];
			}
		}

		private static int WildCopy(byte[] src, int src_0, byte[] dst, int dst_0, int dst_end)
		{
			int num = dst_end - dst_0;
			if (num >= 16)
			{
				Buffer.BlockCopy(src, src_0, dst, dst_0, num);
			}
			else
			{
				while (num >= 4)
				{
					dst[dst_0] = src[src_0];
					dst[dst_0 + 1] = src[src_0 + 1];
					dst[dst_0 + 2] = src[src_0 + 2];
					dst[dst_0 + 3] = src[src_0 + 3];
					num -= 4;
					src_0 += 4;
					dst_0 += 4;
				}
				while (num-- > 0)
				{
					dst[dst_0++] = src[src_0++];
				}
			}
			return num;
		}

		private static int SecureCopy(byte[] buffer, int src, int dst, int dst_end)
		{
			int num = dst - src;
			int num2 = dst_end - dst;
			int num3 = num2;
			if (num >= 16)
			{
				if (num >= num2)
				{
					Buffer.BlockCopy(buffer, src, buffer, dst, num2);
					return num2;
				}
				do
				{
					Buffer.BlockCopy(buffer, src, buffer, dst, num);
					src += num;
					dst += num;
					num3 -= num;
				}
				while (num3 >= num);
			}
			while (num3 >= 4)
			{
				buffer[dst] = buffer[src];
				buffer[dst + 1] = buffer[src + 1];
				buffer[dst + 2] = buffer[src + 2];
				buffer[dst + 3] = buffer[src + 3];
				dst += 4;
				src += 4;
				num3 -= 4;
			}
			while (num3-- > 0)
			{
				buffer[dst++] = buffer[src++];
			}
			return num2;
		}

		public static int Encode32Safe(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			CheckArguments(input, inputOffset, inputLength, output, outputOffset, outputLength);
			if (outputLength == 0)
			{
				return 0;
			}
			if (inputLength < 65547)
			{
				ushort[] uShortHashTablePool = HashTablePool.GetUShortHashTablePool();
				return LZ4_compress64kCtx_safe32(uShortHashTablePool, input, output, inputOffset, outputOffset, inputLength, outputLength);
			}
			int[] intHashTablePool = HashTablePool.GetIntHashTablePool();
			return LZ4_compressCtx_safe32(intHashTablePool, input, output, inputOffset, outputOffset, inputLength, outputLength);
		}

		public static int Encode64Safe(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			CheckArguments(input, inputOffset, inputLength, output, outputOffset, outputLength);
			if (outputLength == 0)
			{
				return 0;
			}
			if (inputLength < 65547)
			{
				ushort[] uShortHashTablePool = HashTablePool.GetUShortHashTablePool();
				return LZ4_compress64kCtx_safe64(uShortHashTablePool, input, output, inputOffset, outputOffset, inputLength, outputLength);
			}
			int[] intHashTablePool = HashTablePool.GetIntHashTablePool();
			return LZ4_compressCtx_safe64(intHashTablePool, input, output, inputOffset, outputOffset, inputLength, outputLength);
		}

		public static int Decode32Safe(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			CheckArguments(input, inputOffset, inputLength, output, outputOffset, outputLength);
			if (outputLength == 0)
			{
				return 0;
			}
			int num = LZ4_uncompress_safe32(input, output, inputOffset, outputOffset, outputLength);
			if (num != inputLength)
			{
				throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
			}
			return outputLength;
		}

		public static int Decode64Safe(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			CheckArguments(input, inputOffset, inputLength, output, outputOffset, outputLength);
			if (outputLength == 0)
			{
				return 0;
			}
			int num = LZ4_uncompress_safe64(input, output, inputOffset, outputOffset, outputLength);
			if (num != inputLength)
			{
				throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
			}
			return outputLength;
		}

		private static int LZ4_compressCtx_safe32(int[] hash_table, byte[] src, byte[] dst, int src_0, int dst_0, int src_len, int dst_maxlen)
		{
			int[] dEBRUIJN_TABLE_ = DEBRUIJN_TABLE_32;
			int num = src_0;
			int num2 = num;
			int num3 = num + src_len;
			int num4 = num3 - 12;
			int num5 = dst_0;
			int num6 = num5 + dst_maxlen;
			int num7 = num3 - 5;
			int num8 = num7 - 1;
			int num9 = num7 - 3;
			int num10 = num6 - 6;
			int num11 = num6 - 8;
			if (src_len >= 13)
			{
				hash_table[(uint)((int)Peek4(src, num) * -1640531535) >> 22] = num - src_0;
				num++;
				uint num12 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
				while (true)
				{
					int num13 = 67;
					int num14 = num;
					int num17;
					while (true)
					{
						uint num15 = num12;
						int num16 = num13++ >> 6;
						num = num14;
						num14 = num + num16;
						if (num14 > num4)
						{
							break;
						}
						num12 = (uint)((int)Peek4(src, num14) * -1640531535) >> 22;
						num17 = src_0 + hash_table[num15];
						hash_table[num15] = num - src_0;
						if (num17 < num - 65535 || !Equal4(src, num17, num))
						{
							continue;
						}
						goto IL_00f9;
					}
					break;
					IL_03ae:
					num2 = num++;
					num12 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
					continue;
					IL_00f9:
					while (num > num2 && num17 > src_0 && src[num - 1] == src[num17 - 1])
					{
						num--;
						num17--;
					}
					int num18 = num - num2;
					int num19 = num5++;
					if (num5 + num18 + (num18 >> 8) > num11)
					{
						return 0;
					}
					if (num18 >= 15)
					{
						int num20 = num18 - 15;
						dst[num19] = 240;
						if (num20 > 254)
						{
							do
							{
								dst[num5++] = byte.MaxValue;
								num20 -= 255;
							}
							while (num20 > 254);
							dst[num5++] = (byte)num20;
							BlockCopy(src, num2, dst, num5, num18);
							num5 += num18;
							goto IL_01e1;
						}
						dst[num5++] = (byte)num20;
					}
					else
					{
						dst[num19] = (byte)(num18 << 4);
					}
					if (num18 > 0)
					{
						int num21 = num5 + num18;
						WildCopy(src, num2, dst, num5, num21);
						num5 = num21;
					}
					goto IL_01e1;
					IL_01e1:
					while (true)
					{
						Poke2(dst, num5, (ushort)(num - num17));
						num5 += 2;
						num += 4;
						num17 += 4;
						num2 = num;
						while (true)
						{
							if (num < num9)
							{
								int num22 = (int)Xor4(src, num17, num);
								if (num22 == 0)
								{
									num += 4;
									num17 += 4;
									continue;
								}
								num += dEBRUIJN_TABLE_[(uint)((num22 & -num22) * 125613361) >> 27];
								break;
							}
							if (num < num8 && Equal2(src, num17, num))
							{
								num += 2;
								num17 += 2;
							}
							if (num < num7 && src[num17] == src[num])
							{
								num++;
							}
							break;
						}
						num18 = num - num2;
						if (num5 + (num18 >> 8) > num10)
						{
							return 0;
						}
						if (num18 >= 15)
						{
							dst[num19] += 15;
							for (num18 -= 15; num18 > 509; num18 -= 510)
							{
								dst[num5++] = byte.MaxValue;
								dst[num5++] = byte.MaxValue;
							}
							if (num18 > 254)
							{
								num18 -= 255;
								dst[num5++] = byte.MaxValue;
							}
							dst[num5++] = (byte)num18;
						}
						else
						{
							dst[num19] += (byte)num18;
						}
						if (num > num4)
						{
							break;
						}
						hash_table[(uint)((int)Peek4(src, num - 2) * -1640531535) >> 22] = num - 2 - src_0;
						uint num15 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
						num17 = src_0 + hash_table[num15];
						hash_table[num15] = num - src_0;
						if (num17 > num - 65536 && Equal4(src, num17, num))
						{
							num19 = num5++;
							dst[num19] = 0;
							continue;
						}
						goto IL_03ae;
					}
					num2 = num;
					break;
				}
			}
			int num23 = num3 - num2;
			if (num5 + num23 + 1 + (num23 + 255 - 15) / 255 > num6)
			{
				return 0;
			}
			if (num23 >= 15)
			{
				dst[num5++] = 240;
				for (num23 -= 15; num23 > 254; num23 -= 255)
				{
					dst[num5++] = byte.MaxValue;
				}
				dst[num5++] = (byte)num23;
			}
			else
			{
				dst[num5++] = (byte)(num23 << 4);
			}
			BlockCopy(src, num2, dst, num5, num3 - num2);
			num5 += num3 - num2;
			return num5 - dst_0;
		}

		private static int LZ4_compress64kCtx_safe32(ushort[] hash_table, byte[] src, byte[] dst, int src_0, int dst_0, int src_len, int dst_maxlen)
		{
			int[] dEBRUIJN_TABLE_ = DEBRUIJN_TABLE_32;
			int num = src_0;
			int num2 = num;
			int num3 = num;
			int num4 = num + src_len;
			int num5 = num4 - 12;
			int num6 = dst_0;
			int num7 = num6 + dst_maxlen;
			int num8 = num4 - 5;
			int num9 = num8 - 1;
			int num10 = num8 - 3;
			int num11 = num7 - 6;
			int num12 = num7 - 8;
			if (src_len >= 13)
			{
				num++;
				uint num13 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
				while (true)
				{
					int num14 = 67;
					int num15 = num;
					int num18;
					while (true)
					{
						uint num16 = num13;
						int num17 = num14++ >> 6;
						num = num15;
						num15 = num + num17;
						if (num15 > num5)
						{
							break;
						}
						num13 = (uint)((int)Peek4(src, num15) * -1640531535) >> 21;
						num18 = num3 + hash_table[num16];
						hash_table[num16] = (ushort)(num - num3);
						if (!Equal4(src, num18, num))
						{
							continue;
						}
						goto IL_00d8;
					}
					break;
					IL_037d:
					num2 = num++;
					num13 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
					continue;
					IL_00d8:
					while (num > num2 && num18 > src_0 && src[num - 1] == src[num18 - 1])
					{
						num--;
						num18--;
					}
					int num19 = num - num2;
					int num20 = num6++;
					if (num6 + num19 + (num19 >> 8) > num12)
					{
						return 0;
					}
					if (num19 >= 15)
					{
						int num21 = num19 - 15;
						dst[num20] = 240;
						if (num21 > 254)
						{
							do
							{
								dst[num6++] = byte.MaxValue;
								num21 -= 255;
							}
							while (num21 > 254);
							dst[num6++] = (byte)num21;
							BlockCopy(src, num2, dst, num6, num19);
							num6 += num19;
							goto IL_01bc;
						}
						dst[num6++] = (byte)num21;
					}
					else
					{
						dst[num20] = (byte)(num19 << 4);
					}
					if (num19 > 0)
					{
						int num22 = num6 + num19;
						WildCopy(src, num2, dst, num6, num22);
						num6 = num22;
					}
					goto IL_01bc;
					IL_01bc:
					while (true)
					{
						Poke2(dst, num6, (ushort)(num - num18));
						num6 += 2;
						num += 4;
						num18 += 4;
						num2 = num;
						while (true)
						{
							if (num < num10)
							{
								int num23 = (int)Xor4(src, num18, num);
								if (num23 == 0)
								{
									num += 4;
									num18 += 4;
									continue;
								}
								num += dEBRUIJN_TABLE_[(uint)((num23 & -num23) * 125613361) >> 27];
								break;
							}
							if (num < num9 && Equal2(src, num18, num))
							{
								num += 2;
								num18 += 2;
							}
							if (num < num8 && src[num18] == src[num])
							{
								num++;
							}
							break;
						}
						int num21 = num - num2;
						if (num6 + (num21 >> 8) > num11)
						{
							return 0;
						}
						if (num21 >= 15)
						{
							dst[num20] += 15;
							for (num21 -= 15; num21 > 509; num21 -= 510)
							{
								dst[num6++] = byte.MaxValue;
								dst[num6++] = byte.MaxValue;
							}
							if (num21 > 254)
							{
								num21 -= 255;
								dst[num6++] = byte.MaxValue;
							}
							dst[num6++] = (byte)num21;
						}
						else
						{
							dst[num20] += (byte)num21;
						}
						if (num > num5)
						{
							break;
						}
						hash_table[(uint)((int)Peek4(src, num - 2) * -1640531535) >> 21] = (ushort)(num - 2 - num3);
						uint num16 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
						num18 = num3 + hash_table[num16];
						hash_table[num16] = (ushort)(num - num3);
						if (Equal4(src, num18, num))
						{
							num20 = num6++;
							dst[num20] = 0;
							continue;
						}
						goto IL_037d;
					}
					num2 = num;
					break;
				}
			}
			int num24 = num4 - num2;
			if (num6 + num24 + 1 + (num24 - 15 + 255) / 255 > num7)
			{
				return 0;
			}
			if (num24 >= 15)
			{
				dst[num6++] = 240;
				for (num24 -= 15; num24 > 254; num24 -= 255)
				{
					dst[num6++] = byte.MaxValue;
				}
				dst[num6++] = (byte)num24;
			}
			else
			{
				dst[num6++] = (byte)(num24 << 4);
			}
			BlockCopy(src, num2, dst, num6, num4 - num2);
			num6 += num4 - num2;
			return num6 - dst_0;
		}

		private static int LZ4_uncompress_safe32(byte[] src, byte[] dst, int src_0, int dst_0, int dst_len)
		{
			int[] dECODER_TABLE_ = DECODER_TABLE_32;
			int num = src_0;
			int num2 = dst_0;
			int num3 = num2 + dst_len;
			int num4 = num3 - 5;
			int num5 = num3 - 8;
			int num6 = num3 - 8;
			while (true)
			{
				byte b = src[num++];
				int num7;
				if ((num7 = b >> 4) == 15)
				{
					int num8;
					while ((num8 = src[num++]) == 255)
					{
						num7 += 255;
					}
					num7 += num8;
				}
				int num9 = num2 + num7;
				if (num9 > num5)
				{
					if (num9 != num3)
					{
						break;
					}
					BlockCopy(src, num, dst, num2, num7);
					num += num7;
					return num - src_0;
				}
				if (num2 < num9)
				{
					int num10 = WildCopy(src, num, dst, num2, num9);
					num += num10;
					num2 += num10;
				}
				num -= num2 - num9;
				num2 = num9;
				int num11 = num9 - Peek2(src, num);
				num += 2;
				if (num11 < dst_0)
				{
					break;
				}
				if ((num7 = b & 0xF) == 15)
				{
					while (src[num] == byte.MaxValue)
					{
						num++;
						num7 += 255;
					}
					num7 += src[num++];
				}
				if (num2 - num11 < 4)
				{
					dst[num2] = dst[num11];
					dst[num2 + 1] = dst[num11 + 1];
					dst[num2 + 2] = dst[num11 + 2];
					dst[num2 + 3] = dst[num11 + 3];
					num2 += 4;
					num11 += 4;
					num11 -= dECODER_TABLE_[num2 - num11];
					Copy4(dst, num11, num2);
					num2 = num2;
					num11 = num11;
				}
				else
				{
					Copy4(dst, num11, num2);
					num2 += 4;
					num11 += 4;
				}
				num9 = num2 + num7;
				if (num9 > num6)
				{
					if (num9 > num4)
					{
						break;
					}
					if (num2 < num5)
					{
						int num10 = SecureCopy(dst, num11, num2, num5);
						num11 += num10;
						num2 += num10;
					}
					while (num2 < num9)
					{
						dst[num2++] = dst[num11++];
					}
					num2 = num9;
				}
				else
				{
					if (num2 < num9)
					{
						SecureCopy(dst, num11, num2, num9);
					}
					num2 = num9;
				}
			}
			return -(num - src_0);
		}

		private static int LZ4_compressCtx_safe64(int[] hash_table, byte[] src, byte[] dst, int src_0, int dst_0, int src_len, int dst_maxlen)
		{
			int[] dEBRUIJN_TABLE_ = DEBRUIJN_TABLE_64;
			int num = src_0;
			int num2 = num;
			int num3 = num + src_len;
			int num4 = num3 - 12;
			int num5 = dst_0;
			int num6 = num5 + dst_maxlen;
			int num7 = num3 - 5;
			int num8 = num7 - 1;
			int num9 = num7 - 3;
			int num10 = num7 - 7;
			int num11 = num6 - 6;
			int num12 = num6 - 8;
			if (src_len >= 13)
			{
				hash_table[(uint)((int)Peek4(src, num) * -1640531535) >> 22] = num - src_0;
				num++;
				uint num13 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
				while (true)
				{
					int num14 = 67;
					int num15 = num;
					int num18;
					while (true)
					{
						uint num16 = num13;
						int num17 = num14++ >> 6;
						num = num15;
						num15 = num + num17;
						if (num15 > num4)
						{
							break;
						}
						num13 = (uint)((int)Peek4(src, num15) * -1640531535) >> 22;
						num18 = src_0 + hash_table[num16];
						hash_table[num16] = num - src_0;
						if (num18 < num - 65535 || !Equal4(src, num18, num))
						{
							continue;
						}
						goto IL_00ff;
					}
					break;
					IL_03da:
					num2 = num++;
					num13 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
					continue;
					IL_00ff:
					while (num > num2 && num18 > src_0 && src[num - 1] == src[num18 - 1])
					{
						num--;
						num18--;
					}
					int num19 = num - num2;
					int num20 = num5++;
					if (num5 + num19 + (num19 >> 8) > num12)
					{
						return 0;
					}
					if (num19 >= 15)
					{
						int num21 = num19 - 15;
						dst[num20] = 240;
						if (num21 > 254)
						{
							do
							{
								dst[num5++] = byte.MaxValue;
								num21 -= 255;
							}
							while (num21 > 254);
							dst[num5++] = (byte)num21;
							BlockCopy(src, num2, dst, num5, num19);
							num5 += num19;
							goto IL_01e7;
						}
						dst[num5++] = (byte)num21;
					}
					else
					{
						dst[num20] = (byte)(num19 << 4);
					}
					if (num19 > 0)
					{
						int num22 = num5 + num19;
						WildCopy(src, num2, dst, num5, num22);
						num5 = num22;
					}
					goto IL_01e7;
					IL_01e7:
					while (true)
					{
						Poke2(dst, num5, (ushort)(num - num18));
						num5 += 2;
						num += 4;
						num18 += 4;
						num2 = num;
						while (true)
						{
							if (num < num10)
							{
								long num23 = (long)Xor8(src, num18, num);
								if (num23 == 0)
								{
									num += 8;
									num18 += 8;
									continue;
								}
								num += dEBRUIJN_TABLE_[(ulong)((num23 & -num23) * 151050438428048703L) >> 58];
								break;
							}
							if (num < num9 && Equal4(src, num18, num))
							{
								num += 4;
								num18 += 4;
							}
							if (num < num8 && Equal2(src, num18, num))
							{
								num += 2;
								num18 += 2;
							}
							if (num < num7 && src[num18] == src[num])
							{
								num++;
							}
							break;
						}
						num19 = num - num2;
						if (num5 + (num19 >> 8) > num11)
						{
							return 0;
						}
						if (num19 >= 15)
						{
							dst[num20] += 15;
							for (num19 -= 15; num19 > 509; num19 -= 510)
							{
								dst[num5++] = byte.MaxValue;
								dst[num5++] = byte.MaxValue;
							}
							if (num19 > 254)
							{
								num19 -= 255;
								dst[num5++] = byte.MaxValue;
							}
							dst[num5++] = (byte)num19;
						}
						else
						{
							dst[num20] += (byte)num19;
						}
						if (num > num4)
						{
							break;
						}
						hash_table[(uint)((int)Peek4(src, num - 2) * -1640531535) >> 22] = num - 2 - src_0;
						uint num16 = (uint)((int)Peek4(src, num) * -1640531535) >> 22;
						num18 = src_0 + hash_table[num16];
						hash_table[num16] = num - src_0;
						if (num18 > num - 65536 && Equal4(src, num18, num))
						{
							num20 = num5++;
							dst[num20] = 0;
							continue;
						}
						goto IL_03da;
					}
					num2 = num;
					break;
				}
			}
			int num24 = num3 - num2;
			if (num5 + num24 + 1 + (num24 + 255 - 15) / 255 > num6)
			{
				return 0;
			}
			if (num24 >= 15)
			{
				dst[num5++] = 240;
				for (num24 -= 15; num24 > 254; num24 -= 255)
				{
					dst[num5++] = byte.MaxValue;
				}
				dst[num5++] = (byte)num24;
			}
			else
			{
				dst[num5++] = (byte)(num24 << 4);
			}
			BlockCopy(src, num2, dst, num5, num3 - num2);
			num5 += num3 - num2;
			return num5 - dst_0;
		}

		private static int LZ4_compress64kCtx_safe64(ushort[] hash_table, byte[] src, byte[] dst, int src_0, int dst_0, int src_len, int dst_maxlen)
		{
			int[] dEBRUIJN_TABLE_ = DEBRUIJN_TABLE_64;
			int num = src_0;
			int num2 = num;
			int num3 = num;
			int num4 = num + src_len;
			int num5 = num4 - 12;
			int num6 = dst_0;
			int num7 = num6 + dst_maxlen;
			int num8 = num4 - 5;
			int num9 = num8 - 1;
			int num10 = num8 - 3;
			int num11 = num8 - 7;
			int num12 = num7 - 6;
			int num13 = num7 - 8;
			if (src_len >= 13)
			{
				num++;
				uint num14 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
				while (true)
				{
					int num15 = 67;
					int num16 = num;
					int num19;
					while (true)
					{
						uint num17 = num14;
						int num18 = num15++ >> 6;
						num = num16;
						num16 = num + num18;
						if (num16 > num5)
						{
							break;
						}
						num14 = (uint)((int)Peek4(src, num16) * -1640531535) >> 21;
						num19 = num3 + hash_table[num17];
						hash_table[num17] = (ushort)(num - num3);
						if (!Equal4(src, num19, num))
						{
							continue;
						}
						goto IL_00de;
					}
					break;
					IL_03a9:
					num2 = num++;
					num14 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
					continue;
					IL_00de:
					while (num > num2 && num19 > src_0 && src[num - 1] == src[num19 - 1])
					{
						num--;
						num19--;
					}
					int num20 = num - num2;
					int num21 = num6++;
					if (num6 + num20 + (num20 >> 8) > num13)
					{
						return 0;
					}
					if (num20 >= 15)
					{
						int num22 = num20 - 15;
						dst[num21] = 240;
						if (num22 > 254)
						{
							do
							{
								dst[num6++] = byte.MaxValue;
								num22 -= 255;
							}
							while (num22 > 254);
							dst[num6++] = (byte)num22;
							BlockCopy(src, num2, dst, num6, num20);
							num6 += num20;
							goto IL_01c2;
						}
						dst[num6++] = (byte)num22;
					}
					else
					{
						dst[num21] = (byte)(num20 << 4);
					}
					if (num20 > 0)
					{
						int num23 = num6 + num20;
						WildCopy(src, num2, dst, num6, num23);
						num6 = num23;
					}
					goto IL_01c2;
					IL_01c2:
					while (true)
					{
						Poke2(dst, num6, (ushort)(num - num19));
						num6 += 2;
						num += 4;
						num19 += 4;
						num2 = num;
						while (true)
						{
							if (num < num11)
							{
								long num24 = (long)Xor8(src, num19, num);
								if (num24 == 0)
								{
									num += 8;
									num19 += 8;
									continue;
								}
								num += dEBRUIJN_TABLE_[(ulong)((num24 & -num24) * 151050438428048703L) >> 58];
								break;
							}
							if (num < num10 && Equal4(src, num19, num))
							{
								num += 4;
								num19 += 4;
							}
							if (num < num9 && Equal2(src, num19, num))
							{
								num += 2;
								num19 += 2;
							}
							if (num < num8 && src[num19] == src[num])
							{
								num++;
							}
							break;
						}
						int num22 = num - num2;
						if (num6 + (num22 >> 8) > num12)
						{
							return 0;
						}
						if (num22 >= 15)
						{
							dst[num21] += 15;
							for (num22 -= 15; num22 > 509; num22 -= 510)
							{
								dst[num6++] = byte.MaxValue;
								dst[num6++] = byte.MaxValue;
							}
							if (num22 > 254)
							{
								num22 -= 255;
								dst[num6++] = byte.MaxValue;
							}
							dst[num6++] = (byte)num22;
						}
						else
						{
							dst[num21] += (byte)num22;
						}
						if (num > num5)
						{
							break;
						}
						hash_table[(uint)((int)Peek4(src, num - 2) * -1640531535) >> 21] = (ushort)(num - 2 - num3);
						uint num17 = (uint)((int)Peek4(src, num) * -1640531535) >> 21;
						num19 = num3 + hash_table[num17];
						hash_table[num17] = (ushort)(num - num3);
						if (Equal4(src, num19, num))
						{
							num21 = num6++;
							dst[num21] = 0;
							continue;
						}
						goto IL_03a9;
					}
					num2 = num;
					break;
				}
			}
			int num25 = num4 - num2;
			if (num6 + num25 + 1 + (num25 - 15 + 255) / 255 > num7)
			{
				return 0;
			}
			if (num25 >= 15)
			{
				dst[num6++] = 240;
				for (num25 -= 15; num25 > 254; num25 -= 255)
				{
					dst[num6++] = byte.MaxValue;
				}
				dst[num6++] = (byte)num25;
			}
			else
			{
				dst[num6++] = (byte)(num25 << 4);
			}
			BlockCopy(src, num2, dst, num6, num4 - num2);
			num6 += num4 - num2;
			return num6 - dst_0;
		}

		private static int LZ4_uncompress_safe64(byte[] src, byte[] dst, int src_0, int dst_0, int dst_len)
		{
			int[] dECODER_TABLE_ = DECODER_TABLE_32;
			int[] dECODER_TABLE_2 = DECODER_TABLE_64;
			int num = src_0;
			int num2 = dst_0;
			int num3 = num2 + dst_len;
			int num4 = num3 - 5;
			int num5 = num3 - 8;
			int num6 = num3 - 8 - 4;
			while (true)
			{
				uint num7 = src[num++];
				int num8;
				if ((num8 = (byte)(num7 >> 4)) == 15)
				{
					int num9;
					while ((num9 = src[num++]) == 255)
					{
						num8 += 255;
					}
					num8 += num9;
				}
				int num10 = num2 + num8;
				if (num10 > num5)
				{
					if (num10 != num3)
					{
						break;
					}
					BlockCopy(src, num, dst, num2, num8);
					num += num8;
					return num - src_0;
				}
				if (num2 < num10)
				{
					int num11 = WildCopy(src, num, dst, num2, num10);
					num += num11;
					num2 += num11;
				}
				num -= num2 - num10;
				num2 = num10;
				int num12 = num10 - Peek2(src, num);
				num += 2;
				if (num12 < dst_0)
				{
					break;
				}
				if ((num8 = (byte)(num7 & 0xF)) == 15)
				{
					while (src[num] == byte.MaxValue)
					{
						num++;
						num8 += 255;
					}
					num8 += src[num++];
				}
				if (num2 - num12 < 8)
				{
					int num13 = dECODER_TABLE_2[num2 - num12];
					dst[num2] = dst[num12];
					dst[num2 + 1] = dst[num12 + 1];
					dst[num2 + 2] = dst[num12 + 2];
					dst[num2 + 3] = dst[num12 + 3];
					num2 += 4;
					num12 += 4;
					num12 -= dECODER_TABLE_[num2 - num12];
					Copy4(dst, num12, num2);
					num2 += 4;
					num12 -= num13;
				}
				else
				{
					Copy8(dst, num12, num2);
					num2 += 8;
					num12 += 8;
				}
				num10 = num2 + num8 - 4;
				if (num10 > num6)
				{
					if (num10 > num4)
					{
						break;
					}
					if (num2 < num5)
					{
						int num11 = SecureCopy(dst, num12, num2, num5);
						num12 += num11;
						num2 += num11;
					}
					while (num2 < num10)
					{
						dst[num2++] = dst[num12++];
					}
					num2 = num10;
				}
				else
				{
					if (num2 < num10)
					{
						SecureCopy(dst, num12, num2, num10);
					}
					num2 = num10;
				}
			}
			return -(num - src_0);
		}

		public static int MaximumOutputLength(int inputLength)
		{
			return inputLength + inputLength / 255 + 16;
		}

		internal static void CheckArguments(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength)
		{
			if (inputLength == 0)
			{
				outputLength = 0;
				return;
			}
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if ((uint)inputOffset > (uint)input.Length)
			{
				throw new ArgumentOutOfRangeException("inputOffset");
			}
			if ((uint)inputLength > (uint)(input.Length - inputOffset))
			{
				throw new ArgumentOutOfRangeException("inputLength");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if ((uint)outputOffset > (uint)output.Length)
			{
				throw new ArgumentOutOfRangeException("outputOffset");
			}
			if ((uint)outputLength <= (uint)(output.Length - outputOffset))
			{
				return;
			}
			throw new ArgumentOutOfRangeException("outputLength");
		}
	}
}
