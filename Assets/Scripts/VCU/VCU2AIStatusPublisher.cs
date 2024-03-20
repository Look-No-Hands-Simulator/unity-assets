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

public class VCU2AIStatusPublisher : MonoBehaviour
{

    public ADS_DV_State adsdv_state;

    public string vcu2ai_status_topic = "/VCU2AIStatus";

    ROSConnection ros;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<VCU2AIStatusMsg>(vcu2ai_status_topic);

    }

    void Update() {

        VCU2AIStatusMsg vcu2ai_status_msg = adsdv_state.get_vcu2aiStatus_msg();

        ros.Publish(vcu2ai_status_topic, vcu2ai_status_msg);
    }
}