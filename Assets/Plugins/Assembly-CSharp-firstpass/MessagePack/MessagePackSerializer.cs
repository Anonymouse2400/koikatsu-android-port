using System;
using System.Globalization;
using System.IO;
using System.Text;
using MessagePack.Formatters;
using MessagePack.Internal;
using MessagePack.Resolvers;

namespace MessagePack
{
	public static class MessagePackSerializer
	{
		private static IFormatterResolver defaultResolver;

		public static IFormatterResolver DefaultResolver
		{
			get
			{
				if (defaultResolver == null)
				{
					defaultResolver = StandardResolver.Instance;
				}
				return defaultResolver;
			}
		}

		public static bool IsInitialized
		{
			get
			{
				return defaultResolver != null;
			}
		}

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

		public static void SetDefaultResolver(IFormatterResolver resolver)
		{
			defaultResolver = resolver;
		}

		public static byte[] Serialize<T>(T obj)
		{
			return Serialize(obj, defaultResolver);
		}

		public static byte[] Serialize<T>(T obj, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = DefaultResolver;
			}
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			byte[] bytes = InternalMemoryPool.GetBuffer();
			int newSize = formatterWithVerify.Serialize(ref bytes, 0, obj, resolver);
			return MessagePackBinary.FastCloneWithResize(bytes, newSize);
		}

		public static ArraySegment<byte> SerializeUnsafe<T>(T obj)
		{
			return SerializeUnsafe(obj, defaultResolver);
		}

		public static ArraySegment<byte> SerializeUnsafe<T>(T obj, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = DefaultResolver;
			}
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			byte[] bytes = InternalMemoryPool.GetBuffer();
			int count = formatterWithVerify.Serialize(ref bytes, 0, obj, resolver);
			return new ArraySegment<byte>(bytes, 0, count);
		}

		public static void Serialize<T>(Stream stream, T obj)
		{
			Serialize(stream, obj, defaultResolver);
		}

		public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = DefaultResolver;
			}
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			byte[] bytes = InternalMemoryPool.GetBuffer();
			int count = formatterWithVerify.Serialize(ref bytes, 0, obj, resolver);
			stream.Write(bytes, 0, count);
		}

		public static T Deserialize<T>(byte[] bytes)
		{
			return Deserialize<T>(bytes, defaultResolver);
		}

		public static T Deserialize<T>(byte[] bytes, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = DefaultResolver;
			}
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			int readSize;
			return formatterWithVerify.Deserialize(bytes, 0, resolver, out readSize);
		}

		public static T Deserialize<T>(Stream stream)
		{
			return Deserialize<T>(stream, defaultResolver);
		}

		public static T Deserialize<T>(Stream stream, IFormatterResolver resolver)
		{
			if (resolver == null)
			{
				resolver = DefaultResolver;
			}
			IMessagePackFormatter<T> formatterWithVerify = resolver.GetFormatterWithVerify<T>();
			byte[] buffer = InternalMemoryPool.GetBuffer();
			FillFromStream(stream, ref buffer);
			int readSize;
			return formatterWithVerify.Deserialize(buffer, 0, resolver, out readSize);
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
