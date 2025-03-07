using System;

namespace Sirenix.OdinInspector.Demos
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class HealthBarAttribute : Attribute
	{
		public float MaxHealth { get; private set; }

		public HealthBarAttribute(float maxHealth)
		{
			MaxHealth = maxHealth;
		}
	}
}
