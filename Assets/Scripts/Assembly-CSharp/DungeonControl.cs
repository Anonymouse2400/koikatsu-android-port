using UnityEngine;

public class DungeonControl : MonoBehaviour
{
	public Camera myCamera;

	public StaticMetaballSeed metaball;

	public ParticleSystem hitPS;

	public AudioSource audioSource;

	private void Start()
	{
		MeshCollider component = metaball.GetComponent<MeshCollider>();
		if (component != null)
		{
			component.sharedMesh = metaball.Mesh;
		}
	}

	private void Update()
	{
	}

	public void AddCell(Vector3 position, float size)
	{
		audioSource.Play();
		GameObject gameObject = new GameObject("MetaballNode");
		gameObject.transform.parent = metaball.sourceRoot.transform;
		gameObject.transform.position = position;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
		MetaballNode metaballNode = gameObject.AddComponent<MetaballNode>();
		metaballNode.baseRadius = size;
		metaball.CreateMesh();
		MeshCollider component = metaball.GetComponent<MeshCollider>();
		if (component != null)
		{
			component.sharedMesh = metaball.Mesh;
		}
		Object.Instantiate(hitPS.gameObject, position, Quaternion.identity);
	}
}
