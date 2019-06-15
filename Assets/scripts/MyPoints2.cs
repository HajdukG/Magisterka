using System.Collections.Generic;
using UnityEngine;
public class MyPoints2{
	float oX;
	float oY;
	float oZ;
    float weight;
    public int sector;

	public void addX(float x)
	{
		oX = x;
	}
	public void addY(float y)
	{
		oY = y;
	}
	public void addZ(float z)
	{
		oZ = z;
	}
    public void addZW(float z, float w)
    {
        oZ = z;
        weight = 0;
    }
    public void addW(float w)
    {
        weight = w;
    }
    public float getX()
	{
		return oX;
	}
	public float getY()
	{
		return oY;
	}
	public float getZ()
	{
		return oZ;
	}
    public float getW()
    {
        return weight;
    }

}
