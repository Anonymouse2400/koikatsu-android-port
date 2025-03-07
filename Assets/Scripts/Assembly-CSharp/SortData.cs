public class SortData
{
	public int ID;

	public double ts;

	public string path;

	public SortData(int ID, double ts, string path)
	{
		this.ID = ID;
		this.ts = ts;
		this.path = path;
	}

	public static int CompareDateTimeAsc(SortData a, SortData b)
	{
		int? num = Check(a, b);
		return num.HasValue ? num.Value : a.ts.CompareTo(b.ts);
	}

	public static int CompareDateTimeDesc(SortData a, SortData b)
	{
		int? num = Check(a, b);
		return num.HasValue ? num.Value : b.ts.CompareTo(a.ts);
	}

	private static int? Check(SortData a, SortData b)
	{
		if (a == null)
		{
			if (b == null)
			{
				return 0;
			}
			return 1;
		}
		if (b == null)
		{
			return -1;
		}
		return null;
	}
}
