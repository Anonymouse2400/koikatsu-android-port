using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
	[Description("Send a Unity message to all game objects with a component of the specified type. Notice: This is slow")]
	[Category("✫ Script Control/Common")]
	public class SendMessageToType<T> : ActionTask where T : Component
	{
		public BBParameter<string> message;

		[BlackboardOnly]
		public BBParameter<object> argument;

		protected override string info
		{
			get
			{
				return string.Format("Message {0}({1}) to all {2}", message, argument.ToString(), typeof(T).Name);
			}
		}

		protected override void OnExecute()
		{
			T[] array = Object.FindObjectsOfType<T>();
			if (array.Length == 0)
			{
				EndAction(false);
				return;
			}
			T[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				T val = array2[i];
				val.gameObject.SendMessage(message.value, argument.value);
			}
			EndAction(true);
		}
	}
}
