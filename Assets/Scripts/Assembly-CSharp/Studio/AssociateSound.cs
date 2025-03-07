using Manager;
using UnityEngine;

namespace Studio
{
	public class AssociateSound : MonoBehaviour
	{
		[SerializeField]
		private AudioSource m_AudioSource;

		private void Awake()
		{
			if (m_AudioSource != null)
			{
				m_AudioSource.outputAudioMixerGroup = Manager.Sound.Mixer.FindMatchingGroups("GameSE")[0];
			}
			Object.Destroy(this);
		}
	}
}
