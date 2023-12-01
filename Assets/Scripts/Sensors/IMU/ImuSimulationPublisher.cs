using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
public class ImuSimulationPublisher : MonoBehaviour
{

    public GameObject imu_sensor_link;
    public string imu_topic = "/imu_unity";

    public int axis = 3;

    public float noise; 

    ROSConnection ros;
    ImuSimulation imu_simulation;
    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImuMsg>(imu_topic);

        imu_simulation = new ImuSimulation(imu_sensor_link);

    }

    void Update() {
        ImuMsg imu_msg = imu_simulation.get_imu_msg();

        ros.Publish(imu_topic, imu_msg);
    }
}