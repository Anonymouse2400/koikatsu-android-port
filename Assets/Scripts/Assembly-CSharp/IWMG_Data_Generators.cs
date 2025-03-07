using System.Collections.Generic;
using UnityEngine;

public interface IWMG_Data_Generators
{
	List<Vector2> GenLinear(int numPoints, float minX, float maxX, float a, float b);

	List<Vector2> GenQuadratic(int numPoints, float minX, float maxX, float a, float b, float c);

	List<Vector2> GenExponential(int numPoints, float minX, float maxX, float a, float b, float c);

	List<Vector2> GenLogarithmic(int numPoints, float minX, float maxX, float a, float b, float c);

	List<Vector2> GenCircular(int numPoints, float a, float b, float c);

	List<Vector2> GenCircular2(int numPoints, float a, float b, float c, float degreeOffset);

	List<Vector2> GenRadar(List<float> data, float a, float b, float degreeOffset);

	List<Vector2> GenRandomXY(int numPoints, float minX, float maxX, float minY, float maxY);

	List<Vector2> GenRandomY(int numPoints, float minX, float maxX, float minY, float maxY);

	List<float> GenRandomList(int numPoints, float min, float max);
}
