using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class OIItemInfo : ObjectInfo
	{
		public float animeSpeed = 1f;

		public Color[] color;

		public PatternInfo[] pattern;

		public float alpha = 1f;

		public Color lineColor;

		public float lineWidth = 1f;

		public Color emissionColor;

		public float emissionPower;

		public float lightCancel;

		public PatternInfo panel;

		public bool enableFK;

		public Dictionary<string, OIBoneInfo> bones;

		public bool enableDynamicBone = true;

		public float animeNormalizedTime;

		public override int kind
		{
			get
			{
				return 1;
			}
		}

		public int group { get; private set; }

		public int category { get; private set; }

		public int no { get; private set; }

		public List<ObjectInfo> child { get; private set; }

		public OIItemInfo(int _group, int _category, int _no, int _key)
			: base(_key)
		{
			group = _group;
			category = _category;
			no = _no;
			child = new List<ObjectInfo>();
			color = new Color[8]
			{
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white,
				Color.white
			};
			lineColor = Utility.ConvertColor(128, 128, 128);
			emissionColor = Utility.ConvertColor(255, 255, 255);
			pattern = new PatternInfo[3];
			for (int i = 0; i < 3; i++)
			{
				pattern[i] = new PatternInfo();
			}
			panel = new PatternInfo();
			bones = new Dictionary<string, OIBoneInfo>();
			animeNormalizedTime = 0f;
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			base.Save(_writer, _version);
			_writer.Write(group);
			_writer.Write(category);
			_writer.Write(no);
			_writer.Write(animeSpeed);
			for (int i = 0; i < 8; i++)
			{
				_writer.Write(JsonUtility.ToJson(color[i]));
			}
			for (int j = 0; j < 3; j++)
			{
				pattern[j].Save(_writer, _version);
			}
			_writer.Write(alpha);
			_writer.Write(JsonUtility.ToJson(lineColor));
			_writer.Write(lineWidth);
			_writer.Write(JsonUtility.ToJson(emissionColor));
			_writer.Write(emissionPower);
			_writer.Write(lightCancel);
			panel.Save(_writer, _version);
			_writer.Write(enableFK);
			_writer.Write(bones.Count);
			foreach (KeyValuePair<string, OIBoneInfo> bone in bones)
			{
				_writer.Write(bone.Key);
				bone.Value.Save(_writer, _version);
			}
			_writer.Write(enableDynamicBone);
			_writer.Write(animeNormalizedTime);
			int count = child.Count;
			_writer.Write(count);
			for (int k = 0; k < count; k++)
			{
				child[k].Save(_writer, _version);
			}
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import);
			group = _reader.ReadInt32();
			category = _reader.ReadInt32();
			no = _reader.ReadInt32();
			animeSpeed = _reader.ReadSingle();
			if (_version.CompareTo(new Version(0, 0, 3)) >= 0)
			{
				for (int i = 0; i < 8; i++)
				{
					color[i] = JsonUtility.FromJson<Color>(_reader.ReadString());
				}
			}
			else
			{
				for (int j = 0; j < 7; j++)
				{
					color[j] = JsonUtility.FromJson<Color>(_reader.ReadString());
				}
			}
			for (int k = 0; k < 3; k++)
			{
				pattern[k].Load(_reader, _version);
			}
			alpha = _reader.ReadSingle();
			if (_version.CompareTo(new Version(0, 0, 4)) >= 0)
			{
				lineColor = JsonUtility.FromJson<Color>(_reader.ReadString());
				lineWidth = _reader.ReadSingle();
			}
			if (_version.CompareTo(new Version(0, 0, 7)) >= 0)
			{
				emissionColor = JsonUtility.FromJson<Color>(_reader.ReadString());
				emissionPower = _reader.ReadSingle();
				lightCancel = _reader.ReadSingle();
			}
			if (_version.CompareTo(new Version(0, 0, 6)) >= 0)
			{
				panel.Load(_reader, _version);
			}
			enableFK = _reader.ReadBoolean();
			int num = _reader.ReadInt32();
			for (int l = 0; l < num; l++)
			{
				string key = _reader.ReadString();
				bones[key] = new OIBoneInfo(_import ? Studio.GetNewIndex() : (-1));
				bones[key].Load(_reader, _version, _import);
			}
			if (_version.CompareTo(new Version(1, 0, 1)) >= 0)
			{
				enableDynamicBone = _reader.ReadBoolean();
			}
			animeNormalizedTime = _reader.ReadSingle();
			ObjectInfoAssist.LoadChild(_reader, _version, child, _import);
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
			int count = child.Count;
			for (int i = 0; i < count; i++)
			{
				child[i].DeleteKey();
			}
		}
	}
}
