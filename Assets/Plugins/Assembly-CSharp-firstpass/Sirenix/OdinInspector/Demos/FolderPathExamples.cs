using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public sealed class FolderPathExamples : MonoBehaviour
	{
		[InfoBox("FolderPath attribute provides a neat interface for assigning paths to strings.\nIt also supports drag and drop from the project folder.", InfoMessageType.Info, null)]
		[FolderPath]
		public string UnityProjectPath;

		[FolderPath(ParentFolder = "Assets/Plugins/Sirenix")]
		public string RelativeToParentPath;

		[FolderPath(ParentFolder = "Assets/Resources")]
		public string ResourcePath;

		[FolderPath(AbsolutePath = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string AbsolutePath;

		[FolderPath(RequireValidPath = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string ValidPath;

		[FolderPath(UseBackslashes = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string Backslashes;

		[FolderPath(ParentFolder = "$DynamicParent")]
		[BoxGroup("Member referencing", true, false, 0)]
		public string DynamicFolderPath;

		[BoxGroup("Member referencing", true, false, 0)]
		public string DynamicParent = "Assets/Plugins/Sirenix";

		[FolderPath(ParentFolder = "Assets/Plugins/Sirenix")]
		[BoxGroup("Lists", true, false, 0)]
		public string[] ListOfFolders;
	}
}
