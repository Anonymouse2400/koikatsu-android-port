using System;
using System.IO;

namespace Studio
{
	public abstract class ObjectInfo
	{
		public int dicKey { get; private set; }

		public abstract int kind { get; }

		public ChangeAmount changeAmount { get; protected set; }

		public TreeNodeObject.TreeState treeState { get; set; }

		public bool visible { get; set; }

		public virtual int[] kinds
		{
			get
			{
				return new int[1] { kind };
			}
		}

		public ObjectInfo(int _key)
		{
			dicKey = _key;
			changeAmount = new ChangeAmount();
			treeState = TreeNodeObject.TreeState.Close;
			visible = true;
			if (dicKey != -1)
			{
				Studio.AddChangeAmount(dicKey, changeAmount);
			}
		}

		public virtual void Save(BinaryWriter _writer, Version _version)
		{
			_writer.Write(kind);
			_writer.Write(dicKey);
			changeAmount.Save(_writer);
			_writer.Write((int)treeState);
			_writer.Write(visible);
		}

		public virtual void Load(BinaryReader _reader, Version _version, bool _import, bool _other = true)
		{
			if (!_import)
			{
				dicKey = Studio.SetNewIndex(_reader.ReadInt32());
			}
			else
			{
				_reader.ReadInt32();
			}
			changeAmount.Load(_reader);
			if (dicKey != -1 && !_import)
			{
				Studio.AddChangeAmount(dicKey, changeAmount);
			}
			if (_other)
			{
				treeState = (TreeNodeObject.TreeState)_reader.ReadInt32();
			}
			if (_other)
			{
				visible = _reader.ReadBoolean();
			}
		}

		public abstract void DeleteKey();
	}
}
