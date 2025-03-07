using System.Linq;
using IllusionUtility.GetUtility;
using IllusionUtility.SetUtility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Studio
{
	public class PVCopy : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] pv;

		[SerializeField]
		private GameObject[] bone;

		private bool[] _enable = new bool[4] { true, true, true, true };

		private bool enable
		{
			get
			{
				return _enable.Any();
			}
		}

		public bool this[int _idx]
		{
			get
			{
				return _enable.SafeGet(_idx);
			}
			set
			{
				if (MathfEx.RangeEqualOn(0, _idx, _enable.Length - 1))
				{
					_enable[_idx] = value;
				}
			}
		}

		private void Start()
		{
			(from _ in this.LateUpdateAsObservable()
				where enable
				select _).Subscribe(delegate
			{
				for (int i = 0; i < pv.Length; i++)
				{
					if (_enable[i])
					{
						bone[i].transform.CopyPosRotScl(pv[i].transform);
					}
				}
			});
		}

		private void Reset()
		{
			string[] array = new string[4] { "cf_pv_hand_L", "cf_pv_hand_R", "cf_pv_leg_L", "cf_pv_leg_R" };
			pv = new GameObject[4];
			for (int i = 0; i < array.Length; i++)
			{
				pv[i] = base.transform.FindLoop(array[i]);
			}
			string[] array2 = new string[4] { "cf_j_hand_L", "cf_j_hand_R", "cf_j_leg03_L", "cf_j_leg03_R" };
			bone = new GameObject[4];
			for (int j = 0; j < array2.Length; j++)
			{
				bone[j] = base.transform.FindLoop(array2[j]);
			}
		}
	}
}
