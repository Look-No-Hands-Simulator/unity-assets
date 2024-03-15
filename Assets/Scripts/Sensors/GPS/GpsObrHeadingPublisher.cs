using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

// Custom msgs
using RosMessageTypes.Obr;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;


public class GpsObrHeadingPublisher: MonoBehaviour
{
    public GameObject gps_sensor_link;
    public string heading_topic = "/gps/heading";

    public float noise; 

    public bool noise_activation = false;


    ROSConnection ros;
    GpsObrHeadingSimulation gps_obr_heading_simulation;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ArdusimpleHeadingMsg>(heading_topic);

        gps_obr_heading_simulation = new GpsObrHeadingSimulation(gps_sensor_link, noise_activation);

    }

    void Update() {
        ArdusimpleHeadingMsg heading_msg = gps_obr_heading_simulation.get_heading_msg();

        ros.Publish(heading_topic, heading_msg);
    }
}