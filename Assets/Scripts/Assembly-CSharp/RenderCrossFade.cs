using UnityEngine;

public class RenderCrossFade : BaseRenderCrossFade
{
	public enum State
	{
		None = 0,
		Ready = 1,
		Draw = 2
	}

	private bool isSubAlpha;

	public bool IsEnd
	{
		get
		{
			return state == State.None;
		}
	}

	public State state { get; set; }

	public bool isMyUpdateCap { get; set; }

	public void SubAlphaStart()
	{
		isSubAlpha = true;
	}

	public void Set()
	{
		state = State.Ready;
		isSubAlpha = false;
	}

	public override void End()
	{
		base.End();
		state = State.None;
	}

	protected override void Awake()
	{
		base.Awake();
		isInitRenderSetting = false;
		state = State.None;
		isMyUpdateCap = true;
		isAlphaAdd = false;
		base.alpha = 0f;
		timer = 0f;
	}

	protected override void Update()
	{
		if (isMyUpdateCap)
		{
			UpdateCalc();
		}
	}

	public void UpdateDrawer()
	{
		if (!isMyUpdateCap)
		{
			UpdateCalc();
		}
	}

	private void UpdateCalc()
	{
		if (state == State.Ready)
		{
			Capture();
			state = State.Draw;
		}
		if (state == State.Draw && isSubAlpha)
		{
			timer += Time.deltaTime;
			AlphaCalc();
			if (timer > maxTime)
			{
				state = State.None;
			}
		}
	}

	protected override void OnGUI()
	{
		if (state == State.Draw)
		{
			GUI.depth = 10;
			GUI.color = new Color(1f, 1f, 1f, base.alpha);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), texture);
			base.isDrawGUI = true;
		}
	}
}
