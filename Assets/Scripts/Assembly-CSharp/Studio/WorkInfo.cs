using System;
using System.IO;
using System.Text;

namespace Studio
{
	public class WorkInfo
	{
		private const string userPath = "studio";

		private const string fileName = "work.dat";

		private readonly Version version = new Version(1, 0, 0);

		public bool[] visibleFlags = new bool[6] { true, true, true, true, true, true };

		public bool visibleCenter = true;

		public bool visibleAxis = true;

		public bool useAlt;

		public bool visibleAxisTranslation = true;

		public bool visibleAxisCenter = true;

		public void Init()
		{
			for (int i = 0; i < 6; i++)
			{
				visibleFlags[i] = true;
			}
			visibleCenter = true;
			visibleAxis = true;
			useAlt = false;
			visibleAxisTranslation = true;
			visibleAxisCenter = true;
		}

		public void Save()
		{
			string path = UserData.Create("studio") + "work.dat";
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output, Encoding.UTF8))
				{
					try
					{
						binaryWriter.Write(version.ToString());
						for (int i = 0; i < 6; i++)
						{
							binaryWriter.Write(visibleFlags[i]);
						}
						binaryWriter.Write(visibleCenter);
						binaryWriter.Write(visibleAxis);
						binaryWriter.Write(useAlt);
						binaryWriter.Write(visibleAxisTranslation);
						binaryWriter.Write(visibleAxisCenter);
					}
					catch (Exception)
					{
						File.Delete(path);
					}
				}
			}
		}

		public void Load()
		{
			string path = UserData.Create("studio") + "work.dat";
			if (!File.Exists(path))
			{
				Init();
				return;
			}
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8))
				{
					try
					{
						Version version = new Version(binaryReader.ReadString());
						for (int i = 0; i < 6; i++)
						{
							visibleFlags[i] = binaryReader.ReadBoolean();
						}
						visibleCenter = binaryReader.ReadBoolean();
						visibleAxis = binaryReader.ReadBoolean();
						useAlt = binaryReader.ReadBoolean();
						if (version.CompareTo(new Version(1, 0, 0)) >= 0)
						{
							visibleAxisTranslation = binaryReader.ReadBoolean();
						}
						if (version.CompareTo(new Version(1, 0, 0)) >= 0)
						{
							visibleAxisCenter = binaryReader.ReadBoolean();
						}
					}
					catch (Exception)
					{
						File.Delete(path);
						Init();
					}
				}
			}
		}
	}
}
