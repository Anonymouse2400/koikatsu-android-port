using Illusion.Game;
using Manager;
using UnityEngine;

internal class AnimeEvent : BaseLoader
{
	[SerializeField]
	private AudioClip clip;

	private AudioSource playingSource;

	public void SePlayOneShot(int n)
	{
		playingSource = Utils.Sound.Play(Manager.Sound.Type.GameSE2D, clip);
	}

	private void OnDestroy()
	{
		if (playingSource != null)
		{
			playingSource.Stop();
		}
		Resources.UnloadAsset(clip);
	}
}
