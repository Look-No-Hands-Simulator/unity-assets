using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour {
	
	
	// Car position/rotation thread
	private Thread carReceiveThread;
	
	//Cone position thread
	private Thread coneRecieveThread;
	
	//UDP client for car pose
	UdpClient carDataClient;
	
	//UDP client for cone pose
	UdpClient coneClient;
	
	public int port; // define > 8051
	public int conePort; 
	
	
	// start from unity3d
	public void Start()
	{
		
		init();

	}
	
	// init
	private void init()
	{
		
		// define ports
		port = 8051;
		conePort = 8052;
		
		//Start threads for both UDP receivers
		carReceiveThread = new Thread(
			new ThreadStart(ReceiveCarData));
		carReceiveThread.IsBackground = false;
		carReceiveThread.Start();
		
		coneRecieveThread = new Thread(
			new ThreadStart(ReceiveConeData));
		coneRecieveThread.IsBackground = true;
		coneRecieveThread.Start();
	


	}
	
	// Car pose receive thread
	private  void ReceiveCarData()
	{
		//Create UDP client on port 8051
		carDataClient = new UdpClient(port);
		
		while (true)
		{
			
			try
			{
				//Set up UDP client to recieve packets
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = carDataClient.Receive(ref anyIP);

				
				//Create array for the data in the packet. Each var is is a double -> 8 bytes long
				double[] convertedData = new double[data.Length / 8];
				
				//Loop through packet and store in array
				for (int ii = 0; ii < convertedData.Length; ii++)
				{
					
					convertedData[ii] = BitConverter.ToDouble(data, 8 * ii);
				}

				string text = Encoding.UTF8.GetString(data);


				//Set position data from array - y and z are flipped due to different reference planes
				UDPData.xFloat= (float)convertedData[0];
				UDPData.yFloat= (float)convertedData[2];
				UDPData.zFloat= (float)convertedData[1];
				
				//Set rotation data form array - q and r are flipped again due to different reference planes
				UDPData.pFloat= (float)convertedData[3];
				UDPData.qFloat= (float)convertedData[5];
				UDPData.rFloat= (float)convertedData[4];
				UDPData.wFloat= (float)convertedData[6];
				
			}
			//Handle errors in reception
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}
	
	
	//Cone pose recieve thread
	private  void ReceiveConeData()
	{
		
		//Create UDP client on port 8052
		coneClient = new UdpClient(conePort);
		
		while (true)
		{
			
			try
			{
				//Set up UDP client to recieve packets
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = coneClient.Receive(ref anyIP);


				//Get amount of blue and yellow cones specified in first 2 vars
				double blueCount = BitConverter.ToDouble(data, 0);
				double yellowCount =  BitConverter.ToDouble(data, 8);
				
				// Store cone counts in UDPData
				UDPData.blueCount = (int) blueCount;
				UDPData.yellowCount = (int) yellowCount;
				Debug.Log(blueCount);
				Debug.Log(yellowCount);
				
				//Create blue cone arrays of correct size
				UDPData.blueX = new float[(int)blueCount];
				UDPData.blueY = new float[(int)blueCount];
				UDPData.blueZ = new float[(int)blueCount];
				
				//Create yellow cone arrays of correct size
				UDPData.yellowX = new float[(int)yellowCount];
				UDPData.yellowY = new float[(int)yellowCount];
				UDPData.yellowZ = new float[(int)yellowCount];

				
				//Loop through packet, store blue cone x,y,z then yellow cone x,y,z
				//Starts at first cone pose value in packet
				for (int ii = 2; ii < ((blueCount + yellowCount));  ii = ii+3)
				{
					//Start storing yellow cones once past blue cones
					if (ii > blueCount)
					{
						
						//Store yellow cone poses in array offset by two to start at 0 in array
						//Not using ii++ to increment to to an unknown issue
						UDPData.yellowX[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, ii *8));
						UDPData.yellowZ[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, (ii+1) *8));
						UDPData.yellowY[(ii-2)-(int)blueCount] = ((float) BitConverter.ToDouble(data, (ii+2)* 8));
						print("Ran");
						
			
					}
					else
					{
						//Store blue cone poses in array offset by two to start at 0 in array
						UDPData.blueX[ii-2] = ((float) BitConverter.ToDouble(data, ii *8));
						UDPData.blueZ[ii-2] = ((float) BitConverter.ToDouble(data, (ii+1) *8));
						UDPData.blueY[ii-2] = ((float) BitConverter.ToDouble(data, (ii+2)* 8));
					}
					
				}
				
				//Set readyToRun flag once has run and stored cone vlaues
				UDPData.readyToRun = true;
				
			}
			//Handle errors
			catch (Exception err)
			{
				print(err.ToString());
			}
			
			// Stop thread, only needs to run once
			coneRecieveThread.Abort();

		}
	}
	
	//Shut down threads on stopping script
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
