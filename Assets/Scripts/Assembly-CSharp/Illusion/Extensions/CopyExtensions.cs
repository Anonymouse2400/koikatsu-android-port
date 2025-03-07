using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Illusion.Extensions
{
	public static class CopyExtensions
	{
		public static T DeepCopy<T>(this T self)
		{
			if (self == null)
			{
				return default(T);
			}
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, self);
				memoryStream.Position = 0L;
				return (T)binaryFormatter.Deserialize(memoryStream);
			}
			finally
			{
				memoryStream.Close();
			}
		}
	}
}
