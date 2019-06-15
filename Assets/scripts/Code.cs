/*
using UnityEngine;
using System.Collections.Generic;

    public class Code : MonoBehaviour
    {
    public GameObject ob;
        public Material m_material;
    List<MyPoints2> mypoints = new List<MyPoints2>();

        public int seed = 0;

        List<GameObject> meshes = new List<GameObject>();

        void Start()
        {
            //Set the mode used to create the mesh.
            //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
            MyMarching marching = null;
            marching = new MyMarchingCubes();

            //Surface is the value that represents the surface of mesh
            //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
            //The target value does not have to be the mid point it can be any value with in the range.

            //The size of voxel array.
            int width = 1;
            int height = 1;
            int length = 1;

            //float[] voxels = new float[width * height * length];

            //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
            for (int x = 0; x < width/2; x++)
            {
                for (int y = 0; y < height/2; y++)
                {
                    for (int z = 0; z < length/2; z++)
                    {
                    MyPoints2 thepoint = new MyPoints2();
                    thepoint.addX(0.1f * x);
                    thepoint.addY(0.1f * y);
                    thepoint.addZ(0.1f * z);
                    mypoints.Add(thepoint);
                    }
                }
            }
            
            List<Vector3> verts = new List<Vector3>();
            List<int> indices = new List<int>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        //marching.Generate(, width, height, length, verts, indices);
        marching.Generate(true, mypoints, width, height, length, -1, -1, -1, 0.1f, verts, indices,ob);
            //A mesh in unity can only be made up of 65000 verts.
            //Need to split the verts between multiple meshes.
            
            int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
            int numMeshes = verts.Count / maxVertsPerMesh + 1;

            for (int i = 0; i < numMeshes; i++)
            {

                List<Vector3> splitVerts = new List<Vector3>();
                List<int> splitIndices = new List<int>();

                for (int j = 0; j < maxVertsPerMesh; j++)
                {
                    int idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Count)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject go = new GameObject("Mesh");
                go.transform.parent = transform;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = m_material;
                go.GetComponent<MeshFilter>().mesh = mesh;
                go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);

                meshes.Add(go);
            }
            
        }
        
   }
    */