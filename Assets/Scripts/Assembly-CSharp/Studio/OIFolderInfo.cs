using System;
using System.Collections.Generic;
using System.IO;

namespace Studio
{
	public class OIFolderInfo : ObjectInfo
	{
		public string name = string.Empty;

		public override int kind
		{
			get
			{
				return 3;
			}
		}

		public List<ObjectInfo> child { get; private set; }

		public OIFolderInfo(int _key)
			: base(_key)
		{
			name = "フォルダー";
			child = new List<ObjectInfo>();
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
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import);
			name = _reader.ReadString();
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
