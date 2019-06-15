using System.Collections.Generic;
using UnityEngine;
public class MyPoints{
	private static List<float> xPoints = new List<float>();
	private static List<float> yPoints = new List<float>();
	private static List<float> zPoints = new List<float>();
	public void addX(float x)
	{
		xPoints.Add(x);
	}
	public void addY(float y)
	{
		yPoints.Add(y);
	}
	public void addZ(float z)
	{
		zPoints.Add(z);
	}

	public float getX(int i)
	{
		return xPoints[i];
	}
	public float getY(int i)
	{
		return yPoints[i];
	}
	public float getZ(int i)
	{
		return zPoints[i];
	}

	public bool ValidatePoints()
	{
		if (xPoints.Count == yPoints.Count && yPoints.Count == zPoints.Count)
			return true;
		else
			return false;
	}

	public int AmountOfPoints()
	{
		return xPoints.Count;
	}
}
