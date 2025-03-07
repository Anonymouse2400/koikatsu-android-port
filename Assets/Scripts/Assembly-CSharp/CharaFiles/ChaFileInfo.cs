using FileListUI;

namespace CharaFiles
{
	public class ChaFileInfo : ThreadFileInfo
	{
		private ChaFileInfoComponent _fic;

		public ChaFileInfoComponent fic
		{
			get
			{
				return _fic ?? (_fic = base.component as ChaFileInfoComponent);
			}
		}

		public string nickname { get; set; }

		public int height { get; set; }

		public int bustSize { get; set; }

		public int hair { get; set; }

		public int personality { get; set; }

		public int bloodType { get; set; }

		public int month { get; set; }

		public int day { get; set; }

		public string birthDay_Origin
		{
			get
			{
				return string.Format("{0}月{1}日", month, day);
			}
		}

		public string birthDay { get; set; }

		public int club { get; set; }

		public int sex { get; set; }

		public ChaFileInfo(ChaFileControl chaFile, FolderAssist.FileInfo info)
			: base(info)
		{
			ChaFileParameter parameter = chaFile.parameter;
			ChaFileCustom custom = chaFile.custom;
			base.name = parameter.fullname;
			nickname = parameter.nickname;
			height = custom.GetHeightKind();
			bustSize = custom.GetBustSizeKind();
			hair = custom.hair.kind;
			personality = parameter.personality;
			bloodType = parameter.bloodType;
			month = parameter.birthMonth;
			day = parameter.birthDay;
			birthDay = parameter.strBirthDay;
			club = parameter.clubActivities;
			sex = parameter.sex;
		}
	}
}
