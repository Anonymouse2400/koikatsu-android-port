using System.Collections;
using System.IO;
using Manager;
using UnityEngine;

public class InitScene : BaseLoader
{
	private IEnumerator Start()
	{
		yield return new WaitUntil(Singleton<Manager.Config>.IsInstance);
		if (SystemInfo.graphicsShaderLevel < 30)
		{
			MonoBehaviour.print("shaders Non support");
		}
		SetupData.Load();
		if (CommonLib.GetUUID() == string.Empty)
		{
			string registryInfoFrom = YS_Assist.GetRegistryInfoFrom("Software\\illusion\\Koikatu\\KoikatuTrial", "INSTALLDIR");
			string text = "UserData/save/";
			string text2 = "netUID.dat";
			string text3 = registryInfoFrom + text + text2;
			if (File.Exists(text3))
			{
				string text4 = Path.GetDirectoryName(Application.dataPath) + "/" + text;
				string destFileName = text4 + text2;
				if (!Directory.Exists(text4))
				{
					Directory.CreateDirectory(text4);
				}
				File.Copy(text3, destFileName);
			}
			else
			{
				CommonLib.CreateUUID();
			}
		}
		string nextScene = "Logo";
		Scene.Data data = new Scene.Data
		{
			levelName = nextScene
		};
		Singleton<Scene>.Instance.LoadReserve(data, false);
	}
}
