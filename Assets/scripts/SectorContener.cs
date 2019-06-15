using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorContener{
    int sector;
    public List<MyPoints2> list = new List<MyPoints2>();
    public void addSector(int _sector)
    {
        sector = _sector;
    }
    public void addPoint(MyPoints2 p)
    {
        list.Add(p);
    }
    public int getSector()
    {
        return sector;
    }
}
