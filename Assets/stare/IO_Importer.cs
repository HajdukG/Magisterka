using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections;
public class IO_Importer : MonoBehaviour
{
    public Transform pointer; //Object passed as a display point
    [Range(0.1f, 5.0f)]
    public float scale = 1; //Scale of display
    public string fileToAnalise = "/data.txt";
    MyPoints allPoints = new MyPoints(); //Seperate class. Contains all points as list of X's, Y's and Z's.
    string readPath; //Contains information about the location of the file.
    string entireText = ""; //String containing entire data for visualisation
    Regex regex; //Regex object
    MatchCollection matches; //Used in Regex for matching 
    int numberOfUndisplayedPoints; //Shows how many points are out of bounds
    public float uEpsilon = 200; //A boundary beyond which points will not be displayed

    void Start() //At the start of the program
    {
        numberOfUndisplayedPoints = 0;
        readPath = Application.dataPath + fileToAnalise;
        ReadFile(readPath, false); //Reading data.
                                  //StartCoroutine(ShowPoints());
        ShowPoints();
    }

    /*
    private void OnValidate()
    {
        ShowPoints();
    }
    */

    /*
	* =========================================================
	* Function reads data stored in file specified in readPath 
	* =========================================================
	*/
    void ReadFile(string filePath, bool hex)
    {
        string line = "";
        StreamReader stream = new StreamReader(filePath);
        line = stream.ReadLine();
        while (line != null)
        {
            line.Trim();
            //Debug.Log(line);
            entireText += line + " ";
            line = stream.ReadLine();
        }
        stream.Close();
        if (hex)
            StorePoints(entireText);
        else
            StorePointsWithoutHex(entireText);
    }
    /*
     * =========================================================
     *    Function gathers data about points into X,Y,Z lists
     * =========================================================
     */
    void StorePoints(string data)
    {
        string pattern = "\\s([a-z0-9]{4})\\s([a-z0-9]{4})"; //Regex patern
        regex = new Regex(pattern);
        int temp = 0;
        matches = regex.Matches(data);
        string tempString;
        for (int k = 0; k < matches.Count; ++k)
        {
            tempString = matches[k].ToString();
            tempString = tempString.Trim();
            tempString = tempString.Replace(" ", string.Empty);
            if (!tempString.Contains("abcdabcd"))
            {
                float decValue = HextoFloat(tempString);
                switch (temp)
                {
                    case 0:
                        allPoints.addX(decValue); temp = 1;
                        break;
                    case 1:
                        allPoints.addY(decValue); temp = 2;
                        break;
                    case 2:
                        allPoints.addZ(decValue); temp = 0;
                        break;
                }
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
        string pattern = @"-?\d+(?:\.\d+)?"; //Regex patern
        regex = new Regex(pattern);
        int temp = 0;
        matches = regex.Matches(data);
        string tempString;
        for (int k = 0; k < matches.Count; ++k)
        {
            tempString = matches[k].ToString();
            tempString = tempString.Trim();
            float decValue = HextoFloat(tempString);
            Debug.Log("String: " + tempString + " Float: " + decValue);
            switch (temp)
            {
                case 0:
                    allPoints.addX(decValue); temp = 1;
                    break;
                case 1:
                    allPoints.addY(decValue); temp = 2;
                    break;
                case 2:
                    allPoints.addZ(decValue); temp = 0;
                    break;
            }
        }

    }
    /*
     * =========================================================
     *    Function displays points
     * =========================================================
     */
    int ShowPoints()
    {
        /*
         * TO DO:
         * USUNIĘCIE WSZYSTKICH PUNKTÓW
         */
        if (allPoints.ValidatePoints())
        {
            float tempX;
            float tempY;
            float tempZ;
            Quaternion qua = new Quaternion(0, 0, 0, 0);
            float numpoits = allPoints.AmountOfPoints();
            for (int x = 0; x < numpoits; ++x)
            {
                tempX = allPoints.getX(x) * scale;
                tempY = allPoints.getY(x) * scale;
                tempZ = allPoints.getZ(x) * scale;
                if (!IsNanCheckVector(tempX, tempY, tempZ) && !GraterThenuEps(tempX, tempY, tempZ))
                {
                    //Debug.Log("Point added: " + tempX + " " + tempY + " " + tempZ);
                    Vector3 vec = new Vector3(tempX, tempY, tempZ);
                    Instantiate(pointer, vec, qua);
                }
                else
                {
                    numberOfUndisplayedPoints++;
                }
                if (x == numpoits)
                {
                    return 0;
                }
            }

            //Debug.Log("Points out of boundary: " + numberOfUndisplayedPoints);
        }
        return -1;
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
        bytes = convertFromMiddleEdian(bytes);
        float myFloat = BitConverter.ToSingle(bytes, 0);
        //Debug.Log("Hex value : " + myFloat);
        return myFloat;
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
}
