//REF: http://forum.unity3d.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > transmitted under
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
	private static int localPort;
	
	// prefs
	private string IP;  // define in init
	public int port;  // define in init
	
	// "connection" things
	IPEndPoint remoteEndPoint;
	UdpClient client;
	
	// gui
	string strMessage="";
	
	
	// call it from shell (as program)
	private static void Main()
	{
		UDPSend sendObj=new UDPSend();
		sendObj.init();
		
		// testing via console
		// sendObj.inputFromConsole();
		
		// as server sending endless
		sendObj.sendEndless(" endless infos \n");
		
	}
	// start from unity3d
	public void Start()
	{
		init();
	}
	
	// OnGUI
	void OnGUI()
	{
		Rect rectObj=new Rect(40,380,200,400);
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		GUI.Box(rectObj,"# UDPSend-Data\n127.0.0.1 "+port+" #\n"
		        + "shell> nc -lu 127.0.0.1  "+port+" \n"
		        ,style);
		
		// ------------------------
		// send it
		// ------------------------
		strMessage=GUI.TextField(new Rect(40,420,140,20),strMessage);
		if (GUI.Button(new Rect(190,420,40,20),"send"))
		{
			sendString(strMessage+"\n");
		}      
	}
	
	// init
	public void init()
	{
		// Define end point , from which the messages are sent.
		print("UDPSend.init()");
		
		// define
		IP="127.0.0.1";
		port=8050;
		
		// ----------------------------
		// Send
		// ----------------------------
		remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
		client = new UdpClient();
		
		// status
		print("Sending to "+IP+" : "+port);
		print("Testing: nc -lu "+IP+" : "+port);
		sendString("jt");
	}
	
	// inputFromConsole
	private void inputFromConsole()
	{
		try
		{
			string text;
			do
			{
				text = Console.ReadLine();
				
				// Send the text to the remote client .
				if (text != "")
				{
					
					// Encode data using the UTF8 encoding to binary format.
					byte[] data = Encoding.UTF8.GetBytes(text);
					
					// Send the text to the remote client.
					client.Send(data, data.Length, remoteEndPoint);
				}
			} while (text != "");
		}
		catch (Exception err)
		{
			print(err.ToString());
		}
		
	}
	
	// sendData
	private void sendString(string message)
	{
		try
		{
			//if (message != "")
			//{
			
			// Encode data using the UTF8 encoding to binary format.
			byte[] data = Encoding.UTF8.GetBytes(message);
			
			// Send the message to the remote client.
			client.Send(data, data.Length, remoteEndPoint);
			//}
		}
		catch (Exception err)
		{
			print(err.ToString());
		}
	}
	
	
	// endless test
	private void sendEndless(string testStr)
	{
		do
		{
			sendString(testStr);
			
			
		}
		while(true);
		
	}
	
}

