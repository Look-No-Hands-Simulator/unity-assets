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

// This is not used

public class AI2VCUStatusSubscriber: MonoBehaviour
{

    public ADS_DV_State adsdvState;

    public string ai2vcuStatusTopic = "/AI2VCUStatus";


    void Start() {


        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUStatusMsg>(ai2vcuStatusTopic, AI2VCUSubscriberManager);


    }

    void Update() {

    }

    public void AI2VCUSubscriberManager(AI2VCUStatusMsg statusMsg) {

        // Debug.Log("Recieved AI2VCUStatus msg: ");
        // Debug.Log(statusMsg.ToString());

        // Get values from the msg and assign them into the ADS_DV_State 
        adsdvState.manage_ai2vcuStatus_msg(statusMsg);


    }
}