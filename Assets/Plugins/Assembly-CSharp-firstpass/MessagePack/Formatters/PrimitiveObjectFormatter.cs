using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MessagePack.Formatters
{
	public class PrimitiveObjectFormatter : IMessagePackFormatter<object>
	{
		public static readonly IMessagePackFormatter<object> Instance = new PrimitiveObjectFormatter();

		private static readonly Dictionary<Type, int> typeToJumpCode = new Dictionary<Type, int>
		{
			{
				typeof(bool),
				0
			},
			{
				typeof(char),
				1
			},
			{
				typeof(sbyte),
				2
			},
			{
				typeof(byte),
				3
			},
			{
				typeof(short),
				4
			},
			{
				typeof(ushort),
				5
			},
			{
				typeof(int),
				6
			},
			{
				typeof(uint),
				7
			},
			{
				typeof(long),
				8
			},
			{
				typeof(ulong),
				9
			},
			{
				typeof(float),
				10
			},
			{
				typeof(double),
				11
			},
			{
				typeof(DateTime),
				12
			},
			{
				typeof(string),
				13
			},
			{
				typeof(byte[]),
				14
			}
		};

		private PrimitiveObjectFormatter()
		{
		}

		public int Serialize(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver)
		{
			if (value == null)
			{
				return MessagePackBinary.WriteNil(ref bytes, offset);
			}
			Type type = value.GetType();
			int value2;
			if (typeToJumpCode.TryGetValue(type, out value2))
			{
				switch (value2)
				{
				case 0:
					return MessagePackBinary.WriteBoolean(ref bytes, offset, (bool)value);
				case 1:
					return MessagePackBinary.WriteChar(ref bytes, offset, (char)value);
				case 2:
					return MessagePackBinary.WriteSByteForceSByteBlock(ref bytes, offset, (sbyte)value);
				case 3:
					return MessagePackBinary.WriteByteForceByteBlock(ref bytes, offset, (byte)value);
				case 4:
					return MessagePackBinary.WriteInt16ForceInt16Block(ref bytes, offset, (short)value);
				case 5:
					return MessagePackBinary.WriteUInt16ForceUInt16Block(ref bytes, offset, (ushort)value);
				case 6:
					return MessagePackBinary.WriteInt32ForceInt32Block(ref bytes, offset, (int)value);
				case 7:
					return MessagePackBinary.WriteUInt32ForceUInt32Block(ref bytes, offset, (uint)value);
				case 8:
					return MessagePackBinary.WriteInt64ForceInt64Block(ref bytes, offset, (long)value);
				case 9:
					return MessagePackBinary.WriteUInt64ForceUInt64Block(ref bytes, offset, (ulong)value);
				case 10:
					return MessagePackBinary.WriteSingle(ref bytes, offset, (float)value);
				case 11:
					return MessagePackBinary.WriteDouble(ref bytes, offset, (double)value);
				case 12:
					return MessagePackBinary.WriteDateTime(ref bytes, offset, (DateTime)value);
				case 13:
					return MessagePackBinary.WriteString(ref bytes, offset, (string)value);
				case 14:
					return MessagePackBinary.WriteBytes(ref bytes, offset, (byte[])value);
				default:
					throw new InvalidOperationException("Not supported primitive object resolver. type:" + type.Name);
				}
			}
			if (type.GetTypeInfo().IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(type);
				switch (typeToJumpCode[underlyingType])
				{
				case 2:
					return MessagePackBinary.WriteSByteForceSByteBlock(ref bytes, offset, (sbyte)value);
				case 3:
					return MessagePackBinary.WriteByteForceByteBlock(ref bytes, offset, (byte)value);
				case 4:
					return MessagePackBinary.WriteInt16ForceInt16Block(ref bytes, offset, (short)value);
				case 5:
					return MessagePackBinary.WriteUInt16ForceUInt16Block(ref bytes, offset, (ushort)value);
				case 6:
					return MessagePackBinary.WriteInt32ForceInt32Block(ref bytes, offset, (int)value);
				case 7:
					return MessagePackBinary.WriteUInt32ForceUInt32Block(ref bytes, offset, (uint)value);
				case 8:
					return MessagePackBinary.WriteInt64ForceInt64Block(ref bytes, offset, (long)value);
				case 9:
					return MessagePackBinary.WriteUInt64ForceUInt64Block(ref bytes, offset, (ulong)value);
				}
			}
			else
			{
				if (value is IDictionary)
				{
					IDictionary dictionary = value as IDictionary;
					int num = offset;
					offset += MessagePackBinary.WriteMapHeader(ref bytes, offset, dictionary.Count);
					foreach (DictionaryEntry item in dictionary)
					{
						offset += Serialize(ref bytes, offset, item.Key, formatterResolver);
						offset += Serialize(ref bytes, offset, item.Value, formatterResolver);
					}
					return offset - num;
				}
				if (value is ICollection)
				{
					ICollection collection = value as ICollection;
					int num2 = offset;
					offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, collection.Count);
					foreach (object item2 in collection)
					{
						offset += Serialize(ref bytes, offset, item2, formatterResolver);
					}
					return offset - num2;
				}
			}
			throw new InvalidOperationException("Not supported primitive object resolver. type:" + type.Name);
		}

		public object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			switch (MessagePackBinary.GetMessagePackType(bytes, offset))
			{
			case MessagePackType.Integer:
			{
				byte b = bytes[offset];
				if (224 <= b && b <= byte.MaxValue)
				{
					return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
				}
				if (0 <= b && b <= 127)
				{
					return MessagePackBinary.ReadByte(bytes, offset, out readSize);
				}
				switch (b)
				{
				case 208:
					return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
				case 209:
					return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
				case 210:
					return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
				case 211:
					return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
				case 204:
					return MessagePackBinary.ReadByte(bytes, offset, out readSize);
				case 205:
					return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
				case 206:
					return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
				case 207:
					return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
				default:
					throw new InvalidOperationException("Invalid primitive bytes.");
				}
			}
			case MessagePackType.Boolean:
				return MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
			case MessagePackType.Float:
				if (bytes[offset] == 202)
				{
					return MessagePackBinary.ReadSingle(bytes, offset, out readSize);
				}
				return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
			case MessagePackType.String:
				return MessagePackBinary.ReadString(bytes, offset, out readSize);
			case MessagePackType.Binary:
				return MessagePackBinary.ReadBytes(bytes, offset, out readSize);
			case MessagePackType.Extension:
				if (MessagePackBinary.ReadExtensionFormat(bytes, offset, out readSize).TypeCode == -1)
				{
					return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
				}
				throw new InvalidOperationException("Invalid primitive bytes.");
			case MessagePackType.Array:
			{
				int num3 = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
				int num4 = offset;
				offset += readSize;
				object[] array = new object[num3];
				for (int j = 0; j < num3; j++)
				{
					array[j] = Deserialize(bytes, offset, formatterResolver, out readSize);
					offset += readSize;
				}
				readSize = offset - num4;
				return array;
			}
			case MessagePackType.Map:
			{
				int num = MessagePackBinary.ReadMapHeader(bytes, offset, out readSize);
				int num2 = offset;
				offset += readSize;
				Dictionary<object, object> dictionary = new Dictionary<object, object>(num);
				for (int i = 0; i < num; i++)
				{
					object key = Deserialize(bytes, offset, formatterResolver, out readSize);
					offset += readSize;
					object value = Deserialize(bytes, offset, formatterResolver, out readSize);
					offset += readSize;
					dictionary.Add(key, value);
				}
				readSize = offset - num2;
				return dictionary;
			}
			case MessagePackType.Nil:
				readSize = 1;
				return null;
			default:
				throw new InvalidOperationException("Invalid primitive bytes.");
			}
		}
	}
}
