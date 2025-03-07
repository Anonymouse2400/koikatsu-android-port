using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class OICharInfo : ObjectInfo
	{
		public enum IKTargetEN
		{
			Body = 0,
			LeftShoulder = 1,
			LeftArmChain = 2,
			LeftHand = 3,
			RightShoulder = 4,
			RightArmChain = 5,
			RightHand = 6,
			LeftThigh = 7,
			LeftLegChain = 8,
			LeftFoot = 9,
			RightThigh = 10,
			RightLegChain = 11,
			RightFoot = 12
		}

		public enum KinematicMode
		{
			None = 0,
			FK = 1,
			IK = 2
		}

		public class AnimeInfo
		{
			public int group;

			public int category;

			public int no;

			public bool exist
			{
				get
				{
					return (group != -1) & (category != -1) & (no != -1);
				}
			}

			public void Set(int _group, int _category, int _no)
			{
				group = _group;
				category = _category;
				no = _no;
			}

			public void Copy(AnimeInfo _src)
			{
				group = _src.group;
				category = _src.category;
				no = _src.no;
			}
		}

		public KinematicMode kinematicMode;

		public AnimeInfo animeInfo = new AnimeInfo();

		public int[] handPtn = new int[2];

		public float mouthOpen;

		public bool lipSync = true;

		public bool enableIK;

		public bool[] activeIK = new bool[5] { true, true, true, true, true };

		public bool enableFK;

		public bool[] activeFK = new bool[7] { false, true, false, true, false, false, false };

		public bool[] expression = new bool[8];

		public float animeSpeed = 1f;

		public float animePattern;

		public bool animeOptionVisible = true;

		public bool isAnimeForceLoop;

		public VoiceCtrl voiceCtrl = new VoiceCtrl();

		public bool visibleSon;

		public float sonLength = 1f;

		public float[] animeOptionParam = new float[2];

		public float nipple;

		public byte[] siru = new byte[5];

		public bool visibleSimple;

		public Color simpleColor;

		public byte[] neckByteData;

		public byte[] eyesByteData;

		public float animeNormalizedTime;

		public Dictionary<int, TreeNodeObject.TreeState> dicAccessGroup;

		public Dictionary<int, TreeNodeObject.TreeState> dicAccessNo;

		public override int kind
		{
			get
			{
				return 0;
			}
		}

		public int sex { get; private set; }

		public ChaFileControl charFile { get; private set; }

		public Dictionary<int, OIBoneInfo> bones { get; private set; }

		public Dictionary<int, OIIKTargetInfo> ikTarget { get; private set; }

		public Dictionary<int, List<ObjectInfo>> child { get; private set; }

		public LookAtTargetInfo lookAtTarget { get; set; }

		public OICharInfo(ChaFileControl _charFile, int _key)
			: base(_key)
		{
			int num = 0;
			sex = ((_charFile != null) ? _charFile.parameter.sex : 0);
			charFile = _charFile;
			bones = new Dictionary<int, OIBoneInfo>();
			ikTarget = new Dictionary<int, OIIKTargetInfo>();
			child = new Dictionary<int, List<ObjectInfo>>();
			num = Singleton<Info>.Instance.accessoryPointNum;
			for (int i = 0; i < num; i++)
			{
				child[i] = new List<ObjectInfo>();
			}
			simpleColor = Color.blue;
			dicAccessGroup = new Dictionary<int, TreeNodeObject.TreeState>();
			dicAccessNo = new Dictionary<int, TreeNodeObject.TreeState>();
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			base.Save(_writer, _version);
			_writer.Write(sex);
			charFile.SaveCharaFile(_writer, false);
			int count = bones.Count;
			_writer.Write(count);
			foreach (KeyValuePair<int, OIBoneInfo> bone in bones)
			{
				int key = bone.Key;
				_writer.Write(key);
				bone.Value.Save(_writer, _version);
			}
			count = ikTarget.Count;
			_writer.Write(count);
			foreach (KeyValuePair<int, OIIKTargetInfo> item in ikTarget)
			{
				int key2 = item.Key;
				_writer.Write(key2);
				item.Value.Save(_writer, _version);
			}
			count = child.Count;
			_writer.Write(count);
			foreach (KeyValuePair<int, List<ObjectInfo>> item2 in child)
			{
				int key3 = item2.Key;
				_writer.Write(key3);
				count = item2.Value.Count;
				_writer.Write(count);
				for (int i = 0; i < count; i++)
				{
					item2.Value[i].Save(_writer, _version);
				}
			}
			_writer.Write((int)kinematicMode);
			_writer.Write(animeInfo.group);
			_writer.Write(animeInfo.category);
			_writer.Write(animeInfo.no);
			for (int j = 0; j < 2; j++)
			{
				_writer.Write(handPtn[j]);
			}
			_writer.Write(nipple);
			_writer.Write(siru);
			_writer.Write(mouthOpen);
			_writer.Write(lipSync);
			lookAtTarget.Save(_writer, _version);
			_writer.Write(enableIK);
			for (int k = 0; k < 5; k++)
			{
				_writer.Write(activeIK[k]);
			}
			_writer.Write(enableFK);
			for (int l = 0; l < 7; l++)
			{
				_writer.Write(activeFK[l]);
			}
			for (int m = 0; m < 8; m++)
			{
				_writer.Write(expression[m]);
			}
			_writer.Write(animeSpeed);
			_writer.Write(animePattern);
			_writer.Write(animeOptionVisible);
			_writer.Write(isAnimeForceLoop);
			voiceCtrl.Save(_writer, _version);
			_writer.Write(visibleSon);
			_writer.Write(sonLength);
			_writer.Write(visibleSimple);
			_writer.Write(JsonUtility.ToJson(simpleColor));
			_writer.Write(animeOptionParam[0]);
			_writer.Write(animeOptionParam[1]);
			_writer.Write(neckByteData.Length);
			_writer.Write(neckByteData);
			_writer.Write(eyesByteData.Length);
			_writer.Write(eyesByteData);
			_writer.Write(animeNormalizedTime);
			count = ((dicAccessGroup != null) ? dicAccessGroup.Count : 0);
			_writer.Write(count);
			if (count != 0)
			{
				foreach (KeyValuePair<int, TreeNodeObject.TreeState> item3 in dicAccessGroup)
				{
					_writer.Write(item3.Key);
					_writer.Write((int)item3.Value);
				}
			}
			count = ((dicAccessNo != null) ? dicAccessNo.Count : 0);
			_writer.Write(count);
			if (count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, TreeNodeObject.TreeState> item4 in dicAccessNo)
			{
				_writer.Write(item4.Key);
				_writer.Write((int)item4.Value);
			}
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import);
			sex = _reader.ReadInt32();
			charFile = new ChaFileControl();
			charFile.LoadCharaFile(_reader, true, false);
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int key = _reader.ReadInt32();
				bones[key] = new OIBoneInfo(_import ? Studio.GetNewIndex() : (-1));
				bones[key].Load(_reader, _version, _import);
			}
			num = _reader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				int key2 = _reader.ReadInt32();
				ikTarget[key2] = new OIIKTargetInfo(_import ? Studio.GetNewIndex() : (-1));
				ikTarget[key2].Load(_reader, _version, _import);
			}
			num = _reader.ReadInt32();
			for (int k = 0; k < num; k++)
			{
				int key3 = _reader.ReadInt32();
				ObjectInfoAssist.LoadChild(_reader, _version, child[key3], _import);
			}
			kinematicMode = (KinematicMode)_reader.ReadInt32();
			animeInfo.group = _reader.ReadInt32();
			animeInfo.category = _reader.ReadInt32();
			animeInfo.no = _reader.ReadInt32();
			for (int l = 0; l < 2; l++)
			{
				handPtn[l] = _reader.ReadInt32();
			}
			nipple = _reader.ReadSingle();
			siru = _reader.ReadBytes(5);
			mouthOpen = _reader.ReadSingle();
			lipSync = _reader.ReadBoolean();
			if (lookAtTarget == null)
			{
				lookAtTarget = new LookAtTargetInfo(_import ? Studio.GetNewIndex() : (-1));
			}
			lookAtTarget.Load(_reader, _version, _import);
			enableIK = _reader.ReadBoolean();
			for (int m = 0; m < 5; m++)
			{
				activeIK[m] = _reader.ReadBoolean();
			}
			enableFK = _reader.ReadBoolean();
			for (int n = 0; n < 7; n++)
			{
				activeFK[n] = _reader.ReadBoolean();
			}
			num = ((_version.CompareTo(new Version(0, 0, 9)) < 0) ? 4 : 8);
			for (int num2 = 0; num2 < num; num2++)
			{
				expression[num2] = _reader.ReadBoolean();
			}
			animeSpeed = _reader.ReadSingle();
			animePattern = _reader.ReadSingle();
			animeOptionVisible = _reader.ReadBoolean();
			isAnimeForceLoop = _reader.ReadBoolean();
			voiceCtrl.Load(_reader, _version);
			visibleSon = _reader.ReadBoolean();
			sonLength = _reader.ReadSingle();
			visibleSimple = _reader.ReadBoolean();
			simpleColor = JsonUtility.FromJson<Color>(_reader.ReadString());
			animeOptionParam[0] = _reader.ReadSingle();
			animeOptionParam[1] = _reader.ReadSingle();
			num = _reader.ReadInt32();
			neckByteData = _reader.ReadBytes(num);
			num = _reader.ReadInt32();
			eyesByteData = _reader.ReadBytes(num);
			animeNormalizedTime = _reader.ReadSingle();
			num = _reader.ReadInt32();
			if (num != 0)
			{
				dicAccessGroup = new Dictionary<int, TreeNodeObject.TreeState>();
			}
			for (int num3 = 0; num3 < num; num3++)
			{
				int key4 = _reader.ReadInt32();
				dicAccessGroup[key4] = (TreeNodeObject.TreeState)_reader.ReadInt32();
			}
			num = _reader.ReadInt32();
			if (num != 0)
			{
				dicAccessNo = new Dictionary<int, TreeNodeObject.TreeState>();
			}
			for (int num4 = 0; num4 < num; num4++)
			{
				int key5 = _reader.ReadInt32();
				dicAccessNo[key5] = (TreeNodeObject.TreeState)_reader.ReadInt32();
			}
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
			foreach (KeyValuePair<int, OIBoneInfo> bone in bones)
			{
				Studio.DeleteIndex(bone.Value.dicKey);
			}
			foreach (KeyValuePair<int, OIIKTargetInfo> item in ikTarget)
			{
				Studio.DeleteIndex(item.Value.dicKey);
			}
			foreach (KeyValuePair<int, List<ObjectInfo>> item2 in child)
			{
				int count = item2.Value.Count;
				for (int i = 0; i < count; i++)
				{
					item2.Value[i].DeleteKey();
				}
			}
			Studio.DeleteIndex(lookAtTarget.dicKey);
		}
	}
}
