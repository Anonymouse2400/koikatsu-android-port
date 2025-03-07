using System.Collections;
using UniRx;

namespace ADV.Commands.Effect
{
	public class EjaculationEffect : HEffectBase
	{
		protected override IEnumerator FadeLoop(CancellationToken cancel)
		{
			yield return InEffect(0.5f, cancel);
			yield return OutEffect(1f, cancel);
			yield return InEffect(1f, cancel);
			yield return OutEffect(0.5f, cancel);
		}
	}
}
