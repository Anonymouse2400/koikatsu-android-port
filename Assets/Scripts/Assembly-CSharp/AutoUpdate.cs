using UnityEngine;

[RequireComponent(typeof(ImplicitSurfaceMeshCreaterBase))]
public class AutoUpdate : MonoBehaviour
{
	private ImplicitSurfaceMeshCreaterBase _surface;

	private void Awake()
	{
		_surface = GetComponent<ImplicitSurfaceMeshCreaterBase>();
	}

	private void Update()
	{
		_surface.CreateMesh();
	}
}
