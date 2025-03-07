using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public abstract class IndexedItem<TKey, TValue> : IValidates
	{
		[SerializeField]
		private TKey _ID;

		[SerializeField]
		private TValue _Value;

		public TKey ID
		{
			get
			{
				return _ID;
			}
			protected set
			{
				_ID = value;
			}
		}

		public TValue Value
		{
			get
			{
				return _Value;
			}
			protected set
			{
				_Value = value;
			}
		}

		public IndexedItem()
			: this(default(TKey), default(TValue))
		{
		}

		public IndexedItem(TKey ID, TValue value)
		{
			_ID = ID;
			_Value = value;
		}

		public abstract bool IsValid();
	}
}
