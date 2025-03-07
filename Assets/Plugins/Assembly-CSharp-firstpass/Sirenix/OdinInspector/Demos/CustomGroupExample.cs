using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class CustomGroupExample : SerializedMonoBehaviour
	{
		public class Thingy
		{
			[PartyGroup(1f, 12f, 0)]
			public Thingy ThingyField;
		}

		[PartyGroup(3f, 20f, 0)]
		public int MyInt;

		[PartyGroup("Group Two", 10f, 8f, 0)]
		public Vector3 AVector3;

		[PartyGroup("Group Two", 0f, 0f, 0)]
		public int AnotherInt;

		[InfoBox("Of course, all the controls are still usable. If you can catch them at least.", InfoMessageType.Info, null)]
		[PartyGroup("Group Three", 0.8f, 250f, 0)]
		public Quaternion AllTheWayAroundAndBack;

		[PartyGroup("Group Four", 1f, 12f, 0)]
		public Thingy ThingyField;

		[PartyGroup(0f, 0f, 0)]
		public float MyFloat { get; set; }

		[PartyGroup(0f, 0f, 0)]
		public void StateTruth()
		{
			Debug.Log("Odin Inspector is awesome.");
		}
	}
}
