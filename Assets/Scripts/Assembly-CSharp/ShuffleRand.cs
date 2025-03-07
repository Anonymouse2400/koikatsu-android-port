using System;
using System.Linq;

public class ShuffleRand
{
	private int[] no;

	private int cnt;

	public ShuffleRand(int num = -1)
	{
		if (num != -1)
		{
			Init(num);
		}
	}

	public void Init(int num)
	{
		no = new int[num];
		Shuffle();
	}

	private void Shuffle()
	{
		if (no.Length != 0)
		{
			int[] array = new int[no.Length];
			for (int j = 0; j < no.Length; j++)
			{
				array[j] = j;
			}
			no = array.OrderBy((int i) => Guid.NewGuid()).ToArray();
			cnt = 0;
		}
	}

	public int Get()
	{
		if (no.Length == 0)
		{
			return -1;
		}
		if (cnt >= no.Length)
		{
			Shuffle();
		}
		return no[cnt++];
	}
}
