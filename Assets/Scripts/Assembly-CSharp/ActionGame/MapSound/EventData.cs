using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionGame.MapSound
{
	[Serializable]
	public class EventData
	{
		[SerializeField]
		private float _time;

		[SerializeField]
		private KeyType _keyType;

		[SerializeField]
		private int _clipID;

		[SerializeField]
		private int[] _clipIDArray = new int[0];

		[SerializeField]
		private Threshold _rolloffThreshold = new Threshold(1f, 7.5f);

		[SerializeField]
		[HideInInspector]
		private bool _execute;

		public float Time
		{
			get
			{
				return _time;
			}
		}

		public KeyType KeyType
		{
			get
			{
				return _keyType;
			}
		}

		public int ClipID
		{
			get
			{
				return _clipID;
			}
		}

		public IEnumerable<int> ClipIDArray
		{
			get
			{
				return _clipIDArray;
			}
		}

		public Threshold RolloffThreshold
		{
			get
			{
				return _rolloffThreshold;
			}
		}

		public bool Execute
		{
			get
			{
				return _execute;
			}
			set
			{
				_execute = value;
			}
		}
	}
}
