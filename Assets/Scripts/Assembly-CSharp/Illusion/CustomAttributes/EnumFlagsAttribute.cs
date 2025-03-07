using System;
using UnityEngine;

namespace Illusion.CustomAttributes
{
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
	public sealed class EnumFlagsAttribute : PropertyAttribute
	{
	}
}
