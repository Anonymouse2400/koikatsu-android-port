using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class UniRxTestBehaviour : MonoBehaviour
{
	private void Start()
	{
		IObservable<int> source = (from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(0)
			select 1).Scan((int acc, int current) => acc + current);
		source.Subscribe(delegate
		{
		});
	}

	private void Update()
	{
	}
}
