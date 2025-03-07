using System.Collections.Generic;
using UnityEngine;

namespace Illusion.Component
{
	internal class AnimationChangeState : MonoBehaviour
	{
		public enum PlayType
		{
			Animator = 0,
			Chara = 1
		}

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private PlayType _playType;

		[SerializeField]
		private int _layerNo;

		[SerializeField]
		private ChaControl _info;

		[SerializeField]
		private List<Animator> _animList = new List<Animator>();

		[SerializeField]
		private List<ChaControl> _chaList = new List<ChaControl>();

		public Animator animator
		{
			get
			{
				return _animator;
			}
			set
			{
				_animator = value;
			}
		}

		public PlayType playType
		{
			get
			{
				return _playType;
			}
		}

		public int layerNo
		{
			get
			{
				return _layerNo;
			}
			set
			{
				_layerNo = value;
			}
		}

		public ChaControl info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;
			}
		}

		public List<Animator> animList
		{
			get
			{
				return _animList;
			}
		}

		public List<ChaControl> chaList
		{
			get
			{
				return _chaList;
			}
		}

		private void Awake()
		{
			if (_animator == null)
			{
				_animator = GetComponent<Animator>();
			}
		}
	}
}
