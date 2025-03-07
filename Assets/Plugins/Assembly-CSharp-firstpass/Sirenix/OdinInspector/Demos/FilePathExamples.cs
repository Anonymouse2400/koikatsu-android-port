using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public sealed class FilePathExamples : MonoBehaviour
	{
		[InfoBox("FilePath attribute provides a neat interface for assigning paths to strings.\nIt also supports drag and drop from the project folder.", InfoMessageType.Info, null)]
		[FilePath]
		public string UnityProjectPath;

		[FilePath(ParentFolder = "Assets/Plugins/Sirenix")]
		public string RelativeToParentPath;

		[FilePath(ParentFolder = "Assets/Resources")]
		public string ResourcePath;

		[FilePath(Extensions = "cs")]
		[BoxGroup("Conditions", true, false, 0)]
		public string ScriptFiles;

		[FilePath(AbsolutePath = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string AbsolutePath;

		[FilePath(RequireValidPath = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string ValidPath;

		[FilePath(UseBackslashes = true)]
		[BoxGroup("Conditions", true, false, 0)]
		public string Backslashes;

		[FilePath(ParentFolder = "$DynamicParent", Extensions = "$DynamicExtensions")]
		[BoxGroup("Member referencing", true, false, 0)]
		public string DynamicFilePath;

		[BoxGroup("Member referencing", true, false, 0)]
		public string DynamicParent = "Assets/Plugin/Sirenix";

		[BoxGroup("Member referencing", true, false, 0)]
		public string DynamicExtensions = "cs, unity, jpg";

		[FilePath(ParentFolder = "Assets/Plugins/Sirenix/Demos/Odin Inspector")]
		[BoxGroup("Lists", true, false, 0)]
		public string[] ListOfFiles;
	}
}
