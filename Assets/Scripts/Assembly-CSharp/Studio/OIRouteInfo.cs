using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Studio
{
	public class OIRouteInfo : ObjectInfo
	{
		public enum Orient
		{
			None = 0,
			XY = 1,
			Y = 2
		}

		public enum Connection
		{
			Line = 0,
			Curve = 1
		}

		public string name = string.Empty;

		public bool active;

		public bool loop = true;

		public bool visibleLine = true;

		public Orient orient;

		public Color color = Color.blue;

		public override int kind
		{
			get
			{
				return 4;
			}
		}

		public List<ObjectInfo> child { get; private set; }

		public List<OIRoutePointInfo> route { get; private set; }

		public OIRouteInfo(int _key)
			: base(_key)
		{
			name = "ルート";
			base.treeState = TreeNodeObject.TreeState.Open;
			child = new List<ObjectInfo>();
			route = new List<OIRoutePointInfo>();
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			base.Save(_writer, _version);
			_writer.Write(name);
			int count = child.Count;
			_writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				child[i].Save(_writer, _version);
			}
			count = route.Count;
			_writer.Write(count);
			for (int j = 0; j < count; j++)
			{
				route[j].Save(_writer, _version);
			}
			_writer.Write(active);
			_writer.Write(loop);
			_writer.Write(visibleLine);
			_writer.Write((int)orient);
			_writer.Write(JsonUtility.ToJson(color));
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import);
			name = _reader.ReadString();
			ObjectInfoAssist.LoadChild(_reader, _version, child, _import);
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				OIRoutePointInfo oIRoutePointInfo = new OIRoutePointInfo(-1);
				oIRoutePointInfo.Load(_reader, _version, false);
				route.Add(oIRoutePointInfo);
			}
			if (_version.CompareTo(new Version(1, 0, 3)) >= 0)
			{
				active = _reader.ReadBoolean();
				loop = _reader.ReadBoolean();
				visibleLine = _reader.ReadBoolean();
			}
			if (_version.CompareTo(new Version(1, 0, 4)) >= 0)
			{
				orient = (Orient)_reader.ReadInt32();
			}
			if (_version.CompareTo(new Version(1, 0, 4, 1)) >= 0)
			{
				color = JsonUtility.FromJson<Color>(_reader.ReadString());
			}
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
			foreach (ObjectInfo item in child)
			{
				item.DeleteKey();
			}
			foreach (OIRoutePointInfo item2 in route)
			{
				item2.DeleteKey();
			}
		}
	}
}
