using System;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class OILightInfo : ObjectInfo
	{
		public Color color;

		public float intensity;

		public float range;

		public float spotAngle;

		public bool shadow;

		public bool enable;

		public bool drawTarget;

		public override int kind
		{
			get
			{
				return 2;
			}
		}

		public int no { get; private set; }

		public OILightInfo(int _no, int _key)
			: base(_key)
		{
			no = _no;
			color = Color.white;
			intensity = 1f;
			range = 10f;
			spotAngle = 30f;
			shadow = true;
			enable = true;
			drawTarget = true;
			Studio.AddLight();
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			base.Save(_writer, _version);
			_writer.Write(no);
			Utility.SaveColor(_writer, color);
			_writer.Write(intensity);
			_writer.Write(range);
			_writer.Write(spotAngle);
			_writer.Write(shadow);
			_writer.Write(enable);
			_writer.Write(drawTarget);
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import);
			no = _reader.ReadInt32();
			color = Utility.LoadColor(_reader);
			intensity = _reader.ReadSingle();
			range = _reader.ReadSingle();
			spotAngle = _reader.ReadSingle();
			shadow = _reader.ReadBoolean();
			enable = _reader.ReadBoolean();
			drawTarget = _reader.ReadBoolean();
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
			Studio.DeleteLight();
		}
	}
}
