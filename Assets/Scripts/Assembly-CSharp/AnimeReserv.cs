using System.Collections.Generic;
using UnityEngine;

public class AnimeReserv
{
	protected class AnimeData
	{
		private string name;

		private float fadeSpeed;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public float FadeSpeed
		{
			get
			{
				return fadeSpeed;
			}
		}

		public AnimeData(string _name, float _fadeSpeed = 0f)
		{
			name = _name;
			fadeSpeed = _fadeSpeed;
		}
	}

	protected Queue<AnimeData> animeQueue;

	protected Animation animation;

	public AnimeReserv(Animation _animation)
	{
		animation = _animation;
		animeQueue = new Queue<AnimeData>();
	}

	public void ReservAdd(string name, float fadeSpeed = 0f)
	{
		animeQueue.Enqueue(new AnimeData(name, fadeSpeed));
	}

	public void ReservEXE()
	{
		while (animeQueue.Count > 0)
		{
			AnimeData animeData = animeQueue.Dequeue();
			if (animeData.FadeSpeed == 0f)
			{
				animation.PlayQueued(animeData.Name);
			}
			else
			{
				animation.CrossFadeQueued(animeData.Name, animeData.FadeSpeed);
			}
		}
	}
}
