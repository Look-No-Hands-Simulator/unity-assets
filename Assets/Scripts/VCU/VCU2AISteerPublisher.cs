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

public class VCU2AISteerPublisher : MonoBehaviour
{

    public ADS_DV_State adsdv_state;

    public string vcu2ai_steer_topic = "/VCU2AISteer";

    ROSConnection ros;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<VCU2AISteerMsg>(vcu2ai_steer_topic);

    }

    void Update() {

        VCU2AISteerMsg vcu2ai_steer_msg = adsdv_state.get_vcu2aisteer_msg();

        ros.Publish(vcu2ai_steer_topic, vcu2ai_steer_msg);
    }
}