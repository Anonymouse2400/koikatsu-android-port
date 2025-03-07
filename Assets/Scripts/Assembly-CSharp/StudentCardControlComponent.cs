using System.Collections.Generic;
using Illusion.CustomAttributes;
using Localize.Translate;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class StudentCardControlComponent : MonoBehaviour
{
	private const float drawH = 272f;

	[SerializeField]
	private RectTransform rtfResize;

	[SerializeField]
	private float drawHeight = 272f;

	[SerializeField]
	private bool updateSize = true;

	[SerializeField]
	private Image baseImage;

	[SerializeField]
	private Image imgEmblem;

	[SerializeField]
	private RawImage riFace;

	[SerializeField]
	private Text textAcademy;

	[SerializeField]
	private Text textName;

	[SerializeField]
	private Text textBirthday;

	[SerializeField]
	private Text textBlood;

	[SerializeField]
	private Text textClub;

	[SerializeField]
	private Text textApproval;

	[SerializeField]
	private GameObject objNickName;

	[SerializeField]
	private TextMeshProUGUI textNickName;

	[Button("Initialize", "設定の初期化", new object[] { })]
	[Space]
	public int initialize;

	public Image BaseImage
	{
		get
		{
			return baseImage;
		}
	}

	private Dictionary<int, Data.Param> charaParamCategory
	{
		get
		{
			return Localize.Translate.Manager.OtherData.Get(3);
		}
	}

	public void SetDrawSize(float height = 272f)
	{
		drawHeight = height;
		updateSize = true;
	}

	public void ChangeDrawNickName(bool draw)
	{
		if ((bool)objNickName)
		{
			objNickName.SetActive(draw);
		}
	}

	public void Clear(string academy)
	{
		if ((bool)imgEmblem)
		{
			if ((bool)imgEmblem.sprite)
			{
				Object.Destroy(imgEmblem.sprite);
			}
			imgEmblem.sprite = null;
			imgEmblem.enabled = false;
		}
		if ((bool)riFace)
		{
			if ((bool)riFace.texture)
			{
				Object.Destroy(riFace.texture);
			}
			riFace.texture = null;
			riFace.enabled = false;
		}
		if ((bool)textAcademy)
		{
			textAcademy.text = academy;
		}
		if ((bool)textName)
		{
			textName.text = string.Empty;
		}
		if ((bool)textBirthday)
		{
			textBirthday.text = string.Empty;
		}
		if ((bool)textBlood)
		{
			textBlood.text = ChaFileDefine.GetBloodTypeStr(-1);
		}
		if ((bool)textClub)
		{
			textClub.text = Localize.Translate.Manager.UnknownText;
		}
		if ((bool)objNickName)
		{
			objNickName.SetActive(false);
		}
		if ((bool)textNickName)
		{
			textNickName.text = string.Empty;
		}
	}

	public void SetCharaInfo(ChaFileControl chaFileCtrl, int emblemId, string academy, string nickname = "")
	{
		SetCharaInfo(chaFileCtrl);
		SetEmblem(emblemId);
		SetAccademy(academy);
		SetNickName(nickname);
	}

	public void SetCharaInfo(ChaFileControl chaFileCtrl)
	{
		if (chaFileCtrl == null)
		{
			return;
		}
		if ((bool)riFace)
		{
			if ((bool)riFace.texture)
			{
				Object.Destroy(riFace.texture);
			}
			riFace.texture = null;
			riFace.enabled = false;
			if (chaFileCtrl.facePngData != null)
			{
				Texture2D texture2D = new Texture2D(240, 320);
				texture2D.LoadImage(chaFileCtrl.facePngData);
				riFace.texture = texture2D;
				riFace.enabled = true;
			}
		}
		ChaFileParameter parameter = chaFileCtrl.parameter;
		if ((bool)textName)
		{
			textName.text = parameter.fullname;
		}
		if ((bool)textBirthday)
		{
			textBirthday.text = parameter.strBirthDay;
		}
		if ((bool)textBlood)
		{
			textBlood.text = ChaFileDefine.GetBloodTypeStr(parameter.bloodType);
		}
		if ((bool)textClub)
		{
			if (parameter.sex == 0)
			{
				textClub.text = charaParamCategory.SafeGetText(3) ?? "コイカツ部";
			}
			else
			{
				textClub.text = Localize.Translate.Manager.GetClubName(parameter.clubActivities, true);
			}
		}
	}

	public void SetEmblem(int emblemId)
	{
		if (!imgEmblem)
		{
			return;
		}
		if ((bool)imgEmblem.sprite)
		{
			Object.Destroy(imgEmblem.sprite);
		}
		imgEmblem.sprite = null;
		imgEmblem.enabled = false;
		ListInfoBase listInfo = Singleton<Character>.Instance.chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.mt_emblem, emblemId);
		if (listInfo == null)
		{
			return;
		}
		string info = listInfo.GetInfo(ChaListDefine.KeyType.MainTexAB);
		string text = listInfo.GetInfo(ChaListDefine.KeyType.MainTex) + "_srgb";
		Texture2D texture2D = null;
		if ("0" != info && "0" != text)
		{
			texture2D = CommonLib.LoadAsset<Texture2D>(info, text, false, string.Empty);
			if ((bool)texture2D)
			{
				imgEmblem.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			}
			imgEmblem.enabled = true;
		}
	}

	public void SetAccademy(string academy)
	{
		if (textAcademy != null)
		{
			textAcademy.text = academy;
		}
		if (!(textApproval != null))
		{
		}
	}

	public void SetNickName(string nickname)
	{
		if ((bool)textNickName)
		{
			textNickName.text = nickname;
		}
	}

	public void SetChargeName(string chargename)
	{
		if ((bool)textClub)
		{
			textClub.text = chargename;
		}
	}

	public void Initialize()
	{
		Transform transform = base.transform.Find("resize");
		if ((bool)transform)
		{
			rtfResize = transform as RectTransform;
		}
		transform = base.transform.Find("resize/image/emblem");
		if ((bool)transform)
		{
			imgEmblem = transform.GetComponent<Image>();
		}
		transform = base.transform.Find("resize/image/face");
		if ((bool)transform)
		{
			riFace = transform.GetComponent<RawImage>();
		}
		transform = base.transform.Find("resize/text/academy");
		if ((bool)transform)
		{
			textAcademy = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/text/name");
		if ((bool)transform)
		{
			textName = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/text/birthday");
		if ((bool)transform)
		{
			textBirthday = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/text/blood");
		if ((bool)transform)
		{
			textBlood = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/text/club");
		if ((bool)transform)
		{
			textClub = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/text/approval");
		if ((bool)transform)
		{
			textApproval = transform.GetComponent<Text>();
		}
		transform = base.transform.Find("resize/image/base");
		if ((bool)transform)
		{
			baseImage = transform.GetComponent<Image>();
		}
		transform = base.transform.Find("resize/imgNickName");
		if ((bool)transform)
		{
			objNickName = transform.gameObject;
		}
		transform = base.transform.Find("resize/imgNickName/textNickName");
		if ((bool)transform)
		{
			textNickName = transform.GetComponent<TextMeshProUGUI>();
		}
	}

	private void Reset()
	{
		Initialize();
	}

	private void Start()
	{
		if (Localize.Translate.Manager.initialized)
		{
			Localize.Translate.Manager.BindFont(textAcademy);
			Localize.Translate.Manager.BindFont(textName);
			Localize.Translate.Manager.BindFont(textBirthday);
			Localize.Translate.Manager.BindFont(textBlood);
			if (textApproval != null)
			{
				textApproval.enabled = false;
			}
			Localize.Translate.Manager.BindFont(textClub);
			Localize.Translate.Manager.BindFont(textNickName);
		}
	}

	private void Update()
	{
		if (updateSize && null != rtfResize)
		{
			float num = drawHeight / 272f;
			rtfResize.localScale = new Vector3(num, num, 1f);
			updateSize = false;
		}
	}
}
