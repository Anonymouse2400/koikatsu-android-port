using UnityEngine;

namespace StrayTech.CustomAttributes
{
	public class PathAttribute : PropertyAttribute
	{
		public enum SelectionType
		{
			Folder = 0,
			File = 1
		}

		public readonly SelectionType PathType;

		public readonly string FileExtension;

		public readonly bool RelativeToAssetsFolder;

		public PathAttribute(string fileExtension, bool relativeToAssetsFolder = false)
		{
			PathType = SelectionType.File;
			FileExtension = fileExtension ?? string.Empty;
			RelativeToAssetsFolder = relativeToAssetsFolder;
		}

		public PathAttribute(SelectionType pathType, bool relativeToAssetsFolder = false)
		{
			PathType = pathType;
			FileExtension = string.Empty;
			RelativeToAssetsFolder = relativeToAssetsFolder;
		}

		public PathAttribute(bool relativeToAssetsFolder = false)
		{
			PathType = SelectionType.File;
			FileExtension = string.Empty;
			RelativeToAssetsFolder = relativeToAssetsFolder;
		}
	}
}
