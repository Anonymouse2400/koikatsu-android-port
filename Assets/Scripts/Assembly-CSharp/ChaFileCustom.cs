using System;
using System.IO;
using MessagePack;

public class ChaFileCustom : ChaFileAssist
{
	public static readonly string BlockName = "Custom";

	public ChaFileFace face;

	public ChaFileBody body;

	public ChaFileHair hair;

	public ChaFileCustom()
	{
		MemberInit();
	}

	public void MemberInit()
	{
		face = new ChaFileFace();
		body = new ChaFileBody();
		hair = new ChaFileHair();
	}

	public byte[] SaveBytes()
	{
		byte[] array = MessagePackSerializer.Serialize(face);
		byte[] array2 = MessagePackSerializer.Serialize(body);
		byte[] array3 = MessagePackSerializer.Serialize(hair);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(array.Length);
				binaryWriter.Write(array);
				binaryWriter.Write(array2.Length);
				binaryWriter.Write(array2);
				binaryWriter.Write(array3.Length);
				binaryWriter.Write(array3);
				return memoryStream.ToArray();
			}
		}
	}

	public bool LoadBytes(byte[] data, Version ver)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int count = binaryReader.ReadInt32();
				byte[] bytes = binaryReader.ReadBytes(count);
				face = MessagePackSerializer.Deserialize<ChaFileFace>(bytes);
				count = binaryReader.ReadInt32();
				bytes = binaryReader.ReadBytes(count);
				body = MessagePackSerializer.Deserialize<ChaFileBody>(bytes);
				count = binaryReader.ReadInt32();
				bytes = binaryReader.ReadBytes(count);
				hair = MessagePackSerializer.Deserialize<ChaFileHair>(bytes);
				face.ComplementWithVersion();
				body.ComplementWithVersion();
				hair.ComplementWithVersion();
				return true;
			}
		}
	}

	protected void SaveFace(string path)
	{
		SaveFileAssist(path, face);
	}

	protected void LoadFace(string path)
	{
		LoadFileAssist(path, face);
		face.ComplementWithVersion();
	}

	protected void SaveBody(string path)
	{
		SaveFileAssist(path, body);
	}

	protected void LoadBody(string path)
	{
		LoadFileAssist(path, body);
		body.ComplementWithVersion();
	}

	protected void SaveHair(string path)
	{
		SaveFileAssist(path, hair);
	}

	protected void LoadHair(string path)
	{
		LoadFileAssist(path, hair);
		hair.ComplementWithVersion();
	}

	public int GetBustSizeKind()
	{
		int result = 1;
		float num = body.shapeValueBody[4];
		if (0.33f >= num)
		{
			result = 0;
		}
		else if (0.66f <= num)
		{
			result = 2;
		}
		return result;
	}

	public int GetHeightKind()
	{
		int result = 1;
		float num = body.shapeValueBody[0];
		if (0.33f >= num)
		{
			result = 0;
		}
		else if (0.66f <= num)
		{
			result = 2;
		}
		return result;
	}
}
