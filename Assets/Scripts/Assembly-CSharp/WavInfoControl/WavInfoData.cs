using System;
using System.IO;
using UnityEngine;

namespace WavInfoControl
{
	public class WavInfoData
	{
		public float[] value;

		public float span { get; private set; }

		public WavInfoData()
		{
			value = null;
			span = 0.01f;
		}

		public float GetValue(float time)
		{
			if (value == null || value.Length == 0)
			{
				return 0f;
			}
			int num = (int)Math.Truncate(time * 100f);
			if (value.Length <= num + 1)
			{
				return 0f;
			}
			float t = time * 100f % 1f;
			return Mathf.Lerp(value[num], value[num + 1], t);
		}

		public bool Save(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			try
			{
				using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(output))
					{
						int num = value.Length;
						binaryWriter.Write(num);
						float[] array = value;
						foreach (float num2 in array)
						{
							binaryWriter.Write(num2);
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool Load(TextAsset ta)
		{
			if (null == ta)
			{
				return false;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(ta.bytes, 0, ta.bytes.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num = binaryReader.ReadInt32();
					value = new float[num];
					for (int i = 0; i < num; i++)
					{
						value[i] = binaryReader.ReadSingle();
					}
				}
			}
			return true;
		}

		public bool Load(string path)
		{
			if (!File.Exists(path))
			{
				return false;
			}
			try
			{
				using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(input))
					{
						int num = binaryReader.ReadInt32();
						value = new float[num];
						for (int i = 0; i < num; i++)
						{
							value[i] = binaryReader.ReadSingle();
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}
	}
}
