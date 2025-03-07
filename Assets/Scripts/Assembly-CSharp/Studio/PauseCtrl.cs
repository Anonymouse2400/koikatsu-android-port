using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Studio
{
	public class PauseCtrl
	{
		public class FileInfo
		{
			public int group = -1;

			public int category = -1;

			public int no = -1;

			public float normalizedTime;

			public bool enableIK;

			public bool[] activeIK = new bool[5] { true, true, true, true, true };

			public Dictionary<int, ChangeAmount> dicIK = new Dictionary<int, ChangeAmount>();

			public bool enableFK;

			public bool[] activeFK = new bool[7] { false, true, false, true, false, false, false };

			public Dictionary<int, ChangeAmount> dicFK = new Dictionary<int, ChangeAmount>();

			public bool[] expression = new bool[4] { true, true, true, true };

			public FileInfo(OCIChar _char = null)
			{
				if (_char == null)
				{
					return;
				}
				group = _char.oiCharInfo.animeInfo.group;
				category = _char.oiCharInfo.animeInfo.category;
				no = _char.oiCharInfo.animeInfo.no;
				normalizedTime = _char.charAnimeCtrl.normalizedTime;
				enableIK = _char.oiCharInfo.enableIK;
				for (int i = 0; i < activeIK.Length; i++)
				{
					activeIK[i] = _char.oiCharInfo.activeIK[i];
				}
				foreach (KeyValuePair<int, OIIKTargetInfo> item in _char.oiCharInfo.ikTarget)
				{
					dicIK.Add(item.Key, item.Value.changeAmount.Clone());
				}
				enableFK = _char.oiCharInfo.enableFK;
				for (int j = 0; j < activeFK.Length; j++)
				{
					activeFK[j] = _char.oiCharInfo.activeFK[j];
				}
				OIBoneInfo.BoneGroup fkGroup = (OIBoneInfo.BoneGroup)895;
				foreach (KeyValuePair<int, OIBoneInfo> item2 in _char.oiCharInfo.bones.Where((KeyValuePair<int, OIBoneInfo> b) => (fkGroup & b.Value.group) != 0))
				{
					dicFK.Add(item2.Key, item2.Value.changeAmount.Clone());
				}
				for (int k = 0; k < expression.Length; k++)
				{
					expression[k] = _char.oiCharInfo.expression[k];
				}
			}

			public void Apply(OCIChar _char)
			{
				_char.LoadAnime(group, category, no, normalizedTime);
				for (int i = 0; i < activeIK.Length; i++)
				{
					_char.ActiveIK((OIBoneInfo.BoneGroup)(1 << i), activeIK[i]);
				}
				_char.ActiveKinematicMode(OICharInfo.KinematicMode.IK, enableIK, true);
				foreach (KeyValuePair<int, ChangeAmount> item in dicIK)
				{
					_char.oiCharInfo.ikTarget[item.Key].changeAmount.Copy(item.Value);
				}
				for (int j = 0; j < activeFK.Length; j++)
				{
					_char.ActiveFK(FKCtrl.parts[j], activeFK[j]);
				}
				_char.ActiveKinematicMode(OICharInfo.KinematicMode.FK, enableFK, true);
				foreach (KeyValuePair<int, ChangeAmount> item2 in dicFK)
				{
					_char.oiCharInfo.bones[item2.Key].changeAmount.Copy(item2.Value);
				}
				for (int k = 0; k < expression.Length; k++)
				{
					_char.EnableExpressionCategory(k, expression[k]);
				}
			}

			public void Save(BinaryWriter _writer)
			{
				_writer.Write(group);
				_writer.Write(category);
				_writer.Write(no);
				_writer.Write(normalizedTime);
				_writer.Write(enableIK);
				for (int i = 0; i < activeIK.Length; i++)
				{
					_writer.Write(activeIK[i]);
				}
				_writer.Write(dicIK.Count);
				foreach (KeyValuePair<int, ChangeAmount> item in dicIK)
				{
					_writer.Write(item.Key);
					item.Value.Save(_writer);
				}
				_writer.Write(enableFK);
				for (int j = 0; j < activeFK.Length; j++)
				{
					_writer.Write(activeFK[j]);
				}
				_writer.Write(dicFK.Count);
				foreach (KeyValuePair<int, ChangeAmount> item2 in dicFK)
				{
					_writer.Write(item2.Key);
					item2.Value.Save(_writer);
				}
				for (int k = 0; k < expression.Length; k++)
				{
					_writer.Write(expression[k]);
				}
			}

			public void Load(BinaryReader _reader, int _ver)
			{
				group = _reader.ReadInt32();
				category = _reader.ReadInt32();
				no = _reader.ReadInt32();
				if (_ver >= 101)
				{
					normalizedTime = _reader.ReadSingle();
				}
				enableIK = _reader.ReadBoolean();
				for (int i = 0; i < activeIK.Length; i++)
				{
					activeIK[i] = _reader.ReadBoolean();
				}
				int num = _reader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					int key = _reader.ReadInt32();
					ChangeAmount changeAmount = new ChangeAmount();
					changeAmount.Load(_reader);
					try
					{
						dicIK.Add(key, changeAmount);
					}
					catch (Exception)
					{
					}
				}
				enableFK = _reader.ReadBoolean();
				for (int k = 0; k < activeFK.Length; k++)
				{
					activeFK[k] = _reader.ReadBoolean();
				}
				num = _reader.ReadInt32();
				for (int l = 0; l < num; l++)
				{
					int key2 = _reader.ReadInt32();
					ChangeAmount changeAmount2 = new ChangeAmount();
					changeAmount2.Load(_reader);
					dicFK.Add(key2, changeAmount2);
				}
				for (int m = 0; m < expression.Length; m++)
				{
					expression[m] = _reader.ReadBoolean();
				}
			}
		}

		public const string savePath = "studio/pose";

		public const string saveExtension = ".dat";

		public const string saveIdentifyingCode = "【pose】";

		public const int saveVersion = 101;

		public static void Save(OCIChar _ociChar, string _name)
		{
			string path = UserData.Create("studio/pose") + Utility.GetCurrentTime() + ".dat";
			FileInfo fileInfo = new FileInfo(_ociChar);
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write("【pose】");
					binaryWriter.Write(101);
					binaryWriter.Write(_ociChar.oiCharInfo.sex);
					binaryWriter.Write(_name);
					fileInfo.Save(binaryWriter);
				}
			}
		}

		public static bool Load(OCIChar _ociChar, string _path)
		{
			FileInfo fileInfo = new FileInfo();
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【pose】") != 0)
					{
						return false;
					}
					int ver = binaryReader.ReadInt32();
					binaryReader.ReadInt32();
					binaryReader.ReadString();
					fileInfo.Load(binaryReader, ver);
				}
			}
			fileInfo.Apply(_ociChar);
			return true;
		}

		public static bool CheckIdentifyingCode(string _path, int _sex)
		{
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【pose】") != 0)
					{
						return false;
					}
					binaryReader.ReadInt32();
					if (_sex != binaryReader.ReadInt32())
					{
						return false;
					}
				}
			}
			return true;
		}

		public static string LoadName(string _path)
		{
			using (FileStream input = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					if (string.Compare(binaryReader.ReadString(), "【pose】") != 0)
					{
						return string.Empty;
					}
					binaryReader.ReadInt32();
					binaryReader.ReadInt32();
					return binaryReader.ReadString();
				}
			}
		}
	}
}
