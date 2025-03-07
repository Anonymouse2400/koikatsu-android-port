using System;
using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue_Custom")]
public class SetRenderQueue_Custom : MonoBehaviour
{
	[Serializable]
	public class QueueData
	{
		public int id;

		public int m_queues = 3000;

		public int m_queuesBackup = 3000;
	}

	[SerializeField]
	protected QueueData[] m_queueDatas;

	public bool isSetQueue;

	private bool isQueueBackup;

	private Renderer rend;

	protected void Awake()
	{
		rend = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (isSetQueue != isQueueBackup)
		{
			ChangeRendererQueue();
			isQueueBackup = isSetQueue;
		}
	}

	public void ChangeRendererQueue()
	{
		if (rend == null)
		{
			return;
		}
		Material[] materials = rend.materials;
		if (materials == null)
		{
			return;
		}
		for (int i = 0; i < materials.Length && i < m_queueDatas.Length; i++)
		{
			int id = m_queueDatas[i].id;
			if (!(null == materials[id]))
			{
				if (isSetQueue)
				{
					materials[id].renderQueue = m_queueDatas[i].m_queues;
				}
				else
				{
					materials[id].renderQueue = m_queueDatas[i].m_queuesBackup;
				}
			}
		}
	}
}
