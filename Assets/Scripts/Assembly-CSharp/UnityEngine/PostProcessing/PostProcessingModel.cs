using System;

namespace UnityEngine.PostProcessing
{
	[Serializable]
	public abstract class PostProcessingModel
	{
		[GetSet("enabled")]
		[SerializeField]
		private bool m_Enabled;

		public bool enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
				if (value)
				{
					OnValidate();
				}
			}
		}

		public abstract void Reset();

		public virtual void OnValidate()
		{
		}
	}
}
