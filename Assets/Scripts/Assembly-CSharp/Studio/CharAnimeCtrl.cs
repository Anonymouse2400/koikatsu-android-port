using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Studio
{
	public class CharAnimeCtrl : MonoBehaviour
	{
		public Animator animator;

		public Transform transSon;

		public bool isForceLoop
		{
			get
			{
				return oiCharInfo != null && oiCharInfo.isAnimeForceLoop;
			}
			set
			{
				oiCharInfo.isAnimeForceLoop = value;
			}
		}

		public OICharInfo oiCharInfo { get; set; }

		public int nameHadh { get; set; }

		public float normalizedTime
		{
			get
			{
				if (animator == null)
				{
					return 0f;
				}
				return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			}
		}

		private bool isSon
		{
			get
			{
				return oiCharInfo != null && oiCharInfo.visibleSon;
			}
		}

		public void Play(string _name, float _normalizedTime)
		{
			if (!(animator == null))
			{
				animator.Play(_name, 0, _normalizedTime);
			}
		}

		private void Awake()
		{
			(from _ in this.LateUpdateAsObservable()
				where isForceLoop
				select _).Subscribe(delegate
			{
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
				if (!currentAnimatorStateInfo.loop && currentAnimatorStateInfo.normalizedTime >= 1f)
				{
					animator.Play(nameHadh, 0, 0f);
				}
			});
			this.LateUpdateAsObservable().Subscribe(delegate
			{
				if (isSon && (bool)transSon)
				{
					transSon.localScale = new Vector3(oiCharInfo.sonLength, oiCharInfo.sonLength, oiCharInfo.sonLength);
				}
			});
		}
	}
}
