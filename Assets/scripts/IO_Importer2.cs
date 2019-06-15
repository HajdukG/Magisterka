using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

public class IO_Importer2 : MonoBehaviour {
    public Material _material;
	public Transform pointer; //Object passed as a display point
	[Range(0.01f,1.0f)]
	public float scale = 0.1f; //Scale of display
    public float areaOfClearance;
    public string fileToAnalise = "/pointsReduced.txt";
	string readPath; //Contains information about the location of the file.
	string entireText = ""; //String containing entire data for visualisation
	Regex regex; //Regex object
	MatchCollection matches; //Used in Regex for matching 
	int numberOfUndisplayedPoints; //Shows how many points are out of bounds
	public float uEpsilon = 200; //A boundary beyond which points will not be displayed
    Renderer rend;
    public List<MyPoints2> thePoints = new List<MyPoints2>();
    List<Transform> objList;
    int deletedPoints = 1;
    List<GameObject> meshes = new List<GameObject>();
    int width = 1;
    int height = 1;
    int length = 1;
    List<Vector3> verts = new List<Vector3>();
    List<int> indices = new List<int>();
    int maxSector,aMaxSector;
    int numberOfSectors;
    List<int> vecSectors = new List<int>();
    List<SectorContener> listOfContainters = new List<SectorContener>();

    NetworkOperation networkOperation;
    public DefaultTrackableEventHandler dteh;
    public bool threadedInterface;
    public String threadedData="";
    int iterations = 0;
    public bool storeDataIsHex = false;

    EliminateCompetition[] elimination;
    public int maxEliminated = 0;
    public int childCount = 0;

    bool debugBool = true;
    public Transform debugObject;
    public double debugTimerCounter = 0;

    void Start ()
	{
        dteh = GetComponent<DefaultTrackableEventHandler>();
        networkOperation = GetComponent<NetworkOperation>();
        int debugInt = 0;
        maxSector = 0;
        aMaxSector = 0;
        //MyMarching marching = null;
        //marching = new MyMarchingCubes();

        objList = new List<Transform>();
        numberOfUndisplayedPoints = 0;
		readPath = Application.dataPath + fileToAnalise;

        //==================CZYTANIE DANYCH==================
        double readFile1 = Time.realtimeSinceStartup;
        ReadFile(readPath, storeDataIsHex);
        double readFile2 = Time.realtimeSinceStartup;
        Debug.Log("ReadFile: " + (readFile2 - readFile1));
        //===================================================

        if (!threadedInterface)
        {
            Debug.Log("=========Startowa liczba punktów: " + thePoints.Count);
            double timeStart = Time.realtimeSinceStartup;
            //thePoints.Sort((x, y) => x.getX().CompareTo(y.getX()));//Sort data
            double t3 = Time.realtimeSinceStartup;
           /* while (deletedPoints != 0)
            {
                ClearDuplicates(areaOfClearance); //Clear duplicated data
                ++debugInt;
            } */
            double t4 = Time.realtimeSinceStartup;
           // Debug.Log("Usunięto " + debugInt + " razy wszystkie duplikaty");

            double t5 = Time.realtimeSinceStartup;
            ShowPoints();
            double t6 = Time.realtimeSinceStartup;

            

          //  Debug.Log("Czas ClearDuplicates: " + (t4 - t3) + " sekund");
            Debug.Log("Czas ShowPoints: " + (t6 - t5) + " sekund");
           // Debug.Log("CZAS OSTATECZNY: " + (t6 - timeStart));
           // Debug.Log("Czas na obliczanie normalnych " + debugTimerCounter);
            //dteh.OnTrackingLost();
        }
	}

    private void OnApplicationQuit()
    {
        networkOperation.StopThread();
    }

    private void Update()
    {
        if (threadedInterface)
        {
            //double timeStart = Time.realtimeSinceStartup;
            if (networkOperation.PacketReady())
            {
                thePoints = networkOperation.ReturnPoints();
                //thePoints.Sort((x, y) => x.getX().CompareTo(y.getX()));
                ShowPoints();
                networkOperation.ChangeFlag();
                networkOperation.ClearThePoints();
            }
            Debug.Log("Faktyczna liczba obiektów na scenie: " + debugObject.childCount);
            childCount = debugObject.childCount;
            
        }
        else
        {
            if(debugBool)
            {
                if(debugObject.childCount!=0)
                {
                    Debug.Log(debugObject.childCount);
                    debugBool = false;
                    double t7 = Time.realtimeSinceStartup;
                    elimination = GetComponentsInChildren<EliminateCompetition>();
                    foreach (EliminateCompetition e in elimination)
                    {
                        maxEliminated = maxEliminated < e.deleted ? e.deleted : maxEliminated;
                    }
                    double t8 = Time.realtimeSinceStartup;
                    Debug.Log("czas maxEliminated: " + (t8 - t7));
                }
                
            }
        }
    }

    /*
	* =========================================================
	* Function reads data stored in file specified in readPath 
	* =========================================================
	*/
    public void ReadFile(string filePath, bool hex)
    {
        if (threadedInterface)
        {
            networkOperation.StartApp();
        }
        else
        {
            string line = "";
            StreamReader stream = new StreamReader(filePath);
            line = stream.ReadLine();
            while (line != null)
            {
                line.Trim();
                entireText += line + " ";
                line = stream.ReadLine();
            }
            stream.Close();
            if (hex)
                StorePoints(entireText);
            else
                StorePointsWithoutHex(entireText);
        }
    }
    /*
     * =========================================================
     *    Function gathers data about points into X,Y,Z lists
     * =========================================================
     */
    void StorePoints(string data)
	{
        //Debug.Log("StorePoints");
        string pattern = "([A-Z0-9]{8})"; //Regex patern
        Regex regex = new Regex(pattern);
        int temp = 0;
        MatchCollection matches = regex.Matches(data);
        //Debug.Log("Matches found: "+matches.Count);
        string tempString;
        MyPoints2 currentPoint = new MyPoints2();
        for (int k = 0; k < matches.Count; ++k)
        {
            tempString = matches[k].ToString();
            tempString = tempString.Trim();
            tempString = tempString.Replace(" ", string.Empty);
            if (!tempString.Contains("ABCDABCD") && !tempString.Contains("CD000000"))
            {
                float decValue = HextoFloat(tempString);
                switch (temp)
                {
                    case 0:
                        currentPoint.addX(decValue); temp = 1;
                        break;
                    case 1:
                        currentPoint.addY(decValue); temp = 2;
                        break;
                    case 2:
                        currentPoint.addZ(decValue);
                        temp = 0;
                        thePoints.Add(currentPoint);
                        currentPoint = new MyPoints2();
                        break;
                }
            }
        }

	}
    void PrepareSectors()
    {
        bool isAlreadyThere = false;
        foreach (MyPoints2 p in thePoints)
        {
            foreach (int i in vecSectors)
            {
                if (i == p.sector)
                {
                    isAlreadyThere = true;
                    foreach (SectorContener sc in listOfContainters)
                    {
                        if (sc.getSector() == i)
                            sc.addPoint(p);
                    }
                }
            }
            if (!isAlreadyThere)
            {
                vecSectors.Add(p.sector);
                SectorContener contener = new SectorContener();
                contener.addSector(p.sector);
                contener.addPoint(p);
                listOfContainters.Add(contener);
            }
            isAlreadyThere = false;
        }
        
        //Debug.Log("Liczba vecSectorów: " + vecSectors.Count + " Liczba SectorContenerów: " + listOfContainters.Count);
    }

    /*
      * =============================================================================
      *    Function normalizes the points
      * =============================================================================
      */
    void NormalizePoints(List<MyPoints2> listPoints)
    {
        List<MyPoints2> thePointsY = listPoints;
        thePointsY.Sort((x, y) => x.getY().CompareTo(y.getY()));
        List<MyPoints2> thePointsZ = listPoints;
        thePointsZ.Sort((x, y) => x.getZ().CompareTo(y.getZ()));
        float minX = listPoints[0].getX();
        float maxX = listPoints[listPoints.Count - 1].getX();
        float minY = thePointsY[0].getY();
        float maxY = thePointsY[listPoints.Count - 1].getY();
        float minZ = thePointsZ[0].getZ();
        float maxZ = thePointsZ[listPoints.Count - 1].getZ();
        float minV = minX < minY ? minX : minY;
        float min = minV < minZ ? minV : minZ;
        float maxV = maxX > maxY ? maxX : maxY;
        float max = maxV > maxZ ? maxV : maxZ;
        foreach (MyPoints2 p in listPoints)
        {
            float newValueX = (p.getX() - min) / (max - min);
            p.addX(newValueX);
            float newValueY = (p.getY() - min) / (max - min);
            p.addY(newValueY);
            float newValueZ = (p.getZ() - min) / (max - min);
            p.addZ(newValueZ);
            int OneScaled = Mathf.FloorToInt((1f / scale));
            int theX = Mathf.FloorToInt(p.getX() / scale);
            int theY = Mathf.FloorToInt((p.getY() / scale));
            int theZ = Mathf.FloorToInt((p.getZ() / scale));
            p.sector = theX + (theY * OneScaled) + (theZ * OneScaled * OneScaled);//x+y*a+z*a^2
            if (p.sector > maxSector)
            {
                aMaxSector = maxSector;
                maxSector = p.sector;
            }
        }
    }
    /*
     * =============================================================================
     *    Function gathers data about points into X,Y,Z lists from txt without hex
     * =============================================================================
     */
    void StorePointsWithoutHex(string data)
    {
        string pattern = @"-?\d+(?:\.\d+)?";
        regex = new Regex(pattern);
        int temp = 0;
        matches = regex.Matches(data);
        string tempString;
        MyPoints2 currentPoint = new MyPoints2();
        for (int k = 0; k < matches.Count; ++k)
        {
            tempString = matches[k].ToString();
            tempString = tempString.Trim();
            tempString = tempString.Replace(" ", string.Empty);
            if (!tempString.Contains("ABCDABCD"))
            {
                float decValue = float.Parse(tempString);
                switch (temp)
                {
                    case 0:
                        currentPoint.addX(decValue); temp = 1;
                        break;
                    case 1:
                        currentPoint.addY(decValue); temp = 2;
                        break;
                    case 2:
                        currentPoint.addZW(decValue,0); temp = 0;
                        thePoints.Add(currentPoint);
                        currentPoint = new MyPoints2();
                        break;
                }
            }
        }

    }
    /*
    * =========================================================
    *    Function destroys all points
    * =========================================================
    */
    void DestroyPoints()
    {
        GameObject[] listOfPoints = GameObject.FindGameObjectsWithTag("Point");
        if (listOfPoints != null)
        {
            for (int p = 0; p < listOfPoints.Length; ++p)
            {
                Destroy(listOfPoints[p]);
            }
        }
    }
    /*
     * =========================================================
     *    Function displays points
     * =========================================================
     */
    void ShowPoints()
    {      
        int numOfPoints=0;
        float tempX;
        float tempY;
        float tempZ;
        float minX = 0;
        Quaternion qua = new Quaternion(0, 0, 0, 0);
        int numpoits = thePoints.Count;
        for (int x = 0; x < numpoits; ++x)
        {
            if (x == 0)
                minX = thePoints[x].getX();

            tempX = thePoints[x].getX();
            tempY = thePoints[x].getY();
            tempZ = thePoints[x].getZ();

            if (!IsNanCheckVector(tempX, tempY, tempZ) && !GraterThenuEps(tempX, tempY, tempZ) && IsNotAnError(thePoints[x],maxSector))
            {
                Vector3 vec = new Vector3(tempX, tempY, tempZ);
                Transform obj = Instantiate(pointer, vec, qua);
                //obj.rotation = pointer.rotation;
                obj.name = "object " + x;
               // double t1 = Time.realtimeSinceStartup;
               // obj.transform.rotation = Quaternion.LookRotation(calculateNormal(obj.transform.position, 2f));
               // double t2 = Time.realtimeSinceStartup;
               // debugTimerCounter += (t2 - t1);
                obj.parent = transform;
                numOfPoints += 1;
                float RColor = thePoints[x].getW() / 255f;
                Color colorOfPoint = new Color(RColor, 0f, 1f, 1f);
                obj.GetComponent<Renderer>().material.SetColor("_Color", colorOfPoint);
                //objList.Add(obj);
            }
            else
            {
                numberOfUndisplayedPoints++;
            }
        }
        //double t1 = Time.realtimeSinceStartup;
        //int dagain = ReduceNumberOfExistingPoints();
        //double t2 = Time.realtimeSinceStartup;
        //Debug.Log("Liczba wyświetlających punktów: " + (numOfPoints-dagain));
        //Debug.Log("Czas ReduceNumberOfExistingPoints: " + (t2 - t1));
    }
    
    int ReduceNumberOfExistingPoints()
    {
        int deletedAgain = 0;
        float numpoits = objList.Count;
        for (int x = 0; x < numpoits; ++x)
        {
            Vector3 myPosition = objList[x].gameObject.transform.position;
            rend = objList[x].gameObject.GetComponent<Renderer>();
            for (int xa = x + 1; xa < numpoits; xa++)
            {
                Vector3 diff = objList[xa].gameObject.transform.position - myPosition;
                float diffMag = diff.sqrMagnitude;
                if (diffMag < areaOfClearance*areaOfClearance)
                {
                    Destroy(objList[xa].gameObject);
                    objList.RemoveAt(xa);
                    numpoits = objList.Count;
                    deletedAgain++;
                }
            }
        }
        return deletedAgain;
    }
    /*
     * =========================================================
     *    Function clears duplicates from within the range
     * =========================================================
     */
    void ClearDuplicates(float area)
	{
        int j;
        deletedPoints = 0;
        for (int i=0; i<thePoints.Count;++i)
        {
            j = i + 1;
            if (j >= thePoints.Count)
                break;
            while (Compare(thePoints[i].getX(), thePoints[j].getX(), area))
            {
                if(Compare(thePoints[i].getY(), thePoints[j].getY(), area)
                && Compare(thePoints[i].getZ(), thePoints[j].getZ(), area))
                {
                    thePoints.RemoveAt(j);
                    deletedPoints += 1;
                    if(thePoints[i].getW()<=205)
                        thePoints[i].addW(thePoints[i].getW() + 50);
                }
                j++;
                if (j >= thePoints.Count)
                    break;
            }
        }
	}
    /*
     * =========================================================
     *    Function returns true if point is a duplicate
     * =========================================================
     */
    bool Compare(float x, float y, float REPS)
    {
        if (Mathf.Abs(x-y)<REPS)
            return true;
        else
            return false;
    }

	/*
     * =========================================================
     *    Function converts Hex into Float
     * =========================================================
     */
	float HextoFloat(string hexString)
	{
		uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
		byte[] bytes = BitConverter.GetBytes(num);
		//bytes = convertFromMiddleEdian(bytes);
		float myFloat = BitConverter.ToSingle(bytes, 0);
		//Debug.Log("Hex value : " + myFloat);
        return myFloat / 100;
	}
	/*
     * =========================================================
     *    Function checks if generated float is a NaN
     * =========================================================
     */
	bool IsNanCheck(float number)
	{
		if (float.IsNaN(number))
			return true;
		else
			return false;
	}
	/*
     * =========================================================
     *    Function checks if array of float numbers are NaN
     * =========================================================
     */
	bool IsNanCheckVector(float one, float two, float three)
	{
		if (!IsNanCheck(one) && !IsNanCheck(two) && !IsNanCheck(three))
			return false;
		else
			return true;
	}
	/*
     * =========================================================
     *    Function checks if points are outside of boundary
     * =========================================================
     */
	bool GraterThenuEps(float a, float b, float c)
	{
		return (Mathf.Abs(a) > uEpsilon || Mathf.Abs(b) > uEpsilon || Mathf.Abs(c) > uEpsilon) ? true : false;
	}
	/*
     * =========================================================
     *    Function converts byte value from Middle Edian
     * =========================================================
     */
	byte[] convertFromMiddleEdian(byte[] bytes)
	{
		byte[] returnTable = new byte[4];
		returnTable[0] = bytes[2];
		returnTable[1] = bytes[3];
		returnTable[2] = bytes[0];
		returnTable[3] = bytes[1];
		return returnTable;
	}

    bool IsNotAnError(MyPoints2 p, int max)
    {
        if (p.sector <= max && p.sector >= 0)
        {
            //Debug.Log(p.sector + " <= " + max);
            return true;
        }
        else
            return false;
    }

    private Vector3 calculateNormal(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 1;
        var bounds = new Bounds(hitColliders[0].transform.position, Vector3.zero);
        while (i < hitColliders.Length)
        {
            bounds.Encapsulate(hitColliders[i].transform.position);
            i++;
        }
        Vector3 returningVector = bounds.center;
        returningVector.Normalize();
        return returningVector;
    }

    void Deploy()
    {
        Debug.Log("Deploy start---");
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
            go.GetComponent<Renderer>().material = _material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);

            meshes.Add(go);
        }
        Debug.Log("Deployed!");
    }
}
