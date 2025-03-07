using System.IO;
using System.Text;
using Localize.Translate;
using UnityEngine;

public class CreateURL : MonoBehaviour
{
	[SerializeField]
	private string KK_Check_URL = string.Empty;

	[SerializeField]
	private string KK_Cha_URL = string.Empty;

	[SerializeField]
	private string KK_Ranking_URL = string.Empty;

	[SerializeField]
	private string KK_Check_URL_SV = string.Empty;

	[SerializeField]
	private string KK_Cha_URL_SV = string.Empty;

	public static string Load_KK_Check_URL()
	{
		return Load_KK_Check_URL_SV();
	}

	public static string Load_KK_Cha_URL()
	{
		return Load_KK_Cha_URL_SV();
	}

	public static string Load_KK_Ranking_URL()
	{
		return LoadURL("kk_ranking_url.dat");
	}

	private static string LoadURL(string urlFile)
	{
		return LoadURL(UserData.Path, urlFile);
	}

	private static string Load_KK_Check_URL_SV()
	{
		return LoadURL(Localize.Translate.Manager.DefaultData.commonFile.Path, "kk_check_url_sv.dat");
	}

	private static string Load_KK_Cha_URL_SV()
	{
		return LoadURL(Localize.Translate.Manager.DefaultData.commonFile.Path, "kk_cha_url_sv.dat");
	}

	public static string LoadURL(string firstPath, string urlFile)
	{
		string path = firstPath + "/url/" + urlFile;
		if (!File.Exists(path))
		{
			return string.Empty;
		}
		byte[] array = null;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				array = binaryReader.ReadBytes((int)fileStream.Length);
			}
		}
		if (array == null)
		{
			return string.Empty;
		}
		byte[] bytes = YS_Assist.DecryptAES(array, "koikatu", "phpaddress");
		return Encoding.UTF8.GetString(bytes);
	}

	private void CreateKK_Check_URL()
	{
		CreateFile(UserData.Path, KK_Check_URL, "kk_check_url.dat");
	}

	private void CreateKK_Cha_URL()
	{
		CreateFile(UserData.Path, KK_Cha_URL, "kk_cha_url.dat");
	}

	private void CreateKK_Ranking_URL()
	{
		CreateFile(UserData.Path, KK_Ranking_URL, "kk_ranking_url.dat");
	}

	private void CreateKK_Check_URL_SV()
	{
		CreateFile(UserData.Path, KK_Check_URL_SV, "kk_check_url_sv.dat");
	}

	private void CreateKK_Cha_URL_SV()
	{
		CreateFile(UserData.Path, KK_Cha_URL_SV, "kk_cha_url_sv.dat");
	}

	private static void CreateFile(string firstPath, string url, string urlFile)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(url);
		byte[] buffer = YS_Assist.EncryptAES(bytes, "koikatu", "phpaddress");
		string path = firstPath + "/url/" + urlFile;
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(buffer);
			}
		}
	}
}
