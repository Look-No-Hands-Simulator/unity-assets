using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;


// Publishes ADS-DV msgs from key commands, the keys publish onto topics and then these topics are subscribed to and the vehicle is controlled through these values that
// are being sent to through the keys.
// We can enable or disable this... maybe... maybe disable the direct key control without the publishing

public class ImageMsgPublisher : MonoBehaviour
{
    public GameObject camera_link;
    public Camera left_camera;
    public Camera right_camera;
    public string rightcamera_topic = "/unity/stereo_camera/left/image";
    public string leftcamera_topic = "/unity/stereo_camera/right/image";

    public float min_publishing_time = 0.5f; 

    private float time_elapsed = 0.0f;

    public float noise; 

    ROSConnection ros;
    StereoCameraSimulation stereo_camera_simulation;
    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(rightcamera_topic);
        ros.RegisterPublisher<ImageMsg>(leftcamera_topic);

        // left_camera = camera_link.transform.Find("left_camera").GetComponent<Camera>();
        // right_camera = camera_link.transform.Find("right_camera").GetComponent<Camera>();

        // Debug.Log("Left camera? " + left_camera.depth);

        stereo_camera_simulation = new StereoCameraSimulation(left_camera, right_camera);

    }



    void Update() {
        time_elapsed += Time.deltaTime;
        if (time_elapsed > min_publishing_time) {
            ImageMsg right_imagemsg = stereo_camera_simulation.get_image_msg(right_camera);
            ImageMsg left_imagemsg = stereo_camera_simulation.get_image_msg(left_camera);

            ros.Publish(rightcamera_topic, right_imagemsg);
            ros.Publish(leftcamera_topic, left_imagemsg);

            time_elapsed = 0.0f;
        }
        


    }
}