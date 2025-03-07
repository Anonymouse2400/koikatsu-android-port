using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingItemComponent : MonoBehaviour
{
	public enum ChangeType
	{
		UP = 0,
		KEEP = 1,
		DOWN = 2
	}

	public enum RankType
	{
		FIRST = 0,
		SECOND = 1,
		THIRD = 2,
		ETC = 3
	}

	[SerializeField]
	public TextMeshProUGUI textTitle;

	[SerializeField]
	public TextMeshProUGUI textScore;

	[SerializeField]
	public TextMeshProUGUI textRanking;

	[SerializeField]
	public Text textHandleName;

	[SerializeField]
	public Toggle toggle;

	[SerializeField]
	public Image imgUp;

	[SerializeField]
	public Image imgKeep;

	[SerializeField]
	public Image imgDown;

	[SerializeField]
	public Image imgFirst;

	[SerializeField]
	public Image imgSecond;

	[SerializeField]
	public Image imgThird;

	[SerializeField]
	public Image imgEtc;

	public void RankChangeImage(ChangeType type)
	{
		if ((bool)imgUp)
		{
			imgUp.enabled = ChangeType.UP == type;
		}
		if ((bool)imgKeep)
		{
			imgKeep.enabled = ChangeType.KEEP == type;
		}
		if ((bool)imgDown)
		{
			imgDown.enabled = ChangeType.DOWN == type;
		}
	}

	public void RankBackImage(RankType type)
	{
		if ((bool)imgFirst)
		{
			imgFirst.enabled = RankType.FIRST == type;
		}
		if ((bool)imgSecond)
		{
			imgSecond.enabled = RankType.SECOND == type;
		}
		if ((bool)imgThird)
		{
			imgThird.enabled = RankType.THIRD == type;
		}
		if ((bool)imgEtc)
		{
			imgEtc.enabled = RankType.ETC == type;
		}
	}
}
