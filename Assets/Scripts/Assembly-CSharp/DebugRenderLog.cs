using System.Collections.Generic;
using UnityEngine;

public class DebugRenderLog : MonoBehaviour
{
	public Color color = Color.white;

	private Queue<string> logQueue = new Queue<string>();

	private const uint LOG_MAX = 20u;
}
