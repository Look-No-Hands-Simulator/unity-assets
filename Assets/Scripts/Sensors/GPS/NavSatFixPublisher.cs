using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
public class NavSatFixPublisher : MonoBehaviour
{
    public GameObject gps_sensor_link;
    public string gps_topic = "/gps_unity";

    public float noise; 
    public float lon_origin;
    public float lat_origin; 
    public float alt_origin;
    public bool noise_activation;

    ROSConnection ros;
    GpsSimulation gps_simulation;
    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<NavSatFixMsg>(gps_topic);

        gps_simulation = new GpsSimulation(gps_sensor_link,lon_origin,lat_origin,alt_origin,noise_activation);

    }

    void Update() {
        NavSatFixMsg gps_msg = gps_simulation.get_navsatfix_msg();
        //gps_simulation.get_navsatfix_msg();

        ros.Publish(gps_topic, gps_msg);
    }
}