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
public class VCU2AIWheelspeedsPublisher : MonoBehaviour
{

    public GameObject fl_wheel;
    public GameObject fr_wheel;
    public GameObject bl_wheel;
    public GameObject br_wheel;
    public string wheelspeeds_topic = "/VCU2AIWheelspeeds";
    public string wheelcounts_topic = "/VCU2AIWheelcounts";
    public bool noise_activation;
    public int teeth_count;
    // TODO: Define how much noise there is
    //public float noise; 

    ROSConnection ros;
    WheelspeedsSimulation wheelspeeds_simulation;
    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<VCU2AIWheelspeedsMsg>(wheelspeeds_topic);
        ros.RegisterPublisher<VCU2AIWheelcountsMsg>(wheelcounts_topic);

        wheelspeeds_simulation = new WheelspeedsSimulation(fl_wheel, fr_wheel, bl_wheel, br_wheel, noise_activation);

    }

    void Update() {
        VCU2AIWheelspeedsMsg wheelspeeds_msg = wheelspeeds_simulation.get_vcu2aiwheelspeeds_msg();
        VCU2AIWheelcountsMsg wheelcounts_msg = wheelspeeds_simulation.get_vcu2aiwheelcounts_msg();

        ros.Publish(wheelspeeds_topic, wheelspeeds_msg);
        ros.Publish(wheelcounts_topic, wheelcounts_msg);
    }
}