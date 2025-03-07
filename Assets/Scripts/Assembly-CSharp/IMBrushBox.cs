using UnityEngine;

public class IMBrushBox : IMBrush
{
	public Vector3 extents = Vector3.one;

	protected override void DoDraw()
	{
		im.AddBox(base.transform, extents, base.PowerScale, fadeRadius);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Vector3.zero, extents * 2f);
		if (im != null)
		{
			Gizmos.color = ((!bSubtract) ? Color.white : Color.red);
			float powerThreshold = im.powerThreshold;
			Gizmos.DrawWireCube(size: new Vector3(Mathf.Lerp(extents.x, extents.x - fadeRadius, powerThreshold), Mathf.Lerp(extents.y, extents.y - fadeRadius, powerThreshold), Mathf.Lerp(extents.z, extents.z - fadeRadius, powerThreshold)) * 2f, center: Vector3.zero);
		}
	}
}
