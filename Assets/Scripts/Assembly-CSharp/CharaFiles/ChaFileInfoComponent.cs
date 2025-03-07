using FileListUI;

namespace CharaFiles
{
	public class ChaFileInfoComponent : ThreadFileInfoComponent
	{
		private ChaFileInfo _info;

		public ChaFileInfo info
		{
			get
			{
				return this.GetCache(ref _info, () => base.fileInfo as ChaFileInfo);
			}
		}
	}
}
