using UnityEngine;

public class LogCallback : MonoBehaviour
{
	[SerializeField]
	private bool isDraw = true;

	[SerializeField]
	private bool isLeft = true;

	[SerializeField]
	private bool isUp;

	private bool isGuiArea = true;

	private LogType level = LogType.Log;
}
