using System;

namespace Sirenix.OdinInspector.Demos
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PartyGroupAttribute : PropertyGroupAttribute
	{
		public float Speed { get; private set; }

		public float Range { get; private set; }

		public PartyGroupAttribute(float speed = 0f, float range = 0f, int order = 0)
			: base("_DefaultGroup", order)
		{
			Speed = speed;
			Range = range;
		}

		public PartyGroupAttribute(string groupId, float speed = 0f, float range = 0f, int order = 0)
			: base(groupId, order)
		{
			Speed = speed;
			Range = range;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			PartyGroupAttribute partyGroupAttribute = (PartyGroupAttribute)other;
			if (Speed == 0f)
			{
				Speed = partyGroupAttribute.Speed;
			}
			if (Range == 0f)
			{
				Range = partyGroupAttribute.Range;
			}
		}
	}
}
