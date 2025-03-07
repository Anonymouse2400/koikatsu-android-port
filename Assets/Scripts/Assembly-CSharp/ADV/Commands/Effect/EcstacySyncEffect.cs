using System.Collections;
using UniRx;

namespace ADV.Commands.Effect
{
	public class EcstacySyncEffect : HEffectBase
	{
		protected override IEnumerator FadeLoop(CancellationToken cancel)
		{
			yield return InEffect(0.2f, cancel);
			yield return OutEffect(0.2f, cancel);
			yield return InEffect(0.2f, cancel);
			yield return OutEffect(0.2f, cancel);
			yield return InEffect(2f, cancel);
			yield return OutEffect(2f, cancel);
		}
	}
}
