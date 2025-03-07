using System.Collections.Generic;

public static class GameContentData
{
	private static readonly Dictionary<int, HashSet<int>> ClubContents;

	private static readonly Dictionary<int, HashSet<int>> PlayHList;

	static GameContentData()
	{
		ClubContents = new Dictionary<int, HashSet<int>> { 
		{
			0,
			new HashSet<int>(new int[2] { 0, 1 })
		} };
		PlayHList = new Dictionary<int, HashSet<int>>
		{
			{
				0,
				new HashSet<int>(new int[11]
				{
					0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
					10
				})
			},
			{
				1,
				new HashSet<int>(new int[34]
				{
					0, 1, 2, 5, 7, 8, 11, 12, 13, 15,
					16, 17, 21, 22, 24, 27, 28, 30, 31, 32,
					33, 35, 36, 39, 40, 42, 43, 44, 47, 48,
					49, 50, 51, 52
				})
			},
			{
				2,
				new HashSet<int>(new int[28]
				{
					0, 1, 2, 3, 4, 6, 7, 8, 9, 10,
					11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
					21, 22, 23, 24, 25, 26, 27, 28
				})
			}
		};
	}

	public static void SetClubContents(IDictionary<int, HashSet<int>> clubContents)
	{
		SetLoop(ClubContents, clubContents);
	}

	public static void SetPlayHList(IDictionary<int, HashSet<int>> playHList)
	{
		SetLoop(PlayHList, playHList);
	}

	private static void SetLoop(IDictionary<int, HashSet<int>> src, IDictionary<int, HashSet<int>> dst)
	{
		foreach (KeyValuePair<int, HashSet<int>> item in src)
		{
			HashSet<int> value;
			if (!dst.TryGetValue(item.Key, out value))
			{
				value = (dst[item.Key] = new HashSet<int>());
			}
			foreach (int item2 in item.Value)
			{
				value.Add(item2);
			}
		}
	}
}
