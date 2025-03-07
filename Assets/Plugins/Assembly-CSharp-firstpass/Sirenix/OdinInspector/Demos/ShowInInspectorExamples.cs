using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ShowInInspectorExamples : MonoBehaviour
	{
		[InfoBox("ShowInInspector is used to display properties that otherwise wouldn't be shown in the inspector.\nSuch as private fields, or properties.", InfoMessageType.Info, null)]
		[InfoBox("ShowInInspector also works on properties with custom getters and setters.", InfoMessageType.Info, null)]
		[ShowInInspector]
		private int myPrivateInt;

		[ShowInInspector]
		public int MyPropertyInt { get; set; }

		[ShowInInspector]
		public Vector3 WorldSpacePosition
		{
			get
			{
				return base.transform.position;
			}
			set
			{
				base.transform.position = value;
			}
		}

		[ShowInInspector]
		public Vector3 WorldSpaceScale
		{
			get
			{
				return base.transform.lossyScale;
			}
			set
			{
				Matrix4x4 matrix4x = ((!(base.transform.parent == null)) ? base.transform.parent.worldToLocalMatrix : Matrix4x4.identity);
				base.transform.localScale = new Vector3(matrix4x.m00 * value.x, matrix4x.m11 * value.y, matrix4x.m22 * value.z);
			}
		}

		[ShowInInspector]
		public Quaternion WorldSpaceRotation
		{
			get
			{
				return base.transform.rotation;
			}
			set
			{
				base.transform.rotation = value;
			}
		}
	}
}
