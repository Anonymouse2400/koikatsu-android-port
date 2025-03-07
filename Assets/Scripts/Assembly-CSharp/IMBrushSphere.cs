using UnityEngine;

public class IMBrushSphere : IMBrush
{
	public float radius = 1f;

	protected override void DoDraw()
	{
		im.AddSphere(base.transform, radius, base.PowerScale, fadeRadius);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		if (im != null)
		{
			Gizmos.color = ((!bSubtract) ? Color.white : Color.red);
			float powerThreshold = im.powerThreshold;
			float num = Mathf.Lerp(radius, radius - fadeRadius, powerThreshold);
			Gizmos.DrawWireSphere(Vector3.zero, num);
		}
	}
}
