using UnityEngine;

namespace ILSetUtility.TimeUtility
{
	public class TimeUtilityDrawer : MonoBehaviour
	{
		private GUIStyle style;

		public float fps { private get; set; }

		private void Awake()
		{
			style = new GUIStyle();
			style.fontSize = 20;
			GUIStyleState gUIStyleState = new GUIStyleState();
			gUIStyleState.textColor = Color.white;
			style.normal = gUIStyleState;
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label("FPS:" + fps.ToString("000.0"), style);
			GUILayout.EndVertical();
		}
	}
}
