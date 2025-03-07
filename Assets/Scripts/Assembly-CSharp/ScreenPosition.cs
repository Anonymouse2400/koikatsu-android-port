using UnityEngine;

[ExecuteInEditMode]
public class ScreenPosition : MonoBehaviour
{
	[SerializeField]
	private Vector3 _position = new Vector3(0f, 0f, 10f);

	private bool isChange;

	public Vector3 position
	{
		get
		{
			return _position;
		}
		set
		{
			isChange = true;
			_position = value;
			base.transform.position = Camera.main.ScreenToWorldPoint(_position);
		}
	}

	private void Update()
	{
		if (base.transform.hasChanged && !isChange)
		{
			_position = Camera.main.WorldToScreenPoint(base.transform.position);
		}
		else
		{
			isChange = false;
		}
	}

	private void OnValidate()
	{
		position = _position;
	}
}
