using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;

namespace ActionGame
{
	public class FixEventScheduler
	{
		public class Result
		{
			public int mapNo { get; private set; }

			public WaitPoint wp { get; private set; }

			public int layerIndex { get; private set; }

			public FixEventSchedule.Param param { get; private set; }

			public Result(int mapNo, FixEventSchedule.Param param)
			{
				this.mapNo = mapNo;
				wp = null;
				layerIndex = -1;
				this.param = param;
			}

			public Result(WaitPoint wp, int layerIndex, FixEventSchedule.Param param)
			{
				this.wp = wp;
				this.layerIndex = layerIndex;
				mapNo = wp.MapNo;
				this.param = param;
			}
		}

		public static bool Check(ActionScene actScene, SaveData.Heroine heroine, List<FixEventSchedule.Param> paramList, out Result result)
		{
			result = null;
			if (heroine.isTaked)
			{
				return false;
			}
			for (int i = 0; i < paramList.Count; i++)
			{
				FixEventSchedule.Param param = paramList[i];
				int item = int.Parse(param.Asset);
				if (!heroine.talkEvent.Contains(item))
				{
					if (Check(actScene, heroine, param, out result))
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		private static bool Check(ActionScene actScene, SaveData.Heroine heroine, FixEventSchedule.Param param, out Result result)
		{
			result = null;
			if (heroine.eventAfterDay < param.AfterDay)
			{
				return false;
			}
			if (param.Week.Length > 0)
			{
				Cycle.Week nowWeek = actScene.Cycle.nowWeek;
				if (param.Week.Check("平日") != -1)
				{
					if (nowWeek > Cycle.Week.Friday)
					{
						return false;
					}
				}
				else if (param.Week.Check(nowWeek.GetName()) == -1)
				{
					return false;
				}
			}
			if (param.Cycle.Check(actScene.Cycle.nowType.GetName()) == -1)
			{
				return false;
			}
			int mapNo = actScene.Map.ConvertMapNo(param.Map);
			if (!param.LayerName.IsNullOrEmpty() && actScene.Cycle.isAction)
			{
				result = ((Func<Result>)delegate
				{
					List<WaitPoint> value;
					if (!actScene.PosSet.waitPointDic.TryGetValue(mapNo, out value))
					{
						return (Result)null;
					}
					foreach (WaitPoint item in value)
					{
						foreach (var item2 in item.parameterList.Select((WaitPoint.Parameter p, int index) => new { p, index }))
						{
							if (item2.p.layer == param.LayerName)
							{
								return new Result(item, item2.index, param);
							}
						}
					}
					return (Result)null;
				})();
				if (result == null)
				{
					return false;
				}
			}
			else
			{
				switch (actScene.Cycle.nowType)
				{
				case Cycle.Type.Lesson1:
					if (param.Map.IndexOf(actScene.Cycle.GetNowLessones(actScene.fixCharaInfoDic[heroine.fixCharaID].ClassIndex)[0]) == -1)
					{
						return false;
					}
					break;
				case Cycle.Type.Lesson2:
					if (param.Map.IndexOf(actScene.Cycle.GetNowLessones(actScene.fixCharaInfoDic[heroine.fixCharaID].ClassIndex)[1]) == -1)
					{
						return false;
					}
					break;
				}
				result = new Result(mapNo, param);
			}
			return true;
		}
	}
}
