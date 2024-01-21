// using UnityEngine;
// using UnityEngine.Serialization;
// using System.Collections.Generic;
// using System;

// using RosMessageTypes.Geometry;
// using RosMessageTypes.Sensor;
// using RosMessageTypes.Std;
// using RosMessageTypes.BuiltinInterfaces;

// using Unity.Robotics.Core;
// using Unity.Robotics.ROSTCPConnector;
// public class ImageMsgPublisherDepth : MonoBehaviour
// {
//     public GameObject camera_link;
//     public Camera camera;
//     public string depthcamera_topic = "/unity/stereo_camera/depth/image";

//     public float min_publishing_time = 0.5f; 

//     private float time_elapsed = 0.0f;

//     public float noise; 

//     ROSConnection ros;
//     DepthMappingSimulation depth_mapping_simulation;
//     void Start() {
//         ros = ROSConnection.GetOrCreateInstance();
//         ros.RegisterPublisher<ImageMsg>(depthcamera_topic);
//         depth_mapping_simulation = new DepthMappingSimulation(camera, 20);

//     }

//     void Update() {
//         time_elapsed += Time.deltaTime;
//         if (time_elapsed > min_publishing_time) {
//             ImageMsg depth_imagemsg = depth_mapping_simulation.get_img_msg();

//             ros.Publish(depthcamera_topic, depth_imagemsg);

//             time_elapsed = 0.0f;
//         }
        


//     }
// }