using UnityEngine;

public class MetaballCellObject : MonoBehaviour
{
	protected MetaballCell _cell;

	public MetaballCell Cell
	{
		get
		{
			return _cell;
		}
	}

	public virtual void Setup(MetaballCell cell)
	{
		_cell = cell;
	}
}
