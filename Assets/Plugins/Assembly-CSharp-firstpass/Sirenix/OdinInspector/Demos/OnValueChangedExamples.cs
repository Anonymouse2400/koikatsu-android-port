using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public class OnValueChangedExamples : MonoBehaviour
	{
		[InfoBox("OnValueChanged is used here to create a material for a shader, when the shader is changed.", InfoMessageType.Info, null)]
		[OnValueChanged("CreateMaterial", false)]
		public Shader Shader;

		[ReadOnly]
		[InlineEditor(InlineEditorModes.LargePreview)]
		public Material Material;

		private void CreateMaterial()
		{
			if (Material != null)
			{
				Object.DestroyImmediate(Material);
			}
			if (Shader != null)
			{
				Material = new Material(Shader);
			}
		}
	}
}
