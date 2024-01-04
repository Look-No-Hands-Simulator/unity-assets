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
public class DepthMappingSimulation : MonoBehaviour
{
    Camera camera; 
    int max_range;
    RenderTexture depth_texture; 
    bool printed;

    public DepthMappingSimulation(Camera camera_param, int max_range_param) {
        camera = camera_param;
        max_range = max_range_param;
        // Create depth texture, 24 bits for depth
        depth_texture = new RenderTexture(camera.pixelWidth,camera.pixelHeight,24,RenderTextureFormat.Depth);
        //depth_texture.enableRandomWrite = true;
        depth_texture.Create();
        printed = false;
    }

    public ImageMsg get_img_msg() {
        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        int width = camera.targetTexture.width;
        int height = camera.targetTexture.height;


        RenderTexture current_texture = RenderTexture.active;
        Texture2D captured_texture = new Texture2D(width, height,TextureFormat.RFloat, false);
        RenderTexture.active = camera.targetTexture;
        Graphics.Blit(depth_texture,RenderTexture.active);
        captured_texture.ReadPixels(new Rect(0,0,width, height),0,0);
        captured_texture.Apply();
        RenderTexture.active = current_texture;

        //Color[] pixels = captured_texture.GetPixels();
        float[] depth_values = captured_texture.GetRawTextureData<float>().ToArray();


        byte[] image_data = new byte[depth_values.Length];

        for (int i = 0; i < depth_values.Length; i++) {
            // if (!printed) {
            //     Debug.Log("PIXELS:" + i + ";" + depth_values[i]);
            // }

            float pixel = depth_values[depth_values.Length - (width * (1 + i / width)) + (i % width)];
            // Only red value holds depth
            image_data[i] = (byte)(pixel * 255);
        }
        //printed = true;

        return new ImageMsg {
            header = new HeaderMsg
            {
                frame_id = camera.name,
                stamp = new TimeMsg
                {
                    sec = msg_timestamp.Seconds,
                    nanosec = msg_timestamp.NanoSeconds,
                }
            },
            height = (uint)height,
            width = (uint)width,
            encoding = "mono8",
            is_bigendian = 0,
            // Check this later, width of image in bytes
            // step, how many bits to look at before starting next row.
            step = (uint)width,
            data = image_data

        };

    }
    
}