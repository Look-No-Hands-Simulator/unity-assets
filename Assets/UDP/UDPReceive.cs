// REF http://forum.unity3d.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour {
	
	// car receiving Thread
	Thread carReceiveThread;
	
	//Cone position thread
	private Thread coneRecieveThread;
	
	// udpclient object UNUSED?
	UdpClient client;
	
	//Cone UDP client UNUSED?
	UdpClient coneClient;
	
	// public
	// public string IP = "127.0.0.1"; default local
	public int port; // define > 8051
	public int conePort; // define > 8051
	
	
	// infos
	public string lastReceivedUDPPacket="";
	public string allReceivedUDPPackets=""; // clean up this from time to time!
	
	
	// start from unity3d
	public void Start()
	{
		
		init();
	}
	
	// OnGUI

	/*/
	void OnGUI()
	{
		Rect rectObj=new Rect(40,10,200,400);
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		GUI.Box(rectObj,"# UDPReceive\n127.0.0.1 "+8051+" #\n"
		        + "shell> nc -u 127.0.0.1 : "+8051+" \n"
		        + "\nLast Packet: \n"+ lastReceivedUDPPacket
		        + "\n\nAll Messages: \n"+allReceivedUDPPackets
		        ,style);
	}
	/*/

	// init
	private void init()
	{
		
		// define port
		port = 8051;
		conePort = 8052;
		
		
		carReceiveThread = new Thread(
			new ThreadStart(ReceiveCarData));
		carReceiveThread.IsBackground = true;
		carReceiveThread.Start();
		
		coneRecieveThread = new Thread(
			new ThreadStart(ReceiveConeData));
		coneRecieveThread.IsBackground = true;
		coneRecieveThread.Start();
		

	}
	
	// receive thread
	private  void ReceiveCarData()
	{
		
		client = new UdpClient(port);
		while (true)
		{
			
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				//Debug.Log(BitConverter.ToDouble(data,0).ToString());
				//Debug.Log("Recieving packetssss");
				double[] convertedData = new double[data.Length / 8];
				//Debug.Log("Data  length: " + data.Length);

				//Debug.Log("Converted  length: " + convertedData.Length);

				for (int ii = 0; ii < convertedData.Length; ii++)
				{
					
					convertedData[ii] = BitConverter.ToDouble(data, 8 * ii);
					//Debug.Log("ConvERRR:" + (float)convertedData[ii]);
				}

				string text = Encoding.UTF8.GetString(data);



				UDPData.xFloat= (float)convertedData[0];
				UDPData.yFloat= (float)convertedData[2];
				UDPData.zFloat= (float)convertedData[1];
				//UDPData.pFloat= (float)convertedData[3];
				//UDPData.qFloat= (float)convertedData[4];
				//UDPData.rFloat= (float)convertedData[5];
				// latest UDPpacket
				lastReceivedUDPPacket=text;
				//Debug.Log("X = " + UDPData.xFloat);
				//Debug.Log("Y = " + UDPData.yFloat);
				//Debug.Log("Z = " + UDPData.zFloat);
				// ....
				allReceivedUDPPackets=allReceivedUDPPackets+text;
				
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}
	
	private  void ReceiveConeData()
	{
		
		client = new UdpClient(conePort);
		
		while (true)
		{
			
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				//Debug.Log(BitConverter.ToDouble(data,0).ToString());

				double[] convertedData = new double[data.Length / 8];
				Debug.Log("Data  length: " + data.Length);

				//Debug.Log("Converted  length: " + convertedData.Length);
				double blueCount = BitConverter.ToDouble(data, 0);
				double yellowCount =  BitConverter.ToDouble(data, 8);
				
				UDPData.blueCount = (int) blueCount;
				UDPData.yellowCount = (int) yellowCount;
				Debug.Log(blueCount);
				Debug.Log(yellowCount);
				
				UDPData.blueX = new float[(int)blueCount];
				UDPData.blueY = new float[(int)blueCount];
				UDPData.blueZ = new float[(int)blueCount];
				
				
				UDPData.yellowX = new float[(int)yellowCount];
				UDPData.yellowY = new float[(int)yellowCount];
				UDPData.yellowZ = new float[(int)yellowCount];

				//Debug.Log("TotalCount: " + (blueCount + yellowCount));
				for (int ii = 2; ii < ((blueCount + yellowCount)*3);  ii = ii+3)
				{
					if (ii > blueCount)
					{
						//Debug.Log("worked");
						UDPData.yellowX[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, ii *8));
						UDPData.yellowZ[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, (ii+1) *8));
						UDPData.yellowY[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, (ii+2)* 8));
						
			
					}
					else
					{
						UDPData.blueX[ii-2] = ((float) BitConverter.ToDouble(data, ii *8));
						UDPData.blueZ[ii-2] = ((float) BitConverter.ToDouble(data, (ii+1) *8));
						UDPData.blueY[ii-2] = ((float) BitConverter.ToDouble(data, (ii+2)* 8));
					}
					
				}

				string text = Encoding.UTF8.GetString(data);



				lastReceivedUDPPacket = text;
				allReceivedUDPPackets=allReceivedUDPPackets+text;
				
				
				if ( coneRecieveThread!= null) 
					coneRecieveThread.Abort();
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}
	
	// getLatestUDPPacket
	// cleans up the rest
	public string getLatestUDPPacket()
	{
		allReceivedUDPPackets="";
		return lastReceivedUDPPacket;
	}

	void OnDisable() 
	{ 
		if ( carReceiveThread!= null) 
			carReceiveThread.Abort(); 
		if ( coneRecieveThread!= null) 
			coneRecieveThread.Abort(); 
		
	//	client.Close(); 
		print("client closed");
	} 

}
