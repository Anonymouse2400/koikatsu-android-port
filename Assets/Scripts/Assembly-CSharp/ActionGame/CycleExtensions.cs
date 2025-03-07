namespace ActionGame
{
	public static class CycleExtensions
	{
		public static string GetName(this Cycle.Type self)
		{
			switch (self)
			{
			case Cycle.Type.WakeUp:
				return "起床";
			case Cycle.Type.Morning:
				return "朝";
			case Cycle.Type.GotoSchool:
				return "登校";
			case Cycle.Type.HR1:
				return "朝ホームルーム";
			case Cycle.Type.Lesson1:
				return "授業1";
			case Cycle.Type.LunchTime:
				return "昼休み";
			case Cycle.Type.Lesson2:
				return "授業2";
			case Cycle.Type.HR2:
				return "帰りホームルーム";
			case Cycle.Type.StaffTime:
				return "部活時間";
			case Cycle.Type.AfterSchool:
				return "放課後";
			case Cycle.Type.GotoMyHouse:
				return "帰宅";
			case Cycle.Type.MyHouse:
				return "自宅";
			default:
				return string.Empty;
			}
		}

		public static string GetName(this Cycle.Week self)
		{
			switch (self)
			{
			case Cycle.Week.Monday:
				return "月曜日";
			case Cycle.Week.Tuesday:
				return "火曜日";
			case Cycle.Week.Wednesday:
				return "水曜日";
			case Cycle.Week.Thursday:
				return "木曜日";
			case Cycle.Week.Friday:
				return "金曜日";
			case Cycle.Week.Saturday:
				return "土曜日";
			case Cycle.Week.Holiday:
				return "休日";
			default:
				return string.Empty;
			}
		}
	}
}
