using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class OnInspectorGUIExamples : MonoBehaviour
	{
		[OnInspectorGUI("DrawPreview", true)]
		public Texture2D Texture;

		private void DrawPreview()
		{
			if (!(Texture == null))
			{
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label(Texture);
				GUILayout.EndVertical();
			}
		}
	}
}
