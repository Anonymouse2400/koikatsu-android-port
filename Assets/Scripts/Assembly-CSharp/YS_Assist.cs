using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using UnityEngine;

public static class YS_Assist
{
	private static readonly string passwordChars36 = "0123456789abcdefghijklmnopqrstuvwxyz";

	private static readonly string passwordChars62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

	private static readonly char[] tbl62 = new char[62]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
		'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
		'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
		'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
		'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
		'Y', 'Z'
	};

	private static readonly byte[] tblRevCode = new byte[128]
	{
		50, 112, 114, 160, 140, 152, 202, 10, 235, 9,
		198, 113, 78, 208, 182, 154, 247, 249, 64, 243,
		232, 102, 184, 130, 196, 33, 149, 171, 62, 235,
		124, 183, 193, 189, 168, 165, 243, 117, 48, 23,
		16, 114, 192, 105, 122, 253, 206, 143, 240, 183,
		150, 127, 115, 117, 19, 135, 140, 187, 73, 133,
		254, 231, 48, 92, 205, 127, 122, 237, 250, 212,
		27, 92, 153, 237, 54, 161, 135, 216, 104, 10,
		60, 128, 97, 33, 47, 124, 18, 218, 168, 133,
		217, 249, 188, 179, 198, 104, 68, 229, 179, 61,
		10, 22, 10, 183, 8, 189, 74, 86, 107, 47,
		230, 233, 158, 241, 27, 85, 198, 164, 151, 135,
		148, 4, 24, 172, 122, 214, 18, 140
	};

	public static T DeepCopyWithSerializationSurrogate<T>(T target) where T : ISerializationSurrogate
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			SurrogateSelector surrogateSelector = new SurrogateSelector();
			surrogateSelector.AddSurrogate(context: new StreamingContext(StreamingContextStates.All), type: typeof(T), surrogate: target);
			binaryFormatter.SurrogateSelector = surrogateSelector;
			try
			{
				binaryFormatter.Serialize(memoryStream, target);
				memoryStream.Position = 0L;
				return (T)binaryFormatter.Deserialize(memoryStream);
			}
			finally
			{
				memoryStream.Close();
			}
		}
	}

	public static bool CheckFlagsArray(params bool[] flags)
	{
		if (flags.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < flags.Length; i++)
		{
			if (!flags[i])
			{
				return false;
			}
		}
		return true;
	}

	public static bool SetActiveControl(GameObject obj, params bool[] flags)
	{
		if (null == obj)
		{
			return false;
		}
		bool flag = CheckFlagsArray(flags);
		if (obj.activeSelf != flag)
		{
			obj.SetActive(flag);
			return true;
		}
		return false;
	}

	public static bool SetActiveControl(GameObject obj, bool flag)
	{
		if (null == obj)
		{
			return false;
		}
		if (obj.activeSelf != flag)
		{
			obj.SetActive(flag);
			return true;
		}
		return false;
	}

	public static void GetListString(string text, out string[,] data)
	{
		string[] array = text.Split('\n');
		int num = array.Length;
		if (num != 0 && array[num - 1].Trim() == string.Empty)
		{
			num--;
		}
		string[] array2 = array[0].Split('\t');
		int num2 = array2.Length;
		if (num2 != 0 && array2[num2 - 1].Trim() == string.Empty)
		{
			num2--;
		}
		data = new string[num, num2];
		for (int i = 0; i < num; i++)
		{
			string[] array3 = array[i].Split('\t');
			for (int j = 0; j < array3.Length && j < num2; j++)
			{
				data[i, j] = array3[j];
			}
			data[i, array3.Length - 1] = data[i, array3.Length - 1].Replace("\r", string.Empty).Replace("\n", string.Empty);
		}
	}

	public static void BitRevBytes(byte[] data, int startPos)
	{
		int num = startPos % tblRevCode.Length;
		for (int i = 0; i < data.Length; i++)
		{
			data[i] ^= tblRevCode[num];
			num = (num + 1) % tblRevCode.Length;
		}
	}

	public static string Convert64StringFromInt32(int num)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (num < 0)
		{
			num *= -1;
			stringBuilder.Append("0");
		}
		while (num >= 62)
		{
			stringBuilder.Append(tbl62[num % 62]);
			num /= 62;
		}
		stringBuilder.Append(tbl62[num]);
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int num2 = stringBuilder.Length - 1; num2 >= 0; num2--)
		{
			stringBuilder2.Append(stringBuilder[num2]);
		}
		return stringBuilder2.ToString();
	}

	public static string GeneratePassword36(int length)
	{
		StringBuilder stringBuilder = new StringBuilder(length);
		byte[] array = new byte[length];
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		rNGCryptoServiceProvider.GetBytes(array);
		for (int i = 0; i < length; i++)
		{
			int index = array[i] % passwordChars36.Length;
			char value = passwordChars36[index];
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public static string GeneratePassword62(int length)
	{
		StringBuilder stringBuilder = new StringBuilder(length);
		byte[] array = new byte[length];
		RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
		rNGCryptoServiceProvider.GetBytes(array);
		for (int i = 0; i < length; i++)
		{
			int index = array[i] % passwordChars62.Length;
			char value = passwordChars62[index];
			stringBuilder.Append(value);
		}
		return stringBuilder.ToString();
	}

	public static byte[] CreateSha256(string planeStr, string key)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(planeStr);
		byte[] bytes2 = Encoding.UTF8.GetBytes(key);
		HMACSHA256 hMACSHA = new HMACSHA256(bytes2);
		return hMACSHA.ComputeHash(bytes);
	}

	public static byte[] CreateSha256(byte[] data, string key)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		HMACSHA256 hMACSHA = new HMACSHA256(bytes);
		return hMACSHA.ComputeHash(data);
	}

	public static string ConvertStrX2FromBytes(byte[] data)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		foreach (byte b in data)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static string GetMacAddress()
	{
		string text = string.Empty;
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		if (allNetworkInterfaces != null)
		{
			NetworkInterface[] array = allNetworkInterfaces;
			foreach (NetworkInterface networkInterface in array)
			{
				PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
				byte[] array2 = null;
				if (physicalAddress != null)
				{
					array2 = physicalAddress.GetAddressBytes();
				}
				if (array2 != null && array2.Length == 6)
				{
					text += physicalAddress.ToString();
					break;
				}
			}
		}
		return text;
	}

	public static string CreateUUID()
	{
		StringBuilder stringBuilder = new StringBuilder(32);
		stringBuilder.Append(GetMacAddress());
		if (string.Empty == stringBuilder.ToString())
		{
			stringBuilder.Append(GeneratePassword36(12));
		}
		stringBuilder.Append(DateTime.Now.ToString("MMddHHmmssfff"));
		stringBuilder.Append(GeneratePassword36(40));
		byte[] array = Encoding.UTF8.GetBytes(stringBuilder.ToString());
		BitRevBytes(array, 7);
		stringBuilder.Length = 0;
		int num = array.Length / 4;
		int num2 = array.Length % 4;
		if (num2 != 0)
		{
			int num3 = 4 - num2;
			byte[] sourceArray = new byte[num3];
			int num4 = array.Length;
			Array.Resize(ref array, num4 + num3);
			Array.Copy(sourceArray, 0, array, num4, num3);
			num++;
		}
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append(Convert64StringFromInt32(BitConverter.ToInt32(array, i * 4)));
		}
		string empty = string.Empty;
		if (stringBuilder.Length < 64)
		{
			int length = 64 - stringBuilder.Length;
			stringBuilder.Append(GeneratePassword62(length));
			return stringBuilder.ToString();
		}
		return stringBuilder.ToString().Substring(0, 64);
	}

	public static string GetStringRight(string str, int len)
	{
		if (str == null)
		{
			return string.Empty;
		}
		if (str.Length <= len)
		{
			return str;
		}
		return str.Substring(str.Length - len, len);
	}

	public static string GetRemoveStringRight(string str, int len)
	{
		if (str == null)
		{
			return string.Empty;
		}
		if (str.Length <= len)
		{
			return string.Empty;
		}
		return str.Substring(0, str.Length - len);
	}

	public static string GetRemoveStringLeft(string str, string find, bool removeFind = true)
	{
		if (str == null)
		{
			return string.Empty;
		}
		int num = str.IndexOf(find);
		if (0 >= num)
		{
			return str;
		}
		num += (removeFind ? find.Length : 0);
		return str.Substring(num);
	}

	public static string GetRemoveStringRight(string str, string find, bool removeFind = false)
	{
		if (str == null)
		{
			return string.Empty;
		}
		int num = str.LastIndexOf(find);
		if (0 >= num)
		{
			return str;
		}
		num += ((!removeFind) ? find.Length : 0);
		return str.Substring(0, num);
	}

	public static byte[] EncryptAES(byte[] srcData, string pw = "illusion", string salt = "unityunity")
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] bytes = Encoding.UTF8.GetBytes(salt);
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pw, bytes);
		rfc2898DeriveBytes.IterationCount = 1000;
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] result = cryptoTransform.TransformFinalBlock(srcData, 0, srcData.Length);
		cryptoTransform.Dispose();
		return result;
	}

	public static byte[] DecryptAES(byte[] srcData, string pw = "illusion", string salt = "unityunity")
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 128;
		rijndaelManaged.BlockSize = 128;
		byte[] bytes = Encoding.UTF8.GetBytes(salt);
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(pw, bytes);
		rfc2898DeriveBytes.IterationCount = 1000;
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] result = cryptoTransform.TransformFinalBlock(srcData, 0, srcData.Length);
		cryptoTransform.Dispose();
		return result;
	}

	public static string GetRegistryInfoFrom(string keyName, string valueName, string baseKey = "HKEY_CURRENT_USER")
	{
		string empty = string.Empty;
		try
		{
			RegistryKey registryKey = null;
			if (baseKey == "HKEY_CURRENT_USER")
			{
				registryKey = Registry.CurrentUser.OpenSubKey(keyName);
			}
			else
			{
				if (!(baseKey == "HKEY_LOCAL_MACHINE"))
				{
					return null;
				}
				registryKey = Registry.LocalMachine.OpenSubKey(keyName);
			}
			if (registryKey == null)
			{
				return null;
			}
			empty = (string)registryKey.GetValue(valueName);
			registryKey.Close();
			return empty;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static bool CompareFile(string file1, string file2)
	{
		int num = 0;
		int num2 = 0;
		FileStream fileStream = null;
		FileStream fileStream2 = null;
		if (file1 == file2)
		{
			return true;
		}
		try
		{
			fileStream = new FileStream(file1, FileMode.Open);
		}
		catch (FileNotFoundException)
		{
			UnityEngine.Debug.LogError(file1 + " がない");
			return false;
		}
		try
		{
			fileStream2 = new FileStream(file2, FileMode.Open);
		}
		catch (FileNotFoundException)
		{
			fileStream.Close();
			UnityEngine.Debug.LogError(file2 + " がない");
			return false;
		}
		if (fileStream.Length != fileStream2.Length)
		{
			fileStream.Close();
			fileStream2.Close();
			return false;
		}
		do
		{
			num = fileStream.ReadByte();
			num2 = fileStream2.ReadByte();
		}
		while (num == num2 && num != -1);
		fileStream.Close();
		fileStream2.Close();
		if (num - num2 == 0)
		{
			return true;
		}
		return false;
	}
}
