using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class InlineEditorExamples : MonoBehaviour
	{
		[DisableInInlineEditors]
		public Vector3 DisabledInInlineEditors;

		[HideInInlineEditors]
		public Vector3 HiddenInInlineEditors;

		[InlineEditor(InlineEditorModes.GUIOnly)]
		public InlineEditorExamples Self;

		[InlineEditor(InlineEditorModes.GUIOnly)]
		public Transform InlineComponent;

		[InlineEditor(InlineEditorModes.FullEditor)]
		public Material FullInlineEditor;

		[InlineEditor(InlineEditorModes.GUIAndHeader)]
		public Material InlineMaterial;

		[InlineEditor(InlineEditorModes.SmallPreview)]
		public Material[] InlineMaterialList;

		[InlineEditor(InlineEditorModes.LargePreview)]
		public GameObject InlineObjectPreview;

		[InlineEditor(InlineEditorModes.LargePreview)]
		public Mesh InlineMeshPreview;
	}
}
