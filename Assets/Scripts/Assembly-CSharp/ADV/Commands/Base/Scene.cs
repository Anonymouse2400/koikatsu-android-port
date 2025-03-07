using Manager;

namespace ADV.Commands.Base
{
	public class Scene : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[7] { "Bundle", "Asset", "isLoad", "isAsync", "isFade", "isOverlap", "isImageDraw" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[7]
				{
					string.Empty,
					string.Empty,
					bool.FalseString,
					bool.TrueString,
					bool.TrueString,
					bool.FalseString,
					bool.FalseString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string assetBundleName = args[num++];
			string levelName = args[num++];
			bool flag = bool.Parse(args[num++]);
			bool isAsync = bool.Parse(args[num++]);
			bool isFade = bool.Parse(args[num++]);
			bool isOverlap = bool.Parse(args[num++]);
			bool isLoadingImageDraw = bool.Parse(args[num++]);
			Singleton<Manager.Scene>.Instance.LoadReserve(new Manager.Scene.Data
			{
				assetBundleName = assetBundleName,
				levelName = levelName,
				isAdd = !flag,
				isAsync = isAsync,
				isFade = isFade,
				isOverlap = isOverlap
			}, isLoadingImageDraw);
		}
	}
}
