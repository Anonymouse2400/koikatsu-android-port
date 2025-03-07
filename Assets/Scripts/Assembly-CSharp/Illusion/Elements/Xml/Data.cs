using System.Xml;

namespace Illusion.Elements.Xml
{
	public abstract class Data
	{
		protected readonly string elementName;

		public Data(string elementName)
		{
			this.elementName = elementName;
		}

		public virtual void Init()
		{
		}

		public virtual void Read(string rootName, XmlDocument xml)
		{
		}

		public virtual void Write(XmlWriter writer)
		{
		}
	}
}
