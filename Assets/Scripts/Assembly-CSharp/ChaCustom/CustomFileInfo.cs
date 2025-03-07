using System;
using FileListUI;
using Localize.Translate;

namespace ChaCustom
{
	public class CustomFileInfo : ThreadFileInfo
	{
		private CustomFileInfoComponent _fic;

		public string club { get; set; }

		public string personality { get; set; }

		public int category { get; set; }

		public CustomFileInfoComponent fic
		{
			get
			{
				return _fic ?? (_fic = base.component as CustomFileInfoComponent);
			}
		}

		public bool isDefaultData { get; set; }

		public CustomFileInfo(FolderAssist.FileInfo info)
			: base(info)
		{
			club = string.Empty;
			personality = string.Empty;
		}

		public void UpdateInfo(string name, string club, string personality, DateTime? time)
		{
			this.club = club;
			this.personality = personality;
			UpdateInfo(name, time);
		}

		public void UpdateInfo(string name, DateTime? time)
		{
			base.name = name;
			if (time.HasValue)
			{
				UpdateTime(time.Value);
			}
			DeleteThumb();
		}

		public void UpdateInfo(ChaFileParameter parameter, DateTime dateTime)
		{
			string personalityName;
			string clubName;
			Localize.Translate.Manager.GetName(parameter, false, out personalityName, out clubName);
			UpdateInfo(parameter.fullname, clubName, personalityName, dateTime);
		}
	}
}
