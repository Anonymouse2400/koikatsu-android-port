using System;
using System.IO;

namespace Studio
{
	public class LookAtTargetInfo : ObjectInfo
	{
		public override int kind
		{
			get
			{
				return -1;
			}
		}

		public LookAtTargetInfo(int _key)
			: base(_key)
		{
		}

		public override void Save(BinaryWriter _writer, Version _version)
		{
			_writer.Write(base.dicKey);
			base.changeAmount.Save(_writer);
		}

		public override void Load(BinaryReader _reader, Version _version, bool _import, bool _tree = true)
		{
			base.Load(_reader, _version, _import, false);
		}

		public override void DeleteKey()
		{
			Studio.DeleteIndex(base.dicKey);
		}
	}
}
