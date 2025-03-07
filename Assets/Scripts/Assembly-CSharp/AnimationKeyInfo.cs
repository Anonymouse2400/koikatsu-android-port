using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AnimationKeyInfo
{
	public class AnmKeyInfo
	{
		public int no;

		public Vector3 pos = default(Vector3);

		public Vector3 rot = default(Vector3);

		public Vector3 scl = default(Vector3);

		public void Set(int _no, Vector3 _pos, Vector3 _rot, Vector3 _scl)
		{
			no = _no;
			pos = _pos;
			rot = _rot;
			scl = _scl;
		}

		public string GetInfoStr()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(no.ToString()).Append("\t");
			stringBuilder.Append(pos.ToString("f7")).Append("\t");
			stringBuilder.Append(rot.ToString("f7")).Append("\t");
			stringBuilder.Append(scl.ToString("f7"));
			return stringBuilder.ToString();
		}
	}

	private Dictionary<string, List<AnmKeyInfo>> dictInfo = new Dictionary<string, List<AnmKeyInfo>>();

	public bool CreateInfo(int start, int end, GameObject obj, string[] usename)
	{
		if (null == obj)
		{
			return false;
		}
		Animator component = obj.GetComponent<Animator>();
		if (null == component)
		{
			return false;
		}
		dictInfo.Clear();
		float num = 1f / (float)(end - start);
		int num2 = end - start + 1;
		for (int i = 0; i < num2; i++)
		{
			float normalizedTime = num * (float)i;
			component.Play(component.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, normalizedTime);
			component.Update(0f);
			CreateInfoLoop(i, obj.transform, usename);
		}
		return true;
	}

	private void CreateInfoLoop(int no, Transform tf, string[] usename)
	{
		if (null == tf)
		{
			return;
		}
		List<AnmKeyInfo> value = null;
		bool flag = false;
		if (usename != null && Array.IndexOf(usename, tf.name) == -1)
		{
			flag = true;
		}
		if (!flag)
		{
			if (!dictInfo.TryGetValue(tf.name, out value))
			{
				value = new List<AnmKeyInfo>();
				dictInfo[tf.name] = value;
			}
			AnmKeyInfo anmKeyInfo = new AnmKeyInfo();
			anmKeyInfo.Set(no, tf.localPosition, tf.localEulerAngles, tf.localScale);
			value.Add(anmKeyInfo);
		}
		for (int i = 0; i < tf.childCount; i++)
		{
			Transform child = tf.GetChild(i);
			CreateInfoLoop(no, child, usename);
		}
	}

	public bool GetInfo(string name, float rate, ref Vector3 value, byte type)
	{
		List<AnmKeyInfo> value2 = null;
		if (!dictInfo.TryGetValue(name, out value2))
		{
			return false;
		}
		switch (type)
		{
		case 0:
		{
			if (rate == 0f)
			{
				value = value2[0].pos;
				break;
			}
			if (rate == 1f)
			{
				value = value2[value2.Count - 1].pos;
				break;
			}
			float num5 = (float)(value2.Count - 1) * rate;
			int num6 = Mathf.FloorToInt(num5);
			float t3 = num5 - (float)num6;
			value = Vector3.Lerp(value2[num6].pos, value2[num6 + 1].pos, t3);
			break;
		}
		case 1:
		{
			if (rate == 0f)
			{
				value = value2[0].rot;
				break;
			}
			if (rate == 1f)
			{
				value = value2[value2.Count - 1].rot;
				break;
			}
			float num3 = (float)(value2.Count - 1) * rate;
			int num4 = Mathf.FloorToInt(num3);
			float t2 = num3 - (float)num4;
			value.x = Mathf.LerpAngle(value2[num4].rot.x, value2[num4 + 1].rot.x, t2);
			value.y = Mathf.LerpAngle(value2[num4].rot.y, value2[num4 + 1].rot.y, t2);
			value.z = Mathf.LerpAngle(value2[num4].rot.z, value2[num4 + 1].rot.z, t2);
			break;
		}
		default:
		{
			if (rate == 0f)
			{
				value = value2[0].scl;
				break;
			}
			if (rate == 1f)
			{
				value = value2[value2.Count - 1].scl;
				break;
			}
			float num = (float)(value2.Count - 1) * rate;
			int num2 = Mathf.FloorToInt(num);
			float t = num - (float)num2;
			value = Vector3.Lerp(value2[num2].scl, value2[num2 + 1].scl, t);
			break;
		}
		}
		return true;
	}

	public bool GetInfo(string name, float rate, ref Vector3[] value, bool[] flag)
	{
		if (value.Length != 3 || flag.Length != 3)
		{
			return false;
		}
		List<AnmKeyInfo> value2 = null;
		if (!dictInfo.TryGetValue(name, out value2))
		{
			return false;
		}
		if (flag[0])
		{
			if (rate == 0f)
			{
				value[0] = value2[0].pos;
			}
			else if (rate == 1f)
			{
				value[0] = value2[value2.Count - 1].pos;
			}
			else
			{
				float num = (float)(value2.Count - 1) * rate;
				int num2 = Mathf.FloorToInt(num);
				float t = num - (float)num2;
				value[0] = Vector3.Lerp(value2[num2].pos, value2[num2 + 1].pos, t);
			}
		}
		if (flag[1])
		{
			if (rate == 0f)
			{
				value[1] = value2[0].rot;
			}
			else if (rate == 1f)
			{
				value[1] = value2[value2.Count - 1].rot;
			}
			else
			{
				float num3 = (float)(value2.Count - 1) * rate;
				int num4 = Mathf.FloorToInt(num3);
				float t2 = num3 - (float)num4;
				value[1].x = Mathf.LerpAngle(value2[num4].rot.x, value2[num4 + 1].rot.x, t2);
				value[1].y = Mathf.LerpAngle(value2[num4].rot.y, value2[num4 + 1].rot.y, t2);
				value[1].z = Mathf.LerpAngle(value2[num4].rot.z, value2[num4 + 1].rot.z, t2);
			}
		}
		if (flag[2])
		{
			if (rate == 0f)
			{
				value[2] = value2[0].scl;
			}
			else if (rate == 1f)
			{
				value[2] = value2[value2.Count - 1].scl;
			}
			else
			{
				float num5 = (float)(value2.Count - 1) * rate;
				int num6 = Mathf.FloorToInt(num5);
				float t3 = num5 - (float)num6;
				value[2] = Vector3.Lerp(value2[num6].scl, value2[num6 + 1].scl, t3);
			}
		}
		return true;
	}

	public void SaveInfo(string filepath)
	{
		using (FileStream output = new FileStream(filepath, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				int count = dictInfo.Count;
				binaryWriter.Write(count);
				foreach (KeyValuePair<string, List<AnmKeyInfo>> item in dictInfo)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value.Count);
					for (int i = 0; i < item.Value.Count; i++)
					{
						binaryWriter.Write(item.Value[i].no);
						binaryWriter.Write(item.Value[i].pos.x);
						binaryWriter.Write(item.Value[i].pos.y);
						binaryWriter.Write(item.Value[i].pos.z);
						binaryWriter.Write(item.Value[i].rot.x);
						binaryWriter.Write(item.Value[i].rot.y);
						binaryWriter.Write(item.Value[i].rot.z);
						binaryWriter.Write(item.Value[i].scl.x);
						binaryWriter.Write(item.Value[i].scl.y);
						binaryWriter.Write(item.Value[i].scl.z);
					}
				}
			}
		}
	}

	public void LoadInfo(string filePath)
	{
		using (FileStream st = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		{
			LoadInfo(st);
		}
	}

	public void LoadInfo(string assetBundleName, string assetName, Action<string, string> funcAssetBundleEntry = null)
	{
		if (!AssetBundleCheck.IsFile(assetBundleName, assetName))
		{
			string text = "読み込みエラー\r\nassetBundleName：" + assetBundleName + "\tassetName：" + assetName;
			return;
		}
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(TextAsset));
		if (assetBundleLoadAssetOperation == null)
		{
			string text2 = "読み込みエラー\r\nassetName：" + assetName;
			return;
		}
		TextAsset asset = assetBundleLoadAssetOperation.GetAsset<TextAsset>();
		if (!(null == asset))
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(asset.bytes, 0, asset.bytes.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				LoadInfo(memoryStream);
			}
			if (funcAssetBundleEntry == null)
			{
				AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
			}
			else
			{
				funcAssetBundleEntry(assetBundleName, string.Empty);
			}
		}
	}

	public void LoadInfo(Stream st)
	{
		using (BinaryReader binaryReader = new BinaryReader(st))
		{
			int num = binaryReader.ReadInt32();
			dictInfo.Clear();
			for (int i = 0; i < num; i++)
			{
				List<AnmKeyInfo> list = new List<AnmKeyInfo>();
				string key = binaryReader.ReadString();
				dictInfo[key] = list;
				int num2 = binaryReader.ReadInt32();
				for (int j = 0; j < num2; j++)
				{
					AnmKeyInfo anmKeyInfo = new AnmKeyInfo();
					anmKeyInfo.no = binaryReader.ReadInt32();
					anmKeyInfo.pos.x = binaryReader.ReadSingle();
					anmKeyInfo.pos.y = binaryReader.ReadSingle();
					anmKeyInfo.pos.z = binaryReader.ReadSingle();
					anmKeyInfo.rot.x = binaryReader.ReadSingle();
					anmKeyInfo.rot.y = binaryReader.ReadSingle();
					anmKeyInfo.rot.z = binaryReader.ReadSingle();
					anmKeyInfo.scl.x = binaryReader.ReadSingle();
					anmKeyInfo.scl.y = binaryReader.ReadSingle();
					anmKeyInfo.scl.z = binaryReader.ReadSingle();
					list.Add(anmKeyInfo);
				}
			}
		}
	}

	public void OutputText(string outputPath)
	{
		StringBuilder stringBuilder = new StringBuilder(2048);
		stringBuilder.Length = 0;
		foreach (KeyValuePair<string, List<AnmKeyInfo>> item in dictInfo)
		{
			for (int i = 0; i < item.Value.Count; i++)
			{
				stringBuilder.Append(item.Key).Append("\t");
				stringBuilder.Append(item.Value[i].GetInfoStr());
				stringBuilder.Append("\n");
			}
		}
		using (FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
		{
			using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Write("\n");
			}
		}
	}
}
