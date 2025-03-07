using System.IO;
using System.Text;

public class WaveHeader
{
	public int FileSize { get; private set; }

	public string RIFF { get; private set; }

	public int Size { get; private set; }

	public string WAVE { get; private set; }

	public string FMT { get; private set; }

	public int FmtChunkSize { get; private set; }

	public short FormatId { get; private set; }

	public short Channels { get; private set; }

	public int Frequency { get; private set; }

	public int DataSpeed { get; private set; }

	public short BlockSize { get; private set; }

	public short BitPerSample { get; private set; }

	public string DATA { get; private set; }

	public int TrueWavBufSize { get; private set; }

	public int TrueWavBufIndex { get; private set; }

	public int TrueSamples { get; private set; }

	public static WaveHeader ReadWaveHeader(string path)
	{
		using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			return ReadWaveHeader(stream);
		}
	}

	public static WaveHeader ReadWaveHeader(Stream stream)
	{
		if (stream == null)
		{
			return null;
		}
		using (BinaryReader reader = new BinaryReader(stream))
		{
			return ReadWaveHeader(reader);
		}
	}

	public static WaveHeader ReadWaveHeader(BinaryReader reader)
	{
		WaveHeader waveHeader = new WaveHeader();
		waveHeader.RIFF = Encoding.ASCII.GetString(reader.ReadBytes(4));
		waveHeader.Size = reader.ReadInt32();
		waveHeader.WAVE = Encoding.ASCII.GetString(reader.ReadBytes(4));
		if (waveHeader.RIFF.ToUpper() != "RIFF" || waveHeader.WAVE.ToUpper() != "WAVE")
		{
			return null;
		}
		while (waveHeader.FMT == null || waveHeader.FMT.ToLower() != "fmt")
		{
			waveHeader.FMT = Encoding.ASCII.GetString(reader.ReadBytes(4));
			if (waveHeader.FMT.ToLower().Trim() == "fmt")
			{
				break;
			}
			uint num = reader.ReadUInt32();
			reader.BaseStream.Seek(num, SeekOrigin.Current);
		}
		waveHeader.FmtChunkSize = reader.ReadInt32();
		waveHeader.FormatId = reader.ReadInt16();
		waveHeader.Channels = reader.ReadInt16();
		waveHeader.Frequency = reader.ReadInt32();
		waveHeader.DataSpeed = reader.ReadInt32();
		waveHeader.BlockSize = reader.ReadInt16();
		waveHeader.BitPerSample = reader.ReadInt16();
		reader.BaseStream.Seek(waveHeader.FmtChunkSize - 16, SeekOrigin.Current);
		while (waveHeader.DATA == null || waveHeader.DATA.ToLower() != "data")
		{
			waveHeader.DATA = Encoding.ASCII.GetString(reader.ReadBytes(4));
			if (waveHeader.DATA.ToLower() == "data")
			{
				break;
			}
			uint num2 = reader.ReadUInt32();
			reader.BaseStream.Seek(num2, SeekOrigin.Current);
		}
		waveHeader.TrueWavBufSize = reader.ReadInt32();
		waveHeader.TrueSamples = waveHeader.TrueWavBufSize / (waveHeader.BitPerSample / 8) / waveHeader.Channels;
		waveHeader.TrueWavBufIndex = (int)reader.BaseStream.Position;
		reader.BaseStream.Seek(0L, SeekOrigin.Begin);
		return waveHeader;
	}
}
