using UnityEngine;

public class PinchInOut : MonoBehaviour
{
	public enum State
	{
		NONE = 0,
		ScalUp = 1,
		ScalDown = 2
	}

	public float moveSpeed = 0.01f;

	private State nowState;

	private float scaleRate;

	private float prevDist;

	public State NowState
	{
		get
		{
			return nowState;
		}
	}

	public float Rate
	{
		get
		{
			return scaleRate;
		}
	}

	private void Start()
	{
		nowState = State.NONE;
		scaleRate = 0f;
		prevDist = 0f;
	}

	private void Update()
	{
		if (Input.touchCount != 2)
		{
			nowState = State.NONE;
			return;
		}
		switch (Input.GetTouch(1).phase)
		{
		case TouchPhase.Moved:
		{
			float magnitude = (Input.GetTouch(1).position - Input.GetTouch(0).position).magnitude;
			float num = Input.GetTouch(1).deltaPosition.magnitude + Input.GetTouch(0).deltaPosition.magnitude;
			if (prevDist > magnitude)
			{
				nowState = State.ScalDown;
			}
			else if (prevDist < magnitude)
			{
				nowState = State.ScalUp;
			}
			else
			{
				nowState = State.NONE;
			}
			scaleRate = num * moveSpeed;
			prevDist = magnitude;
			break;
		}
		case TouchPhase.Stationary:
			scaleRate = 0f;
			nowState = State.NONE;
			break;
		}
	}
}
