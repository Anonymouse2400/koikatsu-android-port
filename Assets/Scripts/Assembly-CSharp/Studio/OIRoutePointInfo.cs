using System;
using System.IO;

namespace Studio
{
	public class OIRoutePointInfo : ObjectInfo
	{
		public enum Connection
		{
			Line = 0,
			Curve = 1
		}

		public float speed = 2f;

		public StudioTween.EaseType easeType = StudioTween.EaseType.linear;

		public Connection connection;

		public OIRoutePointAidInfo aidInfo;

		public bool link;

		public override int kind
		{
			get
			{
				return 6;
			}
		}

		public string name { get; private set; }

		public int number
		{
			set
			{
				name = ((value != 0) ? string.Format("ポイント{0}", value) : "スタート");
			}
		}

		public override int[] kinds
		{
			get
			{
				return new int[2] { 6, 4 };
			}
		}

		public OIRoutePointInfo(int _key)
			: base(_key)
		{
			number = 0;
			speed = 2f;
			easeType = StudioTween.EaseType.linear;
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			_writer.Write(base.dicKey);
			base.changeAmount.Save(_writer);
			_writer.Write(speed);
			_writer.Write((int)easeType);
			_writer.Write((int)connection);
			aidInfo.Save(_writer, _version);
			_writer.Write(link);
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import, false);
			speed = _reader.ReadSingle();
			easeType = (StudioTween.EaseType)_reader.ReadInt32();
			if (_version.CompareTo(new Version(1, 0, 3)) == 0)
			{
				_reader.ReadBoolean();
			}
			if (_version.CompareTo(new Version(1, 0, 4, 1)) >= 0)
			{
				connection = (Connection)_reader.ReadInt32();
			}
			if (_version.CompareTo(new Version(1, 0, 4, 1)) >= 0)
			{
				if (aidInfo == null)
				{
					aidInfo = new OIRoutePointAidInfo(_import ? Studio.GetNewIndex() : (-1));
				}
				aidInfo.Load(_reader, _version, _import);
			}
			if (_version.CompareTo(new Version(1, 0, 4, 2)) >= 0)
			{
				link = _reader.ReadBoolean();
			}
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
		}
	}
}
