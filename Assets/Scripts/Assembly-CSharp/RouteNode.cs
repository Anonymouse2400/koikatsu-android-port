using UnityEngine;
using UnityEngine.UI;

public class RouteNode : MonoBehaviour
{
	public enum State
	{
		Stop = 0,
		Play = 1,
		End = 2
	}

	public Button buttonSelect;

	[SerializeField]
	private Text textName;

	public Button buttonPlay;

	public Sprite[] spritePlay;

	public string text
	{
		get
		{
			return textName.text;
		}
		set
		{
			textName.text = value;
		}
	}

	public State state
	{
		set
		{
			switch (value)
			{
			case State.Stop:
				buttonPlay.image.color = Color.white;
				buttonPlay.image.sprite = spritePlay[0];
				break;
			case State.Play:
				buttonPlay.image.color = Color.green;
				buttonPlay.image.sprite = spritePlay[1];
				break;
			case State.End:
				buttonPlay.image.color = Color.red;
				buttonPlay.image.sprite = spritePlay[1];
				break;
			}
		}
	}
}
