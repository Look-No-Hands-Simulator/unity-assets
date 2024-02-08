using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.IO;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;


public class DepthMappingPublisher : MonoBehaviour
{
    public Camera camera;
    public string depth_camera_topic = "/unity/stereo_camera/depth/image";

    public float min_publishing_time = 0.5f; 

    private float time_elapsed = 0.0f;

    public float noise; 

    ImageSynthesis imageSynthesis;

    ROSConnection ros;
    StereoCameraSimulation stereo_camera_simulation;
    int counter = 1;

    void Start() {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(depth_camera_topic);

        this.imageSynthesis = (ImageSynthesis)camera.GetComponent("ImageSynthesis");


    }



    void Update() {
        time_elapsed += Time.deltaTime;
        if (time_elapsed > min_publishing_time) {
            ImageMsg depth_img_msg = this.PrepareImgMsg();

            ros.Publish(depth_camera_topic, depth_img_msg);

            time_elapsed = 0.0f;
        }
        


    }

    public ImageMsg PrepareImgMsg() {
        // Save image as png
        byte[] depth_data = imageSynthesis.GetDepthImage();
        if (this.counter < 4 && depth_data != null) {
            Debug.Log("Size of data: " + depth_data.Length);
            File.WriteAllBytes("/home/louise/dissertation_obr_ws/unityros-ws/src/depthimage" + this.counter + ".png", depth_data);
            counter++;
        }

        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        int width = this.camera.targetTexture.width;
        int height = this.camera.targetTexture.height;

        return new ImageMsg {
            header = new HeaderMsg
            {
                frame_id = this.camera.name,
                stamp = new TimeMsg
                {
                    sec = msg_timestamp.Seconds,
                    nanosec = msg_timestamp.NanoSeconds,
                }
            },
            height = (uint)height,
            width = (uint)width,
            encoding = "32FC1",
            is_bigendian = 0,
            // Check this later, width of image in bytes
            // step, how many bits to look at before starting next row.
            // 32 bit encoding so * 4
            // 4 bytes for each pixel (byte = 8 bits)
            step = (uint)((width)*4),
            data = imageSynthesis.GetDepthImage()

        };
    }
}