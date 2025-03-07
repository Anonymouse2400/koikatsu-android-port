using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ActionGame
{
	public class CvsAttribute : MonoBehaviour
	{
		[SerializeField]
		private Toggle[] tglAttribute;

		[SerializeField]
		private Button btnRandom;

		[SerializeField]
		private Button btnAllOn;

		[SerializeField]
		private Button btnAllOff;

		private ReactiveProperty<ChaFileControl> _chaFile = new ReactiveProperty<ChaFileControl>();

		private const int likeGirlsIndex = 17;

		private bool isSetAttribute;

		public ChaFileControl chaFile
		{
			set
			{
				_chaFile.Value = value;
			}
		}

		public void UISync(ChaFileControl chaFile)
		{
			if (chaFile == null)
			{
				return;
			}
			bool[] attribute = ChaInfo.GetAttribute(chaFile);
			foreach (var item in from p in tglAttribute.Select((Toggle tgl, int index) => new { tgl, index })
				where p.tgl != null && p.tgl.gameObject.activeSelf
				select p)
			{
				SetToggle(item.index, attribute[item.index]);
			}
		}

		public void SetAttribute(int idx, bool flag)
		{
			if (isSetAttribute && _chaFile.Value != null)
			{
				ChaFileParameter.Attribute attribute = _chaFile.Value.parameter.attribute;
				switch (idx)
				{
				case 0:
					attribute.hinnyo = flag;
					break;
				case 1:
					attribute.harapeko = flag;
					break;
				case 2:
					attribute.donkan = flag;
					break;
				case 3:
					attribute.choroi = flag;
					break;
				case 4:
					attribute.bitch = flag;
					break;
				case 5:
					attribute.mutturi = flag;
					break;
				case 6:
					attribute.dokusyo = flag;
					break;
				case 7:
					attribute.ongaku = flag;
					break;
				case 8:
					attribute.kappatu = flag;
					break;
				case 9:
					attribute.ukemi = flag;
					break;
				case 10:
					attribute.friendly = flag;
					break;
				case 11:
					attribute.kireizuki = flag;
					break;
				case 12:
					attribute.taida = flag;
					break;
				case 13:
					attribute.sinsyutu = flag;
					break;
				case 14:
					attribute.hitori = flag;
					break;
				case 15:
					attribute.undo = flag;
					break;
				case 16:
					attribute.majime = flag;
					break;
				case 17:
					attribute.likeGirls = flag;
					break;
				}
			}
		}

		private void SetToggle(int i, bool isOn)
		{
			tglAttribute.SafeProc(i, delegate(Toggle tgl)
			{
				if (tgl.gameObject.activeSelf)
				{
					tgl.isOn = isOn;
				}
			});
		}

		private void Start()
		{
			GameObject child = base.transform.GetChild(0).gameObject;
			_chaFile.Subscribe(delegate(ChaFileControl file)
			{
				child.SetActiveIfDifferent(file != null);
				if (child.activeSelf)
				{
					isSetAttribute = false;
					UISync(file);
					isSetAttribute = true;
				}
			});
			foreach (var item in from p in tglAttribute.Select((Toggle tgl, int index) => new { tgl, index })
				where p.tgl != null && p.tgl.gameObject.activeSelf
				select p)
			{
				item.tgl.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					SetAttribute(item.index, isOn);
				});
			}
			btnRandom.OnClickAsObservable().Subscribe(delegate
			{
				for (int i = 0; i < tglAttribute.Length; i++)
				{
					SetToggle(i, Random.Range(0, 2) == 0);
				}
			});
			btnAllOn.OnClickAsObservable().Subscribe(delegate
			{
				for (int j = 0; j < tglAttribute.Length; j++)
				{
					SetToggle(j, true);
				}
			});
			btnAllOff.OnClickAsObservable().Subscribe(delegate
			{
				for (int k = 0; k < tglAttribute.Length; k++)
				{
					SetToggle(k, false);
				}
			});
		}
	}
}
