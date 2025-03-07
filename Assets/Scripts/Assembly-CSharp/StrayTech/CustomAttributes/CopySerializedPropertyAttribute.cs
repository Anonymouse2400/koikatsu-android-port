using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class CopySerializedPropertyAttribute : PropertyAttribute
	{
		public readonly string SourcePropertyPath;

		public readonly string TargetPropertyPath;

		public CopySerializedPropertyAttribute(string sourcePropertyPath, string targetPropertyPath)
		{
			SourcePropertyPath = sourcePropertyPath;
			TargetPropertyPath = targetPropertyPath;
		}
	}
}
