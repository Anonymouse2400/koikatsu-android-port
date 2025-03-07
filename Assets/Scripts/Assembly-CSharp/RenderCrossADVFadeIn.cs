public class RenderCrossADVFadeIn : BaseRenderCrossFade
{
	protected override void Awake()
	{
		base.Awake();
		isAlphaAdd = false;
		Capture();
	}
}
