using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

// Custom namespace msgs
using RosMessageTypes.AdsDv;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using TMPro;

using System.IO;

/********************************************
 * Publishes VCU2AIStatus msgs and subscribes
 * to AI2VCUStatus msgs
 * 
 ********************************************/

public class VCUAIStatusLink : MonoBehaviour
{
    public Button handshakingbutton; 

    public bool handshakingOnOff = false;

    public ADS_DV_State adsdv_state;

    public string vcu2ai_status_topic = "/VCU2AIStatus";
    public string ai2vcuStatusTopic = "/AI2VCUStatus";

    ROSConnection ros;

    public float timeout_interval = 0.2f;
    private float time_elapsed = 0.0f;

    private StreamWriter logfile;

    public bool enableLoggingFile = false;

    void Start() {

        string filePath = Application.dataPath;

        //Debug.Log("Log filepath: " + filePath);

        string filename = "log.txt";

        if (enableLoggingFile == true) {

            logfile = new StreamWriter(Path.Combine(filePath, filename));
        }


        

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<VCU2AIStatusMsg>(vcu2ai_status_topic);
        ros.Subscribe<AI2VCUStatusMsg>(ai2vcuStatusTopic, AI2VCUSubscriberManager);


        // Send first handshake from VCU
        VCU2AIStatusMsg vcu2ai_status_msg = adsdv_state.get_vcu2aiStatus_msg();

        ros.Publish(vcu2ai_status_topic, vcu2ai_status_msg);

        time_elapsed = 0;

    }

    public void AI2VCUSubscriberManager(AI2VCUStatusMsg statusMsg) {

        // Debug.Log("Recieved AI2VCUStatus msg: ");
        // Debug.Log(statusMsg.ToString());

        time_elapsed = 0;

        if (enableLoggingFile == true) {

            LogToFile(statusMsg);

        }

        // Get values from the msg and assign them into the ADS_DV_State 
        adsdv_state.manage_ai2vcuStatus_msg(statusMsg);

        // Send response from VCU
        VCU2AIStatusMsg vcu2ai_status_msg = adsdv_state.get_vcu2aiStatus_msg();
        ros.Publish(vcu2ai_status_topic, vcu2ai_status_msg);

        time_elapsed = 0;


    }

    void LogToFile(AI2VCUStatusMsg statusMsg) {

        // Log AI2VCUStatusmsg
        logfile.WriteLine(statusMsg.ToString());
        

    }

    public void HandshakingButton() {

        if (handshakingOnOff == false) {

            handshakingOnOff = true;

            handshakingbutton.GetComponent<Image>().color = Color.green;
        } else {

            handshakingOnOff = false;
            handshakingbutton.GetComponent<Image>().color = Color.red;
        }
    }

    void Update() {

        if (handshakingOnOff == true) {

            time_elapsed += Time.deltaTime;
            if (time_elapsed > timeout_interval) {


                if (adsdv_state.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING) {

                    adsdv_state.Ai_comms_lost = true;

                    Debug.Log("Timeout: " + time_elapsed);

                }
            
            }   

        }  

    }






}