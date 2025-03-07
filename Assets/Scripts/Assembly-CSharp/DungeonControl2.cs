using UnityEngine;

public class DungeonControl2 : MonoBehaviour
{
	public Camera myCamera;

	public IncrementalModeling metaball;

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

	public void Attack(IMBrush brush)
	{
		audioSource.Play();
		brush.Draw();
		MeshCollider component = metaball.GetComponent<MeshCollider>();
		if (component != null)
		{
			component.sharedMesh = metaball.Mesh;
		}
	}
}
