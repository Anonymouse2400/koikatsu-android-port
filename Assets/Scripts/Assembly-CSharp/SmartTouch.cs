using UnityEngine;

public class SmartTouch : MonoBehaviour
{
	public float countTime = 0.2f;

	private Vector2 inPos;

	private Vector2 upPos;

	private float inTimer;

	private float outTimer;

	private int tapCount;

	private bool inFlg;

	public Vector2 InPos
	{
		get
		{
			return inPos;
		}
	}

	public Vector2 UpPos
	{
		get
		{
			return upPos;
		}
	}

	public float Distance
	{
		get
		{
			return (!Tapping) ? (upPos - inPos).magnitude : 0f;
		}
	}

	public float TapTime
	{
		get
		{
			return inTimer;
		}
	}

	public bool Tapping
	{
		get
		{
			return inFlg;
		}
	}

	public int TapCount
	{
		get
		{
			return tapCount;
		}
	}

	private void Start()
	{
		inPos = Vector2.zero;
		upPos = Vector2.zero;
		inTimer = 0f;
		outTimer = 0f;
		tapCount = 0;
		inFlg = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			inPos = Input.mousePosition;
			inFlg = true;
			inTimer = 0f;
			if (outTimer < countTime)
			{
				tapCount++;
			}
			else
			{
				tapCount = 1;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			upPos = Input.mousePosition;
			inFlg = false;
		}
		if (inFlg)
		{
			inTimer += Time.deltaTime;
			outTimer = 0f;
			return;
		}
		outTimer += Time.deltaTime;
		outTimer = Mathf.Min(outTimer, countTime);
		if (outTimer == countTime)
		{
			tapCount = 0;
		}
	}
}
