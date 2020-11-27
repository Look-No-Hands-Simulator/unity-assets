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
	
	// receiving Thread
	Thread receiveThread;
	
	// udpclient object
	UdpClient client;
	
	// public
	// public string IP = "127.0.0.1"; default local
	public int port; // define > 8051
	
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
		// Endpunkt definieren, von dem die Nachrichten gesendet werden.
		print("UDPSend.init()");
		
		// define port
		port = 8051;
		
		// status
		// print("Sending to 127.0.0.1 : "+port);
		// print("Test-Sending to this Port: nc -u 127.0.0.1  "+port+"");
		
		
		// ----------------------------
		// Abh�ren
		// ----------------------------
		// Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
		// Einen neuen Thread f�r den Empfang eingehender Nachrichten erstellen.
		receiveThread = new Thread(
			new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		Debug.Log("Hey");

	}
	
	// receive thread
	private  void ReceiveData()
	{
		
		client = new UdpClient(port);
		while (true)
		{
			
			try
			{
				// Bytes empfangen.
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref anyIP);
				Debug.Log(BitConverter.ToDouble(data,0).ToString());
				Debug.Log("Recieving packetssss");
				double[] convertedData = new double[data.Length / 8];
				Debug.Log("Data  length: " + data.Length);

				Debug.Log("Converted  length: " + convertedData.Length);

				for (int ii = 0; ii < convertedData.Length; ii++)
				{
					
					convertedData[ii] = BitConverter.ToDouble(data, 8 * ii);
					Debug.Log("ConvERRR:" + (float)convertedData[ii]);
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
				Debug.Log("X = " + UDPData.xFloat);
				Debug.Log("Y = " + UDPData.yFloat);
				Debug.Log("Z = " + UDPData.zFloat);
				// ....
				allReceivedUDPPackets=allReceivedUDPPackets+text;
				
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
		if ( receiveThread!= null) 
			receiveThread.Abort(); 
		
	//	client.Close(); 
		print("client closed");
	} 

}
