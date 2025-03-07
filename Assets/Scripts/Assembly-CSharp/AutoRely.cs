using Illusion.CustomAttributes;
using UnityEngine;

public class AutoRely : MonoBehaviour
{
	[SerializeField]
	[Label("再始動する時間最大")]
	private float timeReStartMax = 15f;

	[SerializeField]
	[Label("再始動する時間最小")]
	private float timeReStartMin = 10f;

	private GlobalMethod.RandomTimer timerReStart = new GlobalMethod.RandomTimer();

	[Label("強弱変更する時間最大")]
	[SerializeField]
	private float timeChangeWSMax = 20f;

	[Label("強弱変更する時間最小")]
	[SerializeField]
	private float timeChangeWSMin = 15f;

	private GlobalMethod.RandomTimer timerChangeWS = new GlobalMethod.RandomTimer();

	private void Start()
	{
		timerReStart.Init(timeReStartMin, timeReStartMax);
		timerChangeWS.Init(timeChangeWSMin, timeChangeWSMax);
	}

	public bool IsReStart()
	{
		return timerReStart.Check();
	}

	public bool IsChangeWS()
	{
		if (timerChangeWS.Check())
		{
			return Random.Range(0, 100) % 2 == 0;
		}
		return false;
	}

	public void InitTimer()
	{
		timerReStart.InitTime();
		timerChangeWS.InitTime();
	}
}
