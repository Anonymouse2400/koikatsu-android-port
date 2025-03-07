using System.IO;
using System.Xml.Serialization;

namespace Illusion.Serialize
{
	public static class Xml
	{
		public static T Seialize<T>(string filename, T data)
		{
			using (FileStream stream = new FileStream(filename, FileMode.Create))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				xmlSerializer.Serialize(stream, data);
				return data;
			}
		}

		public static T Deserialize<T>(string filename)
		{
			using (FileStream stream = new FileStream(filename, FileMode.Open))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				return (T)xmlSerializer.Deserialize(stream);
			}
		}
	}
}
