using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Illusion.Elements.Xml;
using UnityEngine;

namespace Config
{
	public abstract class BaseSystem : Data
	{
		public FieldInfo[] FieldInfos
		{
			get
			{
				return GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			}
		}

		public BaseSystem(string elementName)
			: base(elementName)
		{
		}

		public override void Read(string rootName, XmlDocument xml)
		{
			string text = rootName + "/" + elementName + "/";
			XmlNodeList xmlNodeList = null;
			FieldInfo[] fieldInfos = FieldInfos;
			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				xmlNodeList = xml.SelectNodes(text + fieldInfo.Name);
				if (xmlNodeList != null)
				{
					XmlElement xmlElement = xmlNodeList.Item(0) as XmlElement;
					if (xmlElement != null)
					{
						fieldInfo.SetValue(this, Cast(xmlElement.InnerText, fieldInfo.FieldType));
					}
				}
			}
		}

		public override void Write(XmlWriter writer)
		{
			writer.WriteStartElement(elementName);
			FieldInfo[] fieldInfos = FieldInfos;
			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				writer.WriteStartElement(fieldInfo.Name);
				writer.WriteValue(ConvertString(fieldInfo.GetValue(this)));
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public static object Cast(string str, Type type)
		{
			if (type == typeof(Color))
			{
				string[] array = str.Split(',');
				if (array.Length == 4)
				{
					int num = 0;
					return new Color(float.Parse(array[num++]), float.Parse(array[num++]), float.Parse(array[num++]), float.Parse(array[num++]));
				}
				return Color.white;
			}
			if (type.IsArray)
			{
				string[] array2 = str.Split(',');
				Type elementType = type.GetElementType();
				Array array3 = Array.CreateInstance(elementType, array2.Length);
				{
					foreach (var item in array2.Select((string v, int i) => new { v, i }))
					{
						array3.SetValue(Convert.ChangeType(item.v, elementType), item.i);
					}
					return array3;
				}
			}
			return Convert.ChangeType(str, type);
		}

		public static string ConvertString(object o)
		{
			if (o is Color)
			{
				Color color = (Color)o;
				return string.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
			}
			if (o.GetType().IsArray)
			{
				Array array = (Array)o;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array.GetValue(i));
					if (i + 1 < array.Length)
					{
						stringBuilder.Append(",");
					}
				}
				return stringBuilder.ToString();
			}
			return o.ToString();
		}
	}
}
