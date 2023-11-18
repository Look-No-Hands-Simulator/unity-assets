using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

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

    ROSConnection ros;


    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(poseStampedTopic);

    }

    void Update() {

        PoseStampedMsg poseStampedMsg = new PoseStampedMsg
        {
            pose = new PoseMsg{
                            position = new PointMsg(carObject.transform.position.x, carObject.transform.position.y, carObject.transform.position.z),
                            orientation = new QuaternionMsg(carObject.transform.rotation.x, carObject.transform.rotation.y, carObject.transform.rotation.z, carObject.transform.rotation.w)
            }

        };

        ros.Publish(poseStampedTopic, poseStampedMsg);
    }
    
}