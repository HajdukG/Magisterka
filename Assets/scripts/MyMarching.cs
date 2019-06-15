using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyMarching
{
    private float[,] Cube { get; set; }
    // Winding order of triangles use 2,1,0 or 0,1,2
    protected int[] WindingOrder { get; private set; }
    public float[] tableOfVert;

    /// Constructor
    public MyMarching()
    {
        Cube = new float[8,3];
        WindingOrder = new int[] { 0, 1, 2 };
    }
    ///<summary>
    /// Generating Grid 
    /// </summary>
    // FacingUP => normals  /points => list of points  /width => width of grid  /height => height of grid  /depth => depth of grid  /startX => starting point of grid OX
    // startY => starting point of grid OY /startZ => starting point of grid OZ  /scale => size of cubes to process   /verts => output verts   /indices => output indices
    public virtual void Generate(bool FaceingUP, int width, int height, int depth, List<SectorContener> SectorList, List<int> VectorList,
           float startX, float startY, float startZ, float scale, IList<Vector3> verts, IList<int> indices)
    {
        List<float[,]> listOfCubes = new List<float[,]>();

        if (FaceingUP)
        {
            WindingOrder[0] = 0;
            WindingOrder[1] = 1;
            WindingOrder[2] = 2;
        }
        else
        {
            WindingOrder[0] = 2;
            WindingOrder[1] = 1;
            WindingOrder[2] = 0;
        }
        for (int z = 1; z < (width/scale)*width+1; z++)
        {
            for (int y = 1; y < (height/scale)*height+1; y++)
            {
                for (int x = 1; x < (depth/scale)*depth+1; x++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Cube[i, 0] = startX + ((VertexOffset[i, 0] * scale)) + (x - 1) * scale;
                        Cube[i, 1] = startY + ((VertexOffset[i, 1] * scale)) + (y - 1) * scale;
                        Cube[i, 2] = startZ + ((VertexOffset[i, 2] * scale)) + (z - 1) * scale;
                        Debug.Log("Cube " + x + " " + y + " " + z + " i: " + i + ": ==       " + Cube[i, 0] + " ==       " +Cube[i, 1] + " ==       " + Cube[i, 2]);
                    }
                    listOfCubes.Add(Cube);
                    
                }
            }
        }

        March(listOfCubes, SectorList, verts, indices, scale, width);
    }

    protected abstract void March(List<float[,]> _listOfCubes, List<SectorContener> _SectorList, IList<Vector3> _vertList, IList<int> _indexList, float _scale, int _width);

    protected virtual float GetOffset(float v1, float v2)
    {
        return 0;
    }

    protected static readonly int[,] VertexOffset = new int[,]
        {
            {0, 0, 0},{1, 0, 0},{1, 1, 0},{0, 1, 0},
            {0, 0, 1},{1, 0, 1},{1, 1, 1},{0, 1, 1}
        };

   /* private int VertSelector(float[,] VsCube, List<SectorContener> _SectorList, float _scale, int _width)
    {
        int flagIn = 0;
        int scaleNumber = (int)(_width / _scale);
        int levelNumber = scaleNumber * scaleNumber;
        bool[] vts = new bool[8];
        for (int h = 0; h < 8; ++h)
        {
            vts[h] = false;
        }
        for (int xz = 0; xz < _SectorList.Count; ++xz)
        {
            if (sector.getSector() != _SectorList[xz].getSector())
            {
                int sectorGetSector = sector.getSector();

                int CornerA = sector.getSector() - levelNumber - scaleNumber + 1;
                int CornerB = sector.getSector() - levelNumber - scaleNumber - 1;
                int CornerC = sector.getSector() - levelNumber + scaleNumber + 1;
                int CornerD = sector.getSector() - levelNumber + scaleNumber - 1;
                int CornerE = sector.getSector() + levelNumber - scaleNumber + 1;
                int CornerF = sector.getSector() + levelNumber - scaleNumber - 1;
                int CornerG = sector.getSector() + levelNumber + scaleNumber + 1;
                int CornerH = sector.getSector() + levelNumber + scaleNumber - 1;

                int MiddleA = sector.getSector() - levelNumber - 1;
                int MiddleB = sector.getSector() - levelNumber + 1;
                int MiddleC = sector.getSector() - levelNumber - scaleNumber;
                int MiddleD = sector.getSector() - levelNumber + scaleNumber;
                int MiddleE = sector.getSector() + levelNumber - 1;
                int MiddleF = sector.getSector() + levelNumber + 1;
                int MiddleG = sector.getSector() + levelNumber - scaleNumber;
                int MiddleH = sector.getSector() + levelNumber + scaleNumber;
                int MiddleI = sector.getSector() + 1 - scaleNumber;
                int MiddleJ = sector.getSector() - 1 - scaleNumber;
                int MiddleK = sector.getSector() + 1 + scaleNumber;
                int MiddleL = sector.getSector() - 1 + scaleNumber;

                int CenterC = sector.getSector() + scaleNumber;
                int CenterD = sector.getSector() - scaleNumber;
                int CenterB = sector.getSector() + 1;
                int CenterA = sector.getSector() - 1;
                int CenterE = sector.getSector() + levelNumber;
                int CenterF = sector.getSector() - levelNumber;

                if (_SectorList[xz].getSector() == CenterA)
                {
                    for (int z = 0; z < 4; ++z)
                    {
                        vts[z] = true;
                    }
                }
                else if (_SectorList[xz].getSector() == CenterB)
                {
                    for (int z = 4; z < 8; ++z)
                    {
                        vts[z] = true;
                    }
                }
                else if (_SectorList[xz].getSector() == CenterC)
                {
                    vts[1] = true;
                    vts[2] = true;
                    vts[5] = true;
                    vts[6] = true;
                }
                else if (_SectorList[xz].getSector() == CenterD)
                {
                    vts[0] = true;
                    vts[3] = true;
                    vts[4] = true;
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == CenterE)
                {
                    vts[2] = true;
                    vts[3] = true;
                    vts[6] = true;
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == CenterF)
                {
                    vts[0] = true;
                    vts[1] = true;
                    vts[4] = true;
                    vts[5] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleA)
                {
                    vts[0] = true;
                    vts[1] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleB)
                {
                    vts[4] = true;
                    vts[5] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleC)
                {
                    vts[4] = true;
                    vts[0] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleD)
                {
                    vts[1] = true;
                    vts[5] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleE)
                {
                    vts[2] = true;
                    vts[3] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleF)
                {
                    vts[6] = true;
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleG)
                {
                    vts[3] = true;
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleH)
                {
                    vts[2] = true;
                    vts[6] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleI)
                {
                    vts[4] = true;
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleJ)
                {
                    vts[0] = true;
                    vts[3] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleK)
                {
                    vts[6] = true;
                    vts[5] = true;
                }
                else if (_SectorList[xz].getSector() == MiddleL)
                {
                    vts[1] = true;
                    vts[2] = true;
                }
                else if (_SectorList[xz].getSector() == CornerA)
                {
                    vts[4] = true;
                }
                else if (_SectorList[xz].getSector() == CornerB)
                {
                    vts[0] = true;
                }
                else if (_SectorList[xz].getSector() == CornerC)
                {
                    vts[5] = true;
                }
                else if (_SectorList[xz].getSector() == CornerD)
                {
                    vts[1] = true;
                }
                else if (_SectorList[xz].getSector() == CornerE)
                {
                    vts[7] = true;
                }
                else if (_SectorList[xz].getSector() == CornerF)
                {
                    vts[3] = true;
                }
                else if (_SectorList[xz].getSector() == CornerG)
                {
                    vts[6] = true;
                }
                else if (_SectorList[xz].getSector() == CornerH)
                {
                    vts[2] = true;
                }
            }
        }
        for (int ab = 0; ab < 8; ab++) if (vts[ab] == true) flagIn |= 1 << ab;
        return flagIn;
    }
    */
}
