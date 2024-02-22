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

    public string vcu2ai_status_topic = "/VCU2AIStatus_LNH";

    ROSConnection ros;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<VCU2AIWheelspeedsMsg>(vcu2ai_status_topic);

        //wheelspeeds_simulation = new WheelspeedsSimulation(fl_wheel, fr_wheel, bl_wheel, br_wheel, noise_activation);

    }

    void Update() {
        //VCU2AIWheelspeedsMsg wheelspeeds_msg = wheelspeeds_simulation.get_vcu2aiwheelspeeds_msg();

        VCU2AIStatusMsg vcu2ai_status_msg = adsdv_state.get_vcu2aiStatus_msg();

        ros.Publish(wheelspeeds_topic, wheelspeeds_msg);
    }
}