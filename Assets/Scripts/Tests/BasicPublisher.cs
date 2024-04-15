using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;

public class BasicPublisher : MonoBehaviour
{
	
    ROSConnection ros;
    ImuSimulation imu_simulation;
    public string imu_topic = "/imu_tester";
    
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImuMsg>(imu_topic);
    }

    // Update is called once per frame
    void Update()
    {
    	// Prepare msg
        var msg = new ImuMsg
        {
            header = new HeaderMsg
            {
                frame_id = "test",
                stamp = new TimeMsg
                {
                    sec = 0,
                    nanosec = 0,
                }
            },
            orientation = new QuaternionMsg // radians per second
            {
                x = 0,
                y = 0,
                z = 0,
                w = 0
            },
            orientation_covariance = new double[9]{0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0}, // 3x3 matrix
            angular_velocity = new Vector3Msg
            {
                x = 0,
                y = 0,
                z = 0
            } ,
            angular_velocity_covariance = new double[9]{0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0},
            linear_acceleration = new Vector3Msg
            {
                x = 0,
                y = 0,
                z = 0
            },
            linear_acceleration_covariance = new double[9]{0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0}
        };
        
        ros.Publish(imu_topic, msg);

    }
    
    
        
    
}
