using System;
using System.IO;
using System.Text;

namespace SaveAssist
{
	public class BlockHeader
	{
		private const int tagSize = 128;

		public string tagName = string.Empty;

		public byte[] tag;

		public int version;

		public long pos;

		public long size;

		public void SetHeader(string _tagName, int _version, long _pos, long _size)
		{
			tagName = _tagName;
			tag = ChangeStringToByte(tagName);
			version = _version;
			pos = _pos;
			size = _size;
		}

		public bool SaveHeader(BinaryWriter writer)
		{
			if (tag == null)
			{
				return false;
			}
			writer.Write(tag);
			writer.Write(version);
			writer.Write(pos);
			writer.Write(size);
			return true;
		}

		public bool LoadHeader(BinaryReader reader)
		{
			tag = reader.ReadBytes(128);
			tagName = ChangeByteToString(tag);
			version = reader.ReadInt32();
			pos = reader.ReadInt64();
			size = reader.ReadInt64();
			return true;
		}

		public static int GetBlockHeaderSize()
		{
			return 148;
		}

		public static byte[] ChangeStringToByte(string _tagName)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(_tagName);
			if (bytes.GetLength(0) > 128)
			{
				return null;
			}
			byte[] array = new byte[128];
			Buffer.BlockCopy(bytes, 0, array, 0, bytes.GetLength(0));
			return array;
		}

		public static string ChangeByteToString(byte[] _tag)
		{
			return Encoding.UTF8.GetString(_tag);
		}
	}
}
