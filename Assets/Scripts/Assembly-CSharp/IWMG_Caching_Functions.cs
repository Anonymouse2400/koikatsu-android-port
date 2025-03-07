using System.Collections.Generic;

public interface IWMG_Caching_Functions
{
	void updateCacheAndFlagList<T>(ref List<T> cache, List<T> val, ref bool flag);

	void updateCacheAndFlag<T>(ref T cache, T val, ref bool flag);

	void SwapVals<T>(ref T val1, ref T val2);

	void SwapValsList<T>(ref List<T> val1, ref List<T> val2);
}
