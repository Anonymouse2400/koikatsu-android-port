using System;
using Illusion.Extensions;

namespace Illusion.Elements.Enum
{
	public abstract class BaseEnumFlags
	{
		public abstract int Length { get; }

		public virtual System.Enum this[int index]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public BaseEnumFlags(bool isEverything = false)
		{
			if (isEverything)
			{
				for (int i = 0; i < Length; i++)
				{
					this[i] = this[i].Everything();
				}
			}
		}

		public BaseEnumFlags(BaseEnumFlags flags)
		{
			for (int i = 0; i < Length; i++)
			{
				this[i] = flags[i];
			}
		}

		public void Add(BaseEnumFlags flags)
		{
			for (int i = 0; i < Length; i++)
			{
				this[i] = (System.Enum)System.Enum.ToObject(this[i].GetType(), this[i].Add(flags[i]));
			}
		}

		public void Sub(BaseEnumFlags flags)
		{
			for (int i = 0; i < Length; i++)
			{
				this[i] = (System.Enum)System.Enum.ToObject(this[i].GetType(), this[i].Sub(flags[i]));
			}
		}

		public void Get(BaseEnumFlags flags)
		{
			for (int i = 0; i < Length; i++)
			{
				this[i] = (System.Enum)System.Enum.ToObject(this[i].GetType(), this[i].Get(flags[i]));
			}
		}
	}
}
