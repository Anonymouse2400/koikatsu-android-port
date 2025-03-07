using System;
using System.Collections.Generic;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ParamUI : MonoBehaviour
{
	private class GaugeInfo
	{
		public Image image { get; private set; }

		public int min { get; private set; }

		public int max { get; private set; }

		public int now { get; private set; }

		public int target { get; set; }

		public bool isUpdate
		{
			get
			{
				return now != target;
			}
		}

		public float inverseLerp
		{
			get
			{
				return Mathf.InverseLerp(min, max, now);
			}
		}

		public SingleAssignmentDisposable disposable { get; set; }

		public GaugeInfo(Image _image, int _min, int _max, int _now)
		{
			image = _image;
			min = _min;
			max = _max;
			now = _now;
			target = _now;
			disposable = new SingleAssignmentDisposable();
			image.fillAmount = inverseLerp;
		}

		public void Update()
		{
			now = Mathf.Clamp((target <= now) ? (now - 1) : (now + 1), min, max);
			image.fillAmount = inverseLerp;
		}

		public void ForceUpdate(int _value)
		{
			now = _value;
			target = _value;
			image.fillAmount = inverseLerp;
		}
	}

	private class NumInfo
	{
		private Sprite[] spriteNum;

		public Image[] imageNum { get; private set; }

		public Image imageLimit { get; private set; }

		public Image imageMax { get; private set; }

		public int now { get; private set; }

		public int target { get; set; }

		public bool isUpdate
		{
			get
			{
				return now != target;
			}
		}

		public SingleAssignmentDisposable disposable { get; set; }

		public NumInfo(Image[] _imageNum, Image _imageLimit, Image _imageMax, int _now, Sprite[] _spriteNum)
		{
			imageNum = _imageNum;
			imageLimit = _imageLimit;
			imageMax = _imageMax;
			now = _now;
			target = _now;
			spriteNum = _spriteNum;
			disposable = new SingleAssignmentDisposable();
			Calc();
		}

		public void Update()
		{
			now = Mathf.Clamp((target <= now) ? (now - 1) : (now + 1), 0, 100);
			Calc();
		}

		public void ForceUpdate(int _value)
		{
			now = _value;
			target = _value;
			Calc();
		}

		private void Calc()
		{
			bool flag = now == 100;
			for (int i = 0; i < imageNum.Length; i++)
			{
				if (imageNum[i].enabled != !flag)
				{
					imageNum[i].enabled = !flag;
				}
			}
			if (imageLimit.enabled != !flag)
			{
				imageLimit.enabled = !flag;
			}
			if (imageMax.enabled != flag)
			{
				imageMax.enabled = flag;
			}
			if (flag)
			{
				return;
			}
			int[] array = Split();
			int num = array.Length - 1;
			for (int num2 = 2; num2 >= 0; num2--)
			{
				if (num < 0)
				{
					imageNum[num2].enabled = false;
				}
				else
				{
					imageNum[num2].enabled = true;
					imageNum[num2].sprite = spriteNum[array[num]];
				}
				num--;
			}
		}

		private int[] Split()
		{
			if (now == 0)
			{
				return new int[1];
			}
			Stack<int> stack = new Stack<int>();
			for (int num = now; num > 0; num /= 10)
			{
				stack.Push(num % 10);
			}
			return stack.ToArray();
		}
	}

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Image imagePhysical;

	[SerializeField]
	private Image[] imagePhysicalNum;

	[SerializeField]
	private Image imagePhysicalNumLimit;

	[SerializeField]
	private Image imagePhysicalNumMax;

	[SerializeField]
	private Image imageIntellect;

	[SerializeField]
	private Image[] imageIntellectNum;

	[SerializeField]
	private Image imageIntellectNumLimit;

	[SerializeField]
	private Image imageIntellectNumMax;

	[SerializeField]
	private Image imageHentai;

	[SerializeField]
	private Image[] imageHentaiNum;

	[SerializeField]
	private Image imageHentaiNumLimit;

	[SerializeField]
	private Image imageHentaiNumMax;

	[SerializeField]
	private Sprite[] spriteNum;

	[SerializeField]
	private Button buttonParam;

	[SerializeField]
	private Animator animatorParam;

	[SerializeField]
	private RawImage rawMaleFace;

	[SerializeField]
	private Image[] imageAggressive;

	[SerializeField]
	private Image[] imageDiligence;

	[SerializeField]
	private Image[] imageKindness;

	[SerializeField]
	private GameObject objFemaleRoot;

	[SerializeField]
	private RawImage rawFemaleFace;

	[SerializeField]
	private Image[] imageFavor;

	[SerializeField]
	private Image[] imageLewdness;

	[SerializeField]
	private Image imageState;

	[SerializeField]
	private Sprite[] spriteState;

	[SerializeField]
	private Image imageFrame;

	[SerializeField]
	private Sprite[] spriteFrame;

	private GaugeInfo physicalGaugeInfo;

	private GaugeInfo intellectGaugeInfo;

	private GaugeInfo hentaiGaugeInfo;

	private NumInfo physicalNumInfo;

	private NumInfo intellectNumInfo;

	private NumInfo hentaiNumInfo;

	private SingleAssignmentDisposable[] disposable;

	public bool active
	{
		get
		{
			return canvas.enabled;
		}
		set
		{
			canvas.enabled = value;
		}
	}

	public bool isOpne { get; private set; }

	private SaveData.Player player
	{
		get
		{
			return (!Singleton<Game>.IsInstance()) ? null : Singleton<Game>.Instance.Player;
		}
	}

	public void Init()
	{
		disposable = new SingleAssignmentDisposable[3];
		physicalGaugeInfo = new GaugeInfo(imagePhysical, 0, 100, player.physical);
		physicalGaugeInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where physicalGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			physicalGaugeInfo.Update();
		}).AddTo(this);
		physicalNumInfo = new NumInfo(imagePhysicalNum, imagePhysicalNumLimit, imagePhysicalNumMax, player.physical, spriteNum);
		physicalNumInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where physicalNumInfo.isUpdate
			select _).Subscribe(delegate
		{
			physicalNumInfo.Update();
		}).AddTo(this);
		disposable[0] = new SingleAssignmentDisposable();
		disposable[0].Disposable = this.ObserveEveryValueChanged((ParamUI x) => x.player.physical).Subscribe(delegate(int x)
		{
			physicalGaugeInfo.target = x;
			physicalNumInfo.target = x;
		}).AddTo(this);
		intellectGaugeInfo = new GaugeInfo(imageIntellect, 0, 100, player.intellect);
		intellectGaugeInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where intellectGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			intellectGaugeInfo.Update();
		}).AddTo(this);
		intellectNumInfo = new NumInfo(imageIntellectNum, imageIntellectNumLimit, imageIntellectNumMax, player.intellect, spriteNum);
		intellectNumInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where intellectNumInfo.isUpdate
			select _).Subscribe(delegate
		{
			intellectNumInfo.Update();
		}).AddTo(this);
		disposable[1] = new SingleAssignmentDisposable();
		disposable[1].Disposable = this.ObserveEveryValueChanged((ParamUI x) => x.player.intellect).Subscribe(delegate(int x)
		{
			intellectGaugeInfo.target = x;
			intellectNumInfo.target = x;
		}).AddTo(this);
		hentaiGaugeInfo = new GaugeInfo(imageHentai, 0, 100, player.hentai);
		hentaiGaugeInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where hentaiGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			hentaiGaugeInfo.Update();
		}).AddTo(this);
		hentaiNumInfo = new NumInfo(imageHentaiNum, imageHentaiNumLimit, imageHentaiNumMax, player.hentai, spriteNum);
		hentaiNumInfo.disposable.Disposable = (from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where hentaiNumInfo.isUpdate
			select _).Subscribe(delegate
		{
			hentaiNumInfo.Update();
		}).AddTo(this);
		disposable[2] = new SingleAssignmentDisposable();
		disposable[2].Disposable = this.ObserveEveryValueChanged((ParamUI x) => x.player.hentai).Subscribe(delegate(int x)
		{
			hentaiGaugeInfo.target = x;
			hentaiNumInfo.target = x;
		}).AddTo(this);
		Texture2D texture2D = new Texture2D(240, 320);
		texture2D.LoadImage(player.charFile.facePngData);
		rawMaleFace.texture = texture2D;
		rawMaleFace.enabled = true;
		isOpne = true;
		SetHeroine(null);
	}

	public void SetHeroine(SaveData.Heroine _heroine)
	{
		if (_heroine == null)
		{
			if (objFemaleRoot.activeSelf)
			{
				objFemaleRoot.SetActive(false);
			}
			return;
		}
		if (!objFemaleRoot.activeSelf)
		{
			objFemaleRoot.SetActive(true);
		}
		bool flag = _heroine.fixCharaID != 0;
		Texture2D texture2D = new Texture2D(240, 320);
		texture2D.LoadImage(_heroine.charFile.facePngData);
		rawFemaleFace.texture = texture2D;
		rawFemaleFace.enabled = true;
		int num = Mathf.Max(0, _heroine.relation);
		imageState.sprite = spriteState[num * 2 + (_heroine.isAnger ? 1 : 0)];
		imageState.enabled = !flag;
		switch (num)
		{
		case 0:
			imageFavor[0].fillAmount = Mathf.InverseLerp(0f, 100f, _heroine.favor);
			imageFavor[1].fillAmount = 0f;
			imageFavor[2].fillAmount = 0f;
			break;
		case 1:
			imageFavor[0].fillAmount = 1f;
			imageFavor[1].fillAmount = Mathf.InverseLerp(0f, 100f, _heroine.favor);
			imageFavor[2].fillAmount = 0f;
			break;
		case 2:
			imageFavor[0].fillAmount = 0f;
			imageFavor[1].fillAmount = 1f;
			imageFavor[2].fillAmount = Mathf.InverseLerp(0f, 100f, _heroine.favor);
			break;
		}
		imageLewdness[1].fillAmount = Mathf.InverseLerp(0f, 100f, _heroine.lewdness);
		Image[] array = imageLewdness;
		foreach (Image image in array)
		{
			image.enabled = !flag;
		}
		imageFrame.sprite = spriteFrame[flag ? ((!_heroine.isTeacher) ? 1 : 2) : 0];
	}

	public void Release()
	{
		if (physicalGaugeInfo != null)
		{
			physicalGaugeInfo.disposable.Dispose();
		}
		if (intellectGaugeInfo != null)
		{
			intellectGaugeInfo.disposable.Dispose();
		}
		if (hentaiGaugeInfo != null)
		{
			hentaiGaugeInfo.disposable.Dispose();
		}
		if (physicalNumInfo != null)
		{
			physicalNumInfo.disposable.Dispose();
		}
		if (intellectNumInfo != null)
		{
			intellectNumInfo.disposable.Dispose();
		}
		if (hentaiNumInfo != null)
		{
			hentaiNumInfo.disposable.Dispose();
		}
		SingleAssignmentDisposable[] array = disposable;
		foreach (SingleAssignmentDisposable singleAssignmentDisposable in array)
		{
			if (singleAssignmentDisposable != null)
			{
				singleAssignmentDisposable.Dispose();
			}
		}
	}

	public void UpdatePlayer()
	{
		physicalGaugeInfo.ForceUpdate(player.physical);
		intellectGaugeInfo.ForceUpdate(player.intellect);
		hentaiGaugeInfo.ForceUpdate(player.hentai);
		physicalNumInfo.ForceUpdate(player.physical);
		intellectNumInfo.ForceUpdate(player.intellect);
		hentaiNumInfo.ForceUpdate(player.hentai);
		Texture2D texture2D = new Texture2D(240, 320);
		texture2D.LoadImage(player.charFile.facePngData);
		rawMaleFace.texture = texture2D;
		rawMaleFace.enabled = true;
	}
}
