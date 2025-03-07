using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ContextExample : MonoBehaviour
	{
		[InfoBox("This examples show how context objects can be used to keep track of individual states, for individual properties.\nContext objects are used throughout all of these drawer examples, for different purposes. We recommend having a look at those examples as well.", InfoMessageType.Info, null)]
		[ContextExample]
		public int Field;
	}
}
