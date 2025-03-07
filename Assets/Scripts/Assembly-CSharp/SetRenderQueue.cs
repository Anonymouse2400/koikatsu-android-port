using System;
using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]
public class SetRenderQueue : MonoBehaviour
{
	[Serializable]
	public class QueueData
	{
		public int id;

		public int m_queues = 3000;
	}

	[SerializeField]
	protected QueueData[] m_queueDatas;

	protected void Awake()
	{
		Material[] materials = GetComponent<Renderer>().materials;
		for (int i = 0; i < materials.Length && i < m_queueDatas.Length; i++)
		{
			materials[m_queueDatas[i].id].renderQueue = m_queueDatas[i].m_queues;
		}
	}
}
