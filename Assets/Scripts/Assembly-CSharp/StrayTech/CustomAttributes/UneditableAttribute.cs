using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class UneditableAttribute : PropertyAttribute
	{
		public enum Effective
		{
			Always = 0,
			OnlyWhilePlaying = 1,
			OnlyWhileEditing = 2
		}

		public readonly Effective EffectiveWhen;

		public UneditableAttribute()
			: this(Effective.Always)
		{
		}

		public UneditableAttribute(Effective effectiveWhen)
		{
			EffectiveWhen = effectiveWhen;
		}
	}
}
