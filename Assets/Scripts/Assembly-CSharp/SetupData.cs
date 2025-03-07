using System.IO;
using System.Xml;
using System.Xml.Linq;
using Manager;
using UnityEngine;

public static class SetupData
{
	private const string SETUP_PATH = "UserData/setup.xml";

	public static bool isExists
	{
		get
		{
			return File.Exists("UserData/setup.xml");
		}
	}

	public static void Load()
	{
		if (!isExists)
		{
			return;
		}
		try
		{
			int width = 1280;
			int height = 720;
			int num = 2;
			bool fullscreen = false;
			XElement xElement = XElement.Load("UserData/setup.xml");
			if (xElement == null)
			{
				return;
			}
			foreach (XElement item in xElement.Elements())
			{
				switch (item.Name.ToString())
				{
				case "Width":
					width = int.Parse(item.Value);
					break;
				case "Height":
					height = int.Parse(item.Value);
					break;
				case "FullScreen":
					fullscreen = bool.Parse(item.Value);
					break;
				case "Quality":
					num = int.Parse(item.Value);
					break;
				}
			}
			Screen.SetResolution(width, height, fullscreen);
			switch (num)
			{
			case 0:
				QualitySettings.SetQualityLevel((!Manager.Config.EtcData.SelfShadow) ? 1 : 0);
				break;
			case 1:
				QualitySettings.SetQualityLevel((!Manager.Config.EtcData.SelfShadow) ? 3 : 2);
				break;
			case 2:
				QualitySettings.SetQualityLevel((!Manager.Config.EtcData.SelfShadow) ? 5 : 4);
				break;
			}
		}
		catch (XmlException)
		{
			File.Delete("UserData/setup.xml");
		}
	}

	public static int LoadLanguage()
	{
		int result = 1;
		if (!isExists)
		{
			return result;
		}
		try
		{
			XElement xElement = XElement.Load("UserData/setup.xml");
			if (xElement != null)
			{
				foreach (XElement item in xElement.Elements())
				{
					if (item.Name.ToString() == "Language")
					{
						result = int.Parse(item.Value);
						break;
					}
				}
			}
		}
		catch (XmlException)
		{
			File.Delete("UserData/setup.xml");
		}
		return result;
	}
}
