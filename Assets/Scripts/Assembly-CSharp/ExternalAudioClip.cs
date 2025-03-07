using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExternalAudioClip
{
	public static readonly float RangeValue8Bit = 1f / Mathf.Pow(2f, 7f);

	public static readonly float RangeValue16Bit = 1f / Mathf.Pow(2f, 15f);

	public static readonly float RangeValue24Bit = 1f / Mathf.Pow(2f, 23f);

	public static readonly float RangeValue32Bit = 1f / Mathf.Pow(2f, 31f);

	public const int BaseConvertSamples = 20480;

	public static AudioClip Load(string path)
	{
		WaveHeader waveHeader = WaveHeader.ReadWaveHeader(path);
		if (waveHeader == null)
		{
			return null;
		}
		float[] array = CreateRangedRawData(path, waveHeader.TrueWavBufIndex, waveHeader.TrueSamples, waveHeader.Channels, waveHeader.BitPerSample);
		if (array.Length == 0)
		{
			return null;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		return CreateClip(fileNameWithoutExtension, array, waveHeader.TrueSamples, waveHeader.Channels, waveHeader.Frequency);
	}

	public static AudioClip CreateClip(string name, float[] rawData, int lengthSamples, int channels, int frequency)
	{
		AudioClip audioClip = AudioClip.Create(name, lengthSamples, channels, frequency, false);
		audioClip.SetData(rawData, 0);
		return audioClip;
	}

	public static float[] CreateRangedRawData(string path, int wavBufIndex, int samples, int channels, int bitPerSample)
	{
		byte[] array = File.ReadAllBytes(path);
		if (array.Length == 0)
		{
			return null;
		}
		return CreateRangedRawData(array, wavBufIndex, samples, channels, bitPerSample);
	}

	public static float[] CreateRangedRawData(byte[] data, int wavBufIndex, int samples, int channels, int bitPerSample)
	{
		float[] array = new float[samples * channels];
		int num = bitPerSample / 8;
		int num2 = wavBufIndex;
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ByteToFloat(data, num2, bitPerSample);
				num2 += num;
			}
			return array;
		}
		catch (Exception)
		{
			return Enumerable.Empty<float>().ToArray();
		}
	}

	private static float ByteToFloat(byte[] data, int index, int bitPerSample)
	{
		float num = 0f;
		switch (bitPerSample)
		{
		case 8:
			return (float)(data[index] - 128) * RangeValue8Bit;
		case 16:
		{
			short num2 = BitConverter.ToInt16(data, index);
			return (float)num2 * RangeValue16Bit;
		}
		default:
			throw new Exception(bitPerSample + "bit is not supported.");
		}
	}
}
