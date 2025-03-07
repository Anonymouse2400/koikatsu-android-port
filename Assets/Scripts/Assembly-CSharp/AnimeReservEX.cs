public class AnimeReservEX : AnimeReserv
{
	private AnimationAssist assist;

	public AnimeReservEX(AnimationAssist _assist)
		: base(_assist.NowAnimation)
	{
		assist = _assist;
	}

	public void Update()
	{
		if (animeQueue.Count > 0 && assist.IsAnimeEnd())
		{
			AnimeData animeData = animeQueue.Dequeue();
			assist.Play(animeData.Name, 0f, animeData.FadeSpeed);
		}
	}
}
