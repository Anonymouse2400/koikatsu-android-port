using System;
using System.Globalization;
using System.IO;
using System.Text;
using MessagePack.Formatters;
using MessagePack.Internal;
using MessagePack.LZ4;

namespace MessagePack
{
	public static class LZ4MessagePackSerializer
	{
		public const sbyte ExtensionTypeCode = 99;

		public const int NotCompressionSize = 64;

		public static string ToJson<T>(T obj)
		{
			return ToJson(Serialize(obj));
		}

		public static string ToJson<T>(T obj, IFormatterResolver resolver)
		{
			return ToJson(Serialize(obj, resolver));
		}

		public static string ToJson(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return string.Empty;
			}
			int readSize;
			if (MessagePackBinary.GetMessagePackType(bytes, 0) == MessagePackType.Extension && MessagePackBinary.ReadExtensionFormatHeader(bytes, 0, out readSize).TypeCode == 99)
			{
				int num = readSize;
				int num2 = MessagePackBinary.ReadInt32(bytes, num, out readSize);
				num += readSize;
				byte[] array = LZ4MemoryPool.GetBuffer();
				if (array.Length < num2)
				{
					array = new byte[num2];
				}
				LZ4Codec.Decode(bytes, num, bytes.Length - num, array, 0, num2);
				bytes = array;
			}
			StringBuilder stringBuilder = new StringBuilder();
			ToJsonCore(bytes, 0, stringBuilder);
			return stringBuilder.ToString();
		}

		private static int ToJsonCore(byte[] bytes, int offset, StringBuilder builder)
		{
			int readSize = 0;
			switch (MessagePackBinary.GetMessagePackType(bytes, offset))
			{
			case MessagePackType.Integer:
			{
				byte b = bytes[offset];
				if (224 <= b && b <= byte.MaxValue)
				{
					builder.Append(MessagePackBinary.ReadSByte(bytes, offset, out readSize));
					break;
				}
				if (0 <= b && b <= 127)
				{
					builder.Append(MessagePackBinary.ReadByte(bytes, offset, out readSize));
					break;
				}
				switch (b)
				{
				case 208:
					builder.Append(MessagePackBinary.ReadSByte(bytes, offset, out readSize));
					break;
				case 209:
					builder.Append(MessagePackBinary.ReadInt16(bytes, offset, out readSize));
					break;
				case 210:
					builder.Append(MessagePackBinary.ReadInt32(bytes, offset, out readSize));
					break;
				case 211:
					builder.Append(MessagePackBinary.ReadInt64(bytes, offset, out readSize));
					break;
				case 204:
					builder.Append(MessagePackBinary.ReadByte(bytes, offset, out readSize));
					break;
				case 205:
					builder.Append(MessagePackBinary.ReadUInt16(bytes, offset, out readSize));
					break;
				case 206:
					builder.Append(MessagePackBinary.ReadUInt32(bytes, offset, out readSize));
					break;
				case 207:
					builder.Append(MessagePackBinary.ReadUInt64(bytes, offset, out readSize));
					break;
				}
				break;
			}
			case MessagePackType.Boolean:
				builder.Append((!MessagePackBinary.ReadBoolean(bytes, offset, out readSize)) ? "false" : "true");
				break;
			case MessagePackType.Float:
				builder.Append(MessagePackBinary.ReadDouble(bytes, offset, out readSize));
				break;
			case MessagePackType.String:
				WriteJsonString(MessagePackBinary.ReadString(bytes, offset, out readSize), builder);
				break;
			case MessagePackType.Binary:
				builder.Append("\"" + Convert.ToBase64String(MessagePackBinary.ReadBytes(bytes, offset, out readSize)) + "\"");
				break;
			case MessagePackType.Array:
			{
				uint num3 = MessagePackBinary.ReadArrayHeaderRaw(bytes, offset, out readSize);
				int num4 = readSize;
				offset += readSize;
				builder.Append("[");
				for (int j = 0; j < num3; j++)
				{
					readSize = ToJsonCore(bytes, offset, builder);
					offset += readSize;
					num4 += readSize;
					if (j != num3 - 1)
					{
						builder.Append(",");
					}
				}
				builder.Append("]");
				return num4;
			}
			case MessagePackType.Map:
			{
				uint num = MessagePackBinary.ReadMapHeaderRaw(bytes, offset, out readSize);
				int num2 = readSize;
				offset += readSize;
				builder.Append("{");
				for (int i = 0; i < num; i++)
				{
					MessagePackType messagePackType = MessagePackBinary.GetMessagePackType(bytes, offset);
					if (messagePackType == MessagePackType.String || messagePackType == MessagePackType.Binary)
					{
						readSize = ToJsonCore(bytes, offset, builder);
					}
					else
					{
						builder.Append("\"");
						readSize = ToJsonCore(bytes, offset, builder);
						builder.Append("\"");
					}
					offset += readSize;
					num2 += readSize;
					builder.Append(":");
					readSize = ToJsonCore(bytes, offset, builder);
					offset += readSize;
					num2 += readSize;
					if (i != num - 1)
					{
						builder.Append(",");
					}
				}
				builder.Append("}");
				return num2;
			}
			case MessagePackType.Extension:
			{
				ExtensionResult extensionResult = MessagePackBinary.ReadExtensionFormat(bytes, offset, out readSize);
				if (extensionResult.TypeCode == -1)
				{
					DateTime dateTime = MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
					builder.Append("\"");
					builder.Append(dateTime.ToString("o", CultureInfo.InvariantCulture));
					builder.Append("\"");
				}
				else
				{
					builder.Append("[");
					builder.Append(extensionResult.TypeCode);
					builder.Append(",");
					builder.Append("\"");
					builder.Append(Convert.ToBase64String(extensionResult.Data));
					builder.Append("\"");
					builder.Append("]");
				}
				break;
			}
			default:
				readSize = 1;
				builder.Append("null");
				break;
			}
			return readSize;
		}

		private static void WriteJsonString(string value, StringBuilder builder)
		{
			builder.Append('"');
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				switch (c)
				{
				case '"':
					builder.Append("\\\"");
					break;
				case '\\':
					builder.Append("\\\\");
					break;
				case '\b':
					builder.Append("\\b");
					break;
				case '\f':
					builder.Append("\\f");
					break;
				case '\n':
					builder.Append("\\n");
					break;
				case '\r':
					builder.Append("\\r");
					break;
				case '\t':
					builder.Append("\\t");
					break;
				default:
					builder.Append(c);
					break;
				}
			}
			builder.Append('"');
		}

		public static byte[] Serialize<T>(T obj)
		{
			return Serialize(obj, null);
		}

		public static byte[] Serialize<T>(T obj, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = MessagePackSerializer.DefaultResolver;
			}
			ArraySegment<byte> arraySegment = SerializeCore(obj, resolver);
			return MessagePackBinary.FastCloneWithResize(arraySegment.Array, arraySegment.Count);
		}

		public static void Serialize<T>(Stream stream, T obj)
		{
			Serialize(stream, obj, null);
		}

		public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = MessagePackSerializer.DefaultResolver;
			}
			ArraySegment<byte> arraySegment = SerializeCore(obj, resolver);
			stream.Write(arraySegment.Array, 0, arraySegment.Count);
		}

		public static int SerializeToBlock<T>(ref byte[] bytes, int offset, T obj, IFormatterResolver resolver)
		{
			ArraySegment<byte> arraySegment = MessagePackSerializer.SerializeUnsafe(obj, resolver);
			if (arraySegment.Count < 64)
			{
				MessagePackBinary.EnsureCapacity(ref bytes, offset, arraySegment.Count);
				Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, bytes, offset, arraySegment.Count);
				return arraySegment.Count;
			}
			int num = LZ4Codec.MaximumOutputLength(arraySegment.Count);
			MessagePackBinary.EnsureCapacity(ref bytes, offset, 11 + num);
			int num2 = offset;
			offset += 11;
			int num3 = LZ4Codec.Encode(arraySegment.Array, arraySegment.Offset, arraySegment.Count, bytes, offset, bytes.Length - offset);
			num2 += MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref bytes, num2, 99, num3 + 5);
			MessagePackBinary.WriteInt32ForceInt32Block(ref bytes, num2, arraySegment.Count);
			return 11 + num3;
		}

		private static ArraySegment<byte> SerializeCore<T>(T obj, IFormatterResolver resolver)
		{
			ArraySegment<byte> result = MessagePackSerializer.SerializeUnsafe(obj, resolver);
			if (result.Count < 64)
			{
				return result;
			}
			int num = 0;
			byte[] bytes = LZ4MemoryPool.GetBuffer();
			int num2 = LZ4Codec.MaximumOutputLength(result.Count);
			if (bytes.Length + 6 + 5 < num2)
			{
				bytes = new byte[11 + num2];
			}
			int num3 = num;
			num += 11;
			int num4 = LZ4Codec.Encode(result.Array, result.Offset, result.Count, bytes, num, bytes.Length - num);
			num3 += MessagePackBinary.WriteExtensionFormatHeaderForceExt32Block(ref bytes, num3, 99, num4 + 5);
			MessagePackBinary.WriteInt32ForceInt32Block(ref bytes, num3, result.Count);
			return new ArraySegment<byte>(bytes, 0, 11 + num4);
		}

		public static T Deserialize<T>(byte[] bytes)
		{
			return Deserialize<T>(bytes, null);
		}

		public static T Deserialize<T>(byte[] bytes, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = MessagePackSerializer.DefaultResolver;
			}
			return DeserializeCore<T>(new ArraySegment<byte>(bytes, 0, bytes.Length), resolver);
		}

		public static T Deserialize<T>(Stream stream)
		{
			return Deserialize<T>(stream, null);
		}

		public static T Deserialize<T>(Stream stream, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = MessagePackSerializer.DefaultResolver;
			}
			byte[] buffer = InternalMemoryPool.GetBuffer();
			int count = FillFromStream(stream, ref buffer);
			return DeserializeCore<T>(new ArraySegment<byte>(buffer, 0, count), resolver);
		}

		private static T DeserializeCore<T>(ArraySegment<byte> bytes, IFormatterResolver resolver)
		{
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			int readSize;
			if (MessagePackBinary.GetMessagePackType(bytes.Array, 0) == MessagePackType.Extension && MessagePackBinary.ReadExtensionFormatHeader(bytes.Array, bytes.Offset, out readSize).TypeCode == 99)
			{
				int num = bytes.Offset + readSize;
				int num2 = MessagePackBinary.ReadInt32(bytes.Array, num, out readSize);
				num += readSize;
				byte[] array = LZ4MemoryPool.GetBuffer();
				if (array.Length < num2)
				{
					array = new byte[num2];
				}
				LZ4Codec.Decode(bytes.Array, num, bytes.Count - num, array, 0, num2);
				return formatterWithVerify.Deserialize(array, 0, resolver, out readSize);
			}
			return formatterWithVerify.Deserialize(bytes.Array, bytes.Offset, resolver, out readSize);
		}

		private static int FillFromStream(Stream input, ref byte[] buffer)
		{
			int num = 0;
			int num2;
			while ((num2 = input.Read(buffer, num, buffer.Length - num)) > 0)
			{
				num += num2;
				if (num == buffer.Length)
				{
					MessagePackBinary.FastResize(ref buffer, num * 2);
				}
			}
			return num;
		}
	}
}
