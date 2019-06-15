using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NetworkOperation : MonoBehaviour
{
    public bool packetReady;
    public List<MyPoints2> thePoints = new List<MyPoints2>();
    //public Text mytext;
    public Thread receiveThread;
    public int pakiety=0;
    int numberOfLines;
    string entireText = "";
    bool socketReady = false;                // global variables are setup here
    UdpClient client;
    public Int32 port;
    public String lastReceivedUDPPacket = "";
    public String allReceivedUDPPackets = "";

    /*-
    private static void Main()
    {
        NetworkOperation receiveObj = new NetworkOperation();
        receiveObj.init();
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    */
    // start from unity3d
    public void StartApp()
    {
        packetReady = false;
        Init();
    }

    
    public void OnApplicationQuit()
    {
        if(receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
    }

    public void StopThread()
    {
        Debug.Log("THREAD WYŁĄCZONY!");
        if(receiveThread!=null && receiveThread.IsAlive)
            receiveThread.Abort();
    }
    
    // receive thread
    private void ReceiveData()
    {
        Debug.Log("ReceiveData Uruchomiony!");
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            if (!packetReady)
            {
                byte[] data = client.Receive(ref anyIP);
                lastReceivedUDPPacket = BitConverter.ToString(data).Replace("-", String.Empty);
                allReceivedUDPPackets += lastReceivedUDPPacket;
                pakiety++;
                if (pakiety % 200 == 0)
                {
                    //Debug.Log("Dlugosc strumienia danych: " + allReceivedUDPPackets.Length);
                    StorePoints(allReceivedUDPPackets);
                    allReceivedUDPPackets = "";
                }
            }
        }
    }

    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        Debug.Log(allReceivedUDPPackets);
        return lastReceivedUDPPacket;
    }

    private void Init()
    {
        client = new UdpClient(port);
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        
    }

    void StorePoints(string data)
    {
        //Debug.Log("To jest String: "+data);
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
        //Debug.Log("ilosc punktow w NetworkOperations: "+thePoints.Count);
        packetReady = true;
    }

    public List<MyPoints2> ReturnPoints()
    {
        //packetReady = false;
        return thePoints;
    }

    public void ClearThePoints()
    {
        thePoints.Clear();
    }

    public void ChangeFlag()
    {
        packetReady = false;
    }

    public bool PacketReady()
    {
        return packetReady;
    }

    float HextoFloat(string hexString)
    {
        uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
        byte[] bytes = BitConverter.GetBytes(num);
        //bytes = convertFromMiddleEdian(bytes);
        float myFloat = BitConverter.ToSingle(bytes, 0);
        //Debug.Log(myFloat);
        return myFloat/100.0f;
    }

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
