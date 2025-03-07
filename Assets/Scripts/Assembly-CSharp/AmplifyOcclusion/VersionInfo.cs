using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	[Serializable]
	public class VersionInfo
	{
		public const byte Major = 1;

		public const byte Minor = 2;

		public const byte Release = 1;

		private static string StageSuffix = "_dev003";

		[SerializeField]
		private int m_major;

		[SerializeField]
		private int m_minor;

		[SerializeField]
		private int m_release;

		public int Number
		{
			get
			{
				return m_major * 100 + m_minor * 10 + m_release;
			}
		}

		private VersionInfo()
		{
			m_major = 1;
			m_minor = 2;
			m_release = 1;
		}

		private VersionInfo(byte major, byte minor, byte release)
		{
			m_major = major;
			m_minor = minor;
			m_release = release;
		}

		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", (byte)1, (byte)2, (byte)1) + StageSuffix;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", m_major, m_minor, m_release) + StageSuffix;
		}

		public static VersionInfo Current()
		{
			return new VersionInfo(1, 2, 1);
		}

		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 2 && 1 == version.m_release;
		}
	}
}
