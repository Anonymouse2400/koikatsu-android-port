namespace UnityEngine.UI.RadarChart
{
	public abstract class BaseDrawer : MaskableGraphic
	{
		[SerializeField]
		private Texture tex;

		private static Vector2[] uvs = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f)
		};

		public override Texture mainTexture
		{
			get
			{
				return (!(tex == null)) ? tex : Graphic.s_WhiteTexture;
			}
		}

		protected abstract void OnPopulateMeshProcess(VertexHelper vh);

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			OnPopulateMeshProcess(vh);
		}

		protected static UIVertex[] GetVert(Color color, params Vector2[] vertices)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}
	}
}
