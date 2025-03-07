using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Illusion.Component
{
	public class TriggerEnterExitEvent : MonoBehaviour
	{
		[SerializeField]
		protected string[] _tagNames = new string[0];

		public HashSet<Collider> hitList { get; private set; }

		public bool isHit
		{
			get
			{
				return hitList.Any();
			}
		}

		public Collider cachedCollider { get; private set; }

		public string[] tagNames
		{
			get
			{
				return _tagNames;
			}
		}

		public event Action<Collider> onTriggerEnter = delegate
		{
		};

		public event Action<Collider> onTriggerExit = delegate
		{
		};

		protected virtual void Awake()
		{
			cachedCollider = GetComponent<Collider>();
			hitList = new HashSet<Collider>();
		}

		protected virtual void OnEnable()
		{
			hitList.Clear();
			cachedCollider.enabled = true;
		}

		protected virtual void OnDisable()
		{
			cachedCollider.enabled = false;
			HashSet<Collider> source = hitList;
			hitList = new HashSet<Collider>();
			foreach (Collider item in source.Where((Collider p) => p != null))
			{
				this.onTriggerExit(item);
			}
		}

		protected virtual bool Check(Collider other)
		{
			if (!base.enabled)
			{
				return false;
			}
			if (!_tagNames.Any())
			{
				return true;
			}
			return _tagNames.Any(other.CompareTag);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (Check(other))
			{
				this.onTriggerEnter(other);
				hitList.Add(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (Check(other))
			{
				hitList.Remove(other);
				this.onTriggerExit(other);
			}
		}
	}
}
