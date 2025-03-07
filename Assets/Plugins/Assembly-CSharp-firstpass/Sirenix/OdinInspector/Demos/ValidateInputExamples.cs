using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class ValidateInputExamples : MonoBehaviour
	{
		[Space(12f)]
		[HideLabel]
		[Title("Default message", "You can just provide a default message that is always used", TitleAlignments.Left, true, true)]
		[ValidateInput("HasMeshRendererDefaultMessage", "Prefab must have a MeshRenderer component", InfoMessageType.Error)]
		public GameObject DefaultMessage;

		[Space(12f)]
		[HideLabel]
		[Title("Dynamic message", "Or the validation method can dynamically provide a custom message", TitleAlignments.Left, true, true)]
		[ValidateInput("HasMeshRendererDynamicMessage", "Prefab must have a MeshRenderer component", InfoMessageType.Error)]
		public GameObject DynamicMessage;

		[Space(12f)]
		[HideLabel]
		[Title("Dynamic message type", "The validation method can also control the type of the message", TitleAlignments.Left, true, true)]
		[ValidateInput("HasMeshRendererDynamicMessageAndType", "Prefab must have a MeshRenderer component", InfoMessageType.Error)]
		public GameObject DynamicMessageAndType;

		[Space(8f)]
		[HideLabel]
		[InfoBox("Change GameObject value to update message type", InfoMessageType.None, null)]
		public InfoMessageType MessageType;

		[Space(12f)]
		[HideLabel]
		[Title("Dynamic default message", "Use $ to indicate a member string as default message", TitleAlignments.Left, true, true)]
		[ValidateInput("AlwaysFalse", "$Message", InfoMessageType.Warning)]
		public string Message = "Dynamic ValidateInput message";

		private bool AlwaysFalse(string value)
		{
			return false;
		}

		private bool HasMeshRendererDefaultMessage(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return true;
			}
			return gameObject.GetComponentInChildren<MeshRenderer>() != null;
		}

		private bool HasMeshRendererDynamicMessage(GameObject gameObject, ref string errorMessage)
		{
			if (gameObject == null)
			{
				return true;
			}
			if (gameObject.GetComponentInChildren<MeshRenderer>() == null)
			{
				errorMessage = "\"" + gameObject.name + "\" must have a MeshRenderer component";
				return false;
			}
			return true;
		}

		private bool HasMeshRendererDynamicMessageAndType(GameObject gameObject, ref string errorMessage, ref InfoMessageType? messageType)
		{
			if (gameObject == null)
			{
				return true;
			}
			if (gameObject.GetComponentInChildren<MeshRenderer>() == null)
			{
				errorMessage = "\"" + gameObject.name + "\" should have a MeshRenderer component";
				messageType = MessageType;
				return false;
			}
			return true;
		}
	}
}
