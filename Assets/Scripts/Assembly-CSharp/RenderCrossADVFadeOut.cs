public class RenderCrossADVFadeOut : BaseRenderCrossFade
{
	protected override void Awake()
	{
		base.Awake();
		isAlphaAdd = true;
		Capture();
	}
}
