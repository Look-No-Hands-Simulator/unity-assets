using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using System;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Obr;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
public class CarPosPublisher : MonoBehaviour
{
    // This needs to be a PoseStamped in order to use RVIZ2 Pose marker

    public GameObject carObject;

    public string poseStampedTopic = "/car_pose_unity";

    public double longitude, latitude, altitude;

    ROSConnection ros;


    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(poseStampedTopic);

    }

    void Update() {
        
        // Get time now and convert to DateTimeOffset, use that object to get seconds and nanoseconds
        DateTime timestamp = DateTime.UtcNow;

        PoseStampedMsg poseStampedMsg = new PoseStampedMsg
        {
            pose = new PoseMsg{
                            position = new PointMsg(carObject.transform.position.x, carObject.transform.position.y, carObject.transform.position.z),
                            orientation = new QuaternionMsg(carObject.transform.rotation.x, carObject.transform.rotation.y, carObject.transform.rotation.z, carObject.transform.rotation.w)
            },
            header = new HeaderMsg{
                stamp = new TimeMsg{
                    sec = (int)((DateTimeOffset)timestamp).ToUnixTimeSeconds(),
                    nanosec = (uint)(timestamp.Millisecond*1000000)
                },
                frame_id = "map"
            }

        };

        ros.Publish(poseStampedTopic, poseStampedMsg);
    }
    
}