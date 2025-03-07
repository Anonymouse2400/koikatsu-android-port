using Illusion;
using UnityEngine;

internal class FussyTest : MonoBehaviour
{
	public float ReverseGrade;

	public float Trapezoid;

	public float Grade;

	public float Triangle;

	[Range(0f, 100f)]
	[SerializeField]
	private float value;

	[Range(0f, 100f)]
	[SerializeField]
	private float a;

	[SerializeField]
	[Range(0f, 100f)]
	private float b;

	[Range(0f, 100f)]
	[SerializeField]
	private float c;

	[SerializeField]
	[Range(0f, 100f)]
	private float d;

	private void OnValidate()
	{
		ReverseGrade = Utils.Math.Fuzzy.ReverseGrade(value, a, b);
		Trapezoid = Utils.Math.Fuzzy.Trapezoid(value, a, b, c, d);
		Grade = Utils.Math.Fuzzy.Grade(value, a, b);
		Triangle = Utils.Math.Fuzzy.Triangle(value, a, b, c);
	}
}
