using System;
using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class EnumAsStringFormatter<T> : IMessagePackFormatter<T>
	{
		private readonly Dictionary<string, T> nameValueMapping;

		private readonly Dictionary<T, string> valueNameMapping;

		public EnumAsStringFormatter()
		{
			string[] names = Enum.GetNames(typeof(T));
			Array values = Enum.GetValues(typeof(T));
			nameValueMapping = new Dictionary<string, T>(names.Length);
			valueNameMapping = new Dictionary<T, string>(names.Length);
			for (int i = 0; i < names.Length; i++)
			{
				nameValueMapping[names[i]] = (T)values.GetValue(i);
				valueNameMapping[(T)values.GetValue(i)] = names[i];
			}
		}

		public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
		{
			string value2;
			if (!valueNameMapping.TryGetValue(value, out value2))
			{
				value2 = value.ToString();
			}
			return MessagePackBinary.WriteString(ref bytes, offset, value2);
		}

		public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
		{
			string text = MessagePackBinary.ReadString(bytes, offset, out readSize);
			T value;
			if (!nameValueMapping.TryGetValue(text, out value))
			{
				return (T)Enum.Parse(typeof(T), text);
			}
			return value;
		}
	}
}
