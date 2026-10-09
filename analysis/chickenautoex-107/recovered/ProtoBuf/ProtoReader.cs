using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	public sealed class ProtoReader : IDisposable
	{
		private const long Int64Msb = long.MinValue;

		private const int Int32Msb = int.MinValue;

		private Stream source;

		private byte[] ioBuffer;

		private TypeModel model;

		private int fieldNumber;

		private WireType wireType = WireType.None;

		private int dataRemaining;

		private readonly bool isFixedLength;

		private bool internStrings = true;

		private readonly SerializationContext context;

		private int ioIndex;

		private int position;

		private int available;

		private Dictionary<string, string> stringInterner;

		private static readonly UTF8Encoding encoding = new UTF8Encoding();

		private int depth;

		private int blockEnd = int.MaxValue;

		private static readonly byte[] EmptyBlob = new byte[0];

		private readonly NetObjectCache netCache = new NetObjectCache();

		private uint trapCount = 1u;

		public int FieldNumber => fieldNumber;

		public WireType WireType => wireType;

		public bool InternStrings
		{
			get
			{
				return internStrings;
			}
			set
			{
				internStrings = value;
			}
		}

		public SerializationContext Context => context;

		public int Position => position;

		public TypeModel Model => model;

		internal NetObjectCache NetCache => netCache;

		public ProtoReader(Stream source, TypeModel model, SerializationContext context)
			: this(source, model, context, -1)
		{
		}

		public ProtoReader(Stream source, TypeModel model, SerializationContext context, int length)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.CanRead)
			{
				throw new ArgumentException("Cannot read from stream", "source");
			}
			this.source = source;
			ioBuffer = BufferPool.GetBuffer();
			this.model = model;
			isFixedLength = length >= 0;
			dataRemaining = (isFixedLength ? length : 0);
			if (context == null)
			{
				context = SerializationContext.Default;
			}
			else
			{
				context.Freeze();
			}
			this.context = context;
		}

		public void Dispose()
		{
			source = null;
			model = null;
			BufferPool.ReleaseBufferToPool(ref ioBuffer);
		}

		internal int TryReadUInt32VariantWithoutMoving(bool trimNegative, out uint value)
		{
			if (available < 10)
			{
				Ensure(10, strict: false);
			}
			if (available == 0)
			{
				value = 0u;
				return 0;
			}
			int num = ioIndex;
			value = ioBuffer[num++];
			if ((value & 0x80) == 0)
			{
				return 1;
			}
			value &= 127u;
			if (available == 1)
			{
				throw EoF(this);
			}
			uint num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 7;
			if ((num2 & 0x80) == 0)
			{
				return 2;
			}
			if (available == 2)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 14;
			if ((num2 & 0x80) == 0)
			{
				return 3;
			}
			if (available == 3)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 21;
			if ((num2 & 0x80) == 0)
			{
				return 4;
			}
			if (available == 4)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num];
			value |= num2 << 28;
			if ((num2 & 0xF0) == 0)
			{
				return 5;
			}
			if (trimNegative && (num2 & 0xF0) == 240 && available >= 10 && ioBuffer[++num] == byte.MaxValue && ioBuffer[++num] == byte.MaxValue && ioBuffer[++num] == byte.MaxValue && ioBuffer[++num] == byte.MaxValue && ioBuffer[num + 1] == 1)
			{
				return 10;
			}
			throw AddErrorData(new OverflowException(), this);
		}

		private uint ReadUInt32Variant(bool trimNegative)
		{
			uint value;
			int num = TryReadUInt32VariantWithoutMoving(trimNegative, out value);
			if (num > 0)
			{
				ioIndex += num;
				available -= num;
				position += num;
				return value;
			}
			throw EoF(this);
		}

		private bool TryReadUInt32Variant(out uint value)
		{
			int num = TryReadUInt32VariantWithoutMoving(trimNegative: false, out value);
			if (num > 0)
			{
				ioIndex += num;
				available -= num;
				position += num;
				return true;
			}
			return false;
		}

		public uint ReadUInt32()
		{
			switch (wireType)
			{
			case WireType.Variant:
				return ReadUInt32Variant(trimNegative: false);
			case WireType.Fixed64:
				return checked((uint)ReadUInt64());
			default:
				throw CreateWireTypeException();
			case WireType.Fixed32:
				if (available < 4)
				{
					Ensure(4, strict: true);
				}
				position += 4;
				available -= 4;
				return (uint)(ioBuffer[ioIndex++] | (ioBuffer[ioIndex++] << 8) | (ioBuffer[ioIndex++] << 16) | (ioBuffer[ioIndex++] << 24));
			}
		}

		internal void Ensure(int count, bool strict)
		{
			if (count > ioBuffer.Length)
			{
				BufferPool.ResizeAndFlushLeft(ref ioBuffer, count, ioIndex, available);
				ioIndex = 0;
			}
			else if (ioIndex + count >= ioBuffer.Length)
			{
				Helpers.BlockCopy(ioBuffer, ioIndex, ioBuffer, 0, available);
				ioIndex = 0;
			}
			count -= available;
			int num = ioIndex + available;
			int num2 = ioBuffer.Length - num;
			if (isFixedLength && dataRemaining < num2)
			{
				num2 = dataRemaining;
			}
			int num3;
			while (count > 0 && num2 > 0 && (num3 = source.Read(ioBuffer, num, num2)) > 0)
			{
				available += num3;
				count -= num3;
				num2 -= num3;
				num += num3;
				if (isFixedLength)
				{
					dataRemaining -= num3;
				}
			}
			if (strict && count > 0)
			{
				throw EoF(this);
			}
		}

		public short ReadInt16()
		{
			return checked((short)ReadInt32());
		}

		public ushort ReadUInt16()
		{
			return checked((ushort)ReadUInt32());
		}

		public byte ReadByte()
		{
			return checked((byte)ReadUInt32());
		}

		public sbyte ReadSByte()
		{
			return checked((sbyte)ReadInt32());
		}

		public int ReadInt32()
		{
			switch (wireType)
			{
			case WireType.Variant:
				return (int)ReadUInt32Variant(trimNegative: true);
			case WireType.Fixed64:
				return checked((int)ReadInt64());
			case WireType.Fixed32:
				if (available < 4)
				{
					Ensure(4, strict: true);
				}
				position += 4;
				available -= 4;
				return ioBuffer[ioIndex++] | (ioBuffer[ioIndex++] << 8) | (ioBuffer[ioIndex++] << 16) | (ioBuffer[ioIndex++] << 24);
			default:
				throw CreateWireTypeException();
			case WireType.SignedVariant:
				return Zag(ReadUInt32Variant(trimNegative: true));
			}
		}

		private static int Zag(uint ziggedValue)
		{
			return (int)((0L - (long)(ziggedValue & 1)) ^ (uint)(((int)ziggedValue >> 1) & 0x7FFFFFFF));
		}

		private static long Zag(ulong ziggedValue)
		{
			return (long)((ziggedValue & 1) ^ ((ziggedValue >> 1) & 0x7FFFFFFFFFFFFFFFL));
		}

		public long ReadInt64()
		{
			switch (wireType)
			{
			case WireType.Variant:
				return (long)ReadUInt64Variant();
			case WireType.Fixed64:
				if (available < 8)
				{
					Ensure(8, strict: true);
				}
				position += 8;
				available -= 8;
				return (long)(ioBuffer[ioIndex++] | ((ulong)ioBuffer[ioIndex++] << 8) | ((ulong)ioBuffer[ioIndex++] << 16) | ((ulong)ioBuffer[ioIndex++] << 24) | ((ulong)ioBuffer[ioIndex++] << 32) | ((ulong)ioBuffer[ioIndex++] << 40) | ((ulong)ioBuffer[ioIndex++] << 48) | ((ulong)ioBuffer[ioIndex++] << 56));
			case WireType.Fixed32:
				return ReadInt32();
			default:
				throw CreateWireTypeException();
			case WireType.SignedVariant:
				return Zag(ReadUInt64Variant());
			}
		}

		private int TryReadUInt64VariantWithoutMoving(out ulong value)
		{
			if (available < 10)
			{
				Ensure(10, strict: false);
			}
			if (available == 0)
			{
				value = 0uL;
				return 0;
			}
			int num = ioIndex;
			value = ioBuffer[num++];
			if ((value & 0x80) == 0L)
			{
				return 1;
			}
			value &= 127uL;
			if (available == 1)
			{
				throw EoF(this);
			}
			ulong num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 7;
			if ((num2 & 0x80) == 0L)
			{
				return 2;
			}
			if (available == 2)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 14;
			if ((num2 & 0x80) == 0L)
			{
				return 3;
			}
			if (available == 3)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 21;
			if ((num2 & 0x80) == 0L)
			{
				return 4;
			}
			if (available == 4)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 28;
			if ((num2 & 0x80) == 0L)
			{
				return 5;
			}
			if (available == 5)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 35;
			if ((num2 & 0x80) == 0L)
			{
				return 6;
			}
			if (available == 6)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 42;
			if ((num2 & 0x80) == 0L)
			{
				return 7;
			}
			if (available == 7)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 49;
			if ((num2 & 0x80) == 0L)
			{
				return 8;
			}
			if (available == 8)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num++];
			value |= (num2 & 0x7F) << 56;
			if ((num2 & 0x80) == 0L)
			{
				return 9;
			}
			if (available == 9)
			{
				throw EoF(this);
			}
			num2 = ioBuffer[num];
			value |= num2 << 63;
			if ((num2 & 0xFFFFFFFFFFFFFFFEuL) != 0L)
			{
				throw AddErrorData(new OverflowException(), this);
			}
			return 10;
		}

		private ulong ReadUInt64Variant()
		{
			ulong value;
			int num = TryReadUInt64VariantWithoutMoving(out value);
			if (num > 0)
			{
				ioIndex += num;
				available -= num;
				position += num;
				return value;
			}
			throw EoF(this);
		}

		private string Intern(string value)
		{
			if (value == null)
			{
				return null;
			}
			if (value.Length == 0)
			{
				return "";
			}
			string value2;
			if (stringInterner == null)
			{
				stringInterner = new Dictionary<string, string>();
				stringInterner.Add(value, value);
			}
			else if (stringInterner.TryGetValue(value, out value2))
			{
				value = value2;
			}
			else
			{
				stringInterner.Add(value, value);
			}
			return value;
		}

		public string ReadString()
		{
			if (wireType != WireType.String)
			{
				throw CreateWireTypeException();
			}
			int num = (int)ReadUInt32Variant(trimNegative: false);
			if (num == 0)
			{
				return "";
			}
			if (available < num)
			{
				Ensure(num, strict: true);
			}
			string text = encoding.GetString(ioBuffer, ioIndex, num);
			if (internStrings)
			{
				text = Intern(text);
			}
			available -= num;
			position += num;
			ioIndex += num;
			return text;
		}

		public void ThrowEnumException(Type type, int value)
		{
			string text = ((type == null) ? "<null>" : type.FullName);
			throw AddErrorData(new ProtoException("No " + text + " enum is mapped to the wire-value " + value), this);
		}

		private Exception CreateWireTypeException()
		{
			return CreateException("Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354");
		}

		private Exception CreateException(string message)
		{
			return AddErrorData(new ProtoException(message), this);
		}

		public unsafe double ReadDouble()
		{
			switch (wireType)
			{
			case WireType.Fixed64:
			{
				long num = ReadInt64();
				return *(double*)(&num);
			}
			case WireType.Fixed32:
				return ReadSingle();
			default:
				throw CreateWireTypeException();
			}
		}

		public static object ReadObject(object value, int key, ProtoReader reader)
		{
			return ReadTypedObject(value, key, reader, null);
		}

		internal static object ReadTypedObject(object value, int key, ProtoReader reader, Type type)
		{
			if (reader.model == null)
			{
				throw AddErrorData(new InvalidOperationException("Cannot deserialize sub-objects unless a model is provided"), reader);
			}
			SubItemToken token = StartSubItem(reader);
			if (key >= 0)
			{
				value = reader.model.Deserialize(key, value, reader);
			}
			else if (type == null || !reader.model.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, skipOtherFields: true, asListItem: false, autoCreate: true, insideList: false))
			{
				TypeModel.ThrowUnexpectedType(type);
			}
			EndSubItem(token, reader);
			return value;
		}

		public static void EndSubItem(SubItemToken token, ProtoReader reader)
		{
			int value = token.value;
			if (reader.wireType == WireType.EndGroup)
			{
				if (value >= 0)
				{
					throw AddErrorData(new ArgumentException("token"), reader);
				}
				if (-value != reader.fieldNumber)
				{
					throw reader.CreateException("Wrong group was ended");
				}
				reader.wireType = WireType.None;
				reader.depth--;
			}
			else
			{
				if (value < reader.position)
				{
					throw reader.CreateException("Sub-message not read entirely");
				}
				if (reader.blockEnd != reader.position && reader.blockEnd != int.MaxValue)
				{
					throw reader.CreateException("Sub-message not read correctly");
				}
				reader.blockEnd = value;
				reader.depth--;
			}
		}

		public static SubItemToken StartSubItem(ProtoReader reader)
		{
			switch (reader.wireType)
			{
			case WireType.String:
			{
				int num = (int)reader.ReadUInt32Variant(trimNegative: false);
				if (num < 0)
				{
					throw AddErrorData(new InvalidOperationException(), reader);
				}
				int value = reader.blockEnd;
				reader.blockEnd = reader.position + num;
				reader.depth++;
				return new SubItemToken(value);
			}
			case WireType.StartGroup:
				reader.wireType = WireType.None;
				reader.depth++;
				return new SubItemToken(-reader.fieldNumber);
			default:
				throw reader.CreateWireTypeException();
			}
		}

		public int ReadFieldHeader()
		{
			if (blockEnd <= position || wireType == WireType.EndGroup)
			{
				return 0;
			}
			if (TryReadUInt32Variant(out var value))
			{
				wireType = (WireType)(value & 7);
				fieldNumber = (int)(value >> 3);
				if (fieldNumber < 1)
				{
					throw new ProtoException("Invalid field in source data: " + fieldNumber);
				}
			}
			else
			{
				wireType = WireType.None;
				fieldNumber = 0;
			}
			if (wireType != WireType.EndGroup)
			{
				return fieldNumber;
			}
			return 0;
		}

		public bool TryReadFieldHeader(int field)
		{
			if (blockEnd <= position || this.wireType == WireType.EndGroup)
			{
				return false;
			}
			uint value;
			int num = TryReadUInt32VariantWithoutMoving(trimNegative: false, out value);
			WireType wireType;
			if (num > 0 && (int)value >> 3 == field && (wireType = (WireType)(value & 7)) != WireType.EndGroup)
			{
				this.wireType = wireType;
				fieldNumber = field;
				position += num;
				ioIndex += num;
				available -= num;
				return true;
			}
			return false;
		}

		public void Hint(WireType wireType)
		{
			if (this.wireType != wireType && (wireType & (WireType)7) == this.wireType)
			{
				this.wireType = wireType;
			}
		}

		public void Assert(WireType wireType)
		{
			if (this.wireType != wireType)
			{
				if ((wireType & (WireType)7) != this.wireType)
				{
					throw CreateWireTypeException();
				}
				this.wireType = wireType;
			}
		}

		public void SkipField()
		{
			switch (wireType)
			{
			case WireType.Variant:
			case WireType.SignedVariant:
				ReadUInt64Variant();
				break;
			case WireType.Fixed64:
				if (available < 8)
				{
					Ensure(8, strict: true);
				}
				available -= 8;
				ioIndex += 8;
				position += 8;
				break;
			case WireType.String:
			{
				int num = (int)ReadUInt32Variant(trimNegative: false);
				if (num <= available)
				{
					available -= num;
					ioIndex += num;
					position += num;
					break;
				}
				position += num;
				num -= available;
				ioIndex = (available = 0);
				if (isFixedLength)
				{
					if (num > dataRemaining)
					{
						throw EoF(this);
					}
					dataRemaining -= num;
				}
				Seek(source, num, ioBuffer);
				break;
			}
			case WireType.StartGroup:
			{
				int num2 = fieldNumber;
				while (ReadFieldHeader() > 0)
				{
					SkipField();
				}
				if (wireType == WireType.EndGroup && fieldNumber == num2)
				{
					wireType = WireType.None;
					break;
				}
				throw CreateWireTypeException();
			}
			case WireType.Fixed32:
				if (available < 4)
				{
					Ensure(4, strict: true);
				}
				available -= 4;
				ioIndex += 4;
				position += 4;
				break;
			default:
				throw CreateWireTypeException();
			}
		}

		public ulong ReadUInt64()
		{
			switch (wireType)
			{
			case WireType.Variant:
				return ReadUInt64Variant();
			case WireType.Fixed64:
				if (available < 8)
				{
					Ensure(8, strict: true);
				}
				position += 8;
				available -= 8;
				return ioBuffer[ioIndex++] | ((ulong)ioBuffer[ioIndex++] << 8) | ((ulong)ioBuffer[ioIndex++] << 16) | ((ulong)ioBuffer[ioIndex++] << 24) | ((ulong)ioBuffer[ioIndex++] << 32) | ((ulong)ioBuffer[ioIndex++] << 40) | ((ulong)ioBuffer[ioIndex++] << 48) | ((ulong)ioBuffer[ioIndex++] << 56);
			default:
				throw CreateWireTypeException();
			case WireType.Fixed32:
				return ReadUInt32();
			}
		}

		public unsafe float ReadSingle()
		{
			switch (wireType)
			{
			case WireType.Fixed32:
			{
				int num3 = ReadInt32();
				return *(float*)(&num3);
			}
			default:
				throw CreateWireTypeException();
			case WireType.Fixed64:
			{
				double num = ReadDouble();
				float num2 = (float)num;
				if (Helpers.IsInfinity(num2) && !Helpers.IsInfinity(num))
				{
					throw AddErrorData(new OverflowException(), this);
				}
				return num2;
			}
			}
		}

		public bool ReadBoolean()
		{
			switch (ReadUInt32())
			{
			case 0u:
				return false;
			case 1u:
				return true;
			default:
				throw CreateException("Unexpected boolean value");
			}
		}

		public static byte[] AppendBytes(byte[] value, ProtoReader reader)
		{
			if (reader.wireType != WireType.String)
			{
				throw reader.CreateWireTypeException();
			}
			int num = (int)reader.ReadUInt32Variant(trimNegative: false);
			reader.wireType = WireType.None;
			if (num != 0)
			{
				int num2;
				if (value == null || value.Length == 0)
				{
					num2 = 0;
					value = new byte[num];
				}
				else
				{
					num2 = value.Length;
					byte[] array = new byte[value.Length + num];
					Helpers.BlockCopy(value, 0, array, 0, value.Length);
					value = array;
				}
				reader.position += num;
				while (num > reader.available)
				{
					if (reader.available > 0)
					{
						Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num2, reader.available);
						num -= reader.available;
						num2 += reader.available;
						reader.ioIndex = (reader.available = 0);
					}
					int num3 = ((num > reader.ioBuffer.Length) ? reader.ioBuffer.Length : num);
					if (num3 > 0)
					{
						reader.Ensure(num3, strict: true);
					}
				}
				if (num > 0)
				{
					Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, num2, num);
					reader.ioIndex += num;
					reader.available -= num;
				}
				return value;
			}
			if (value != null)
			{
				return value;
			}
			return EmptyBlob;
		}

		private static byte[] ReadBytes(Stream stream, int length)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			byte[] array = new byte[length];
			int offset = 0;
			int num;
			while (length > 0 && (num = stream.Read(array, offset, length)) > 0)
			{
				length -= num;
			}
			if (length > 0)
			{
				throw EoF(null);
			}
			return array;
		}

		private static int ReadByteOrThrow(Stream source)
		{
			int num = source.ReadByte();
			if (num < 0)
			{
				throw EoF(null);
			}
			return num;
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber)
		{
			int bytesRead;
			return ReadLengthPrefix(source, expectHeader, style, out fieldNumber, out bytesRead);
		}

		public static int DirectReadLittleEndianInt32(Stream source)
		{
			return ReadByteOrThrow(source) | (ReadByteOrThrow(source) << 8) | (ReadByteOrThrow(source) << 16) | (ReadByteOrThrow(source) << 24);
		}

		public static int DirectReadBigEndianInt32(Stream source)
		{
			return (ReadByteOrThrow(source) << 24) | (ReadByteOrThrow(source) << 16) | (ReadByteOrThrow(source) << 8) | ReadByteOrThrow(source);
		}

		public static int DirectReadVarintInt32(Stream source)
		{
			if (TryReadUInt32Variant(source, out var value) <= 0)
			{
				throw EoF(null);
			}
			return (int)value;
		}

		public static void DirectReadBytes(Stream source, byte[] buffer, int offset, int count)
		{
			int num;
			while (count > 0 && (num = source.Read(buffer, offset, count)) > 0)
			{
				count -= num;
				offset += num;
			}
			if (count > 0)
			{
				throw EoF(null);
			}
		}

		public static byte[] DirectReadBytes(Stream source, int count)
		{
			byte[] array = new byte[count];
			DirectReadBytes(source, array, 0, count);
			return array;
		}

		public static string DirectReadString(Stream source, int length)
		{
			byte[] array = new byte[length];
			DirectReadBytes(source, array, 0, length);
			return Encoding.UTF8.GetString(array, 0, length);
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber, out int bytesRead)
		{
			fieldNumber = 0;
			switch (style)
			{
			case PrefixStyle.None:
				bytesRead = 0;
				return int.MaxValue;
			case PrefixStyle.Base128:
			{
				bytesRead = 0;
				if (expectHeader)
				{
					int num2 = TryReadUInt32Variant(source, out var value);
					bytesRead += num2;
					if (num2 <= 0)
					{
						bytesRead = 0;
						return -1;
					}
					if ((value & 7) != 2)
					{
						throw new InvalidOperationException();
					}
					fieldNumber = (int)(value >> 3);
					num2 = TryReadUInt32Variant(source, out value);
					bytesRead += num2;
					if (bytesRead == 0)
					{
						throw EoF(null);
					}
					return (int)value;
				}
				uint value2;
				int num3 = TryReadUInt32Variant(source, out value2);
				bytesRead += num3;
				if (bytesRead >= 0)
				{
					return (int)value2;
				}
				return -1;
			}
			case PrefixStyle.Fixed32:
			{
				int num4 = source.ReadByte();
				if (num4 < 0)
				{
					bytesRead = 0;
					return -1;
				}
				bytesRead = 4;
				return num4 | (ReadByteOrThrow(source) << 8) | (ReadByteOrThrow(source) << 16) | (ReadByteOrThrow(source) << 24);
			}
			case PrefixStyle.Fixed32BigEndian:
			{
				int num = source.ReadByte();
				if (num < 0)
				{
					bytesRead = 0;
					return -1;
				}
				bytesRead = 4;
				return (num << 24) | (ReadByteOrThrow(source) << 16) | (ReadByteOrThrow(source) << 8) | ReadByteOrThrow(source);
			}
			default:
				throw new ArgumentOutOfRangeException("style");
			}
		}

		private static int TryReadUInt32Variant(Stream source, out uint value)
		{
			value = 0u;
			int num = source.ReadByte();
			if (num < 0)
			{
				return 0;
			}
			value = (uint)num;
			if ((value & 0x80) == 0)
			{
				return 1;
			}
			value &= 127u;
			num = source.ReadByte();
			if (num < 0)
			{
				throw EoF(null);
			}
			value |= (uint)((num & 0x7F) << 7);
			if ((num & 0x80) == 0)
			{
				return 2;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw EoF(null);
			}
			value |= (uint)((num & 0x7F) << 14);
			if ((num & 0x80) == 0)
			{
				return 3;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw EoF(null);
			}
			value |= (uint)((num & 0x7F) << 21);
			if ((num & 0x80) == 0)
			{
				return 4;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw EoF(null);
			}
			value |= (uint)(num << 28);
			if ((num & 0xF0) == 0)
			{
				return 5;
			}
			throw new OverflowException();
		}

		internal static void Seek(Stream source, int count, byte[] buffer)
		{
			if (source.CanSeek)
			{
				source.Seek(count, SeekOrigin.Current);
				count = 0;
			}
			else if (buffer != null)
			{
				int num;
				while (count > buffer.Length && (num = source.Read(buffer, 0, buffer.Length)) > 0)
				{
					count -= num;
				}
				int num2;
				while (count > 0 && (num2 = source.Read(buffer, 0, count)) > 0)
				{
					count -= num2;
				}
			}
			else
			{
				buffer = BufferPool.GetBuffer();
				try
				{
					int num3;
					while (count > buffer.Length && (num3 = source.Read(buffer, 0, buffer.Length)) > 0)
					{
						count -= num3;
					}
					while (count > 0 && (num3 = source.Read(buffer, 0, count)) > 0)
					{
						count -= num3;
					}
				}
				finally
				{
					BufferPool.ReleaseBufferToPool(ref buffer);
				}
			}
			if (count > 0)
			{
				throw EoF(null);
			}
		}

		internal static Exception AddErrorData(Exception exception, ProtoReader source)
		{
			if (exception != null && source != null && !exception.Data.Contains("protoSource"))
			{
				exception.Data.Add("protoSource", $"tag={source.fieldNumber}; wire-type={source.wireType}; offset={source.position}; depth={source.depth}");
			}
			return exception;
		}

		private static Exception EoF(ProtoReader source)
		{
			return AddErrorData(new EndOfStreamException(), source);
		}

		public void AppendExtensionData(IExtensible instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			IExtension extensionObject = instance.GetExtensionObject(createIfMissing: true);
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, model, null))
				{
					AppendExtensionField(protoWriter);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		private void AppendExtensionField(ProtoWriter writer)
		{
			ProtoWriter.WriteFieldHeader(fieldNumber, wireType, writer);
			switch (wireType)
			{
			case WireType.Variant:
			case WireType.Fixed64:
			case WireType.SignedVariant:
				ProtoWriter.WriteInt64(ReadInt64(), writer);
				break;
			case WireType.String:
				ProtoWriter.WriteBytes(AppendBytes(null, this), writer);
				break;
			case WireType.StartGroup:
			{
				SubItemToken token = StartSubItem(this);
				SubItemToken token2 = ProtoWriter.StartSubItem(null, writer);
				while (ReadFieldHeader() > 0)
				{
					AppendExtensionField(writer);
				}
				EndSubItem(token, this);
				ProtoWriter.EndSubItem(token2, writer);
				break;
			}
			case WireType.Fixed32:
				ProtoWriter.WriteInt32(ReadInt32(), writer);
				break;
			default:
				throw CreateWireTypeException();
			}
		}

		public static bool HasSubValue(WireType wireType, ProtoReader source)
		{
			if (source.blockEnd <= source.position || wireType == WireType.EndGroup)
			{
				return false;
			}
			source.wireType = wireType;
			return true;
		}

		internal int GetTypeKey(ref Type type)
		{
			return model.GetKey(ref type);
		}

		internal Type DeserializeType(string value)
		{
			return TypeModel.DeserializeType(model, value);
		}

		internal void SetRootObject(object value)
		{
			netCache.SetKeyedObject(0, value);
			trapCount--;
		}

		public static void NoteObject(object value, ProtoReader reader)
		{
			if (reader.trapCount != 0)
			{
				reader.netCache.RegisterTrappedObject(value);
				reader.trapCount--;
			}
		}

		public Type ReadType()
		{
			return TypeModel.DeserializeType(model, ReadString());
		}

		internal void TrapNextObject(int newObjectKey)
		{
			trapCount++;
			netCache.SetKeyedObject(newObjectKey, null);
		}

		internal void CheckFullyConsumed()
		{
			if (isFixedLength && dataRemaining != 0)
			{
				throw new ProtoException("Incorrect number of bytes consumed");
			}
		}
	}
}
