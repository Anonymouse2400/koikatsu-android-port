using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Illusion.Elements.Xml
{
	public class Control
	{
		private readonly string savePath;

		private readonly string saveName;

		private readonly string rootName;

		private Data[] datas;

		public Data[] Datas
		{
			get
			{
				return datas;
			}
		}

		public Data this[int index]
		{
			get
			{
				return datas[index];
			}
		}

		public Control(string savePath, string saveName, string rootName, params Data[] datas)
		{
			this.savePath = savePath;
			this.saveName = saveName;
			this.rootName = rootName;
			this.datas = datas;
			Init();
		}

		public void Init()
		{
			Data[] array = datas;
			foreach (Data data in array)
			{
				data.Init();
			}
		}

		public void Write()
		{
			string file = UserData.Create(savePath) + saveName;
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			XmlWriter xmlWriter = null;
			try
			{
				xmlWriter = XmlWriter.Create(file, xmlWriterSettings);
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement(rootName);
				Data[] array = datas;
				foreach (Data data in array)
				{
					data.Write(xmlWriter);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Close();
				}
			}
		}

		public void Read()
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				string text = UserData.Path + savePath + '/' + saveName;
				if (File.Exists(text))
				{
					xmlDocument.Load(text);
					Data[] array = datas;
					foreach (Data data in array)
					{
						data.Read(rootName, xmlDocument);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
