using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

// Custom namespace msgs
using RosMessageTypes.AdsDv;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;

public class StereoCameraSimulation : MonoBehaviour
{
    Camera left_camera;
    Camera right_camera;

    int resolution;

    // public enum WhichCamera {
    //     Left,
    //     Right
    // }

    public StereoCameraSimulation(Camera left_camera_param, Camera right_camera_param) {
        left_camera = left_camera_param;
        right_camera = right_camera_param;
        
    }

    public ImageMsg get_image_msg(Camera chosen_camera) {
        // Camera chosen_camera;
        // if (camera_choice == left_camera) {
        //     chosen_camera = left_camera;
        // } else {
        //     chosen_camera = right_camera;
        // }

        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        RenderTexture current_texture = RenderTexture.active;
        Texture2D captured_texture = new Texture2D(chosen_camera.targetTexture.width, chosen_camera.targetTexture.height);
        RenderTexture.active = chosen_camera.targetTexture;
        chosen_camera.Render();
        captured_texture.ReadPixels(new Rect(0,0,chosen_camera.targetTexture.width, chosen_camera.targetTexture.height),0,0);
        captured_texture.Apply();
        RenderTexture.active = current_texture;

        // All colours and pixels (rgb) in image texture
        // Create array of pixels (Color objects, rgb, 0-1 values) from captured texture
        Color[] pixels = captured_texture.GetPixels();
        byte[] image_data = new byte[pixels.Length*3];
        int img_size = pixels.Length*3;

        // Iterate through pixels and record each 3 bytes
        // create a byte for each r,g,b in each pixel
        for (int i = 0; i < pixels.Length; i++) {
            // bytes? ########### TODO
            // By * 0-1 by 255 we get a colour value between 0-255
            // Work from end of array to prevent upside down image
            /// TODO: Flip this left to right
            image_data[img_size - i*3 - 3] = (byte)(pixels[i].b * 255);
            image_data[img_size - i*3 - 2] = (byte)(pixels[i].g * 255);
            image_data[img_size - i*3 - 1] = (byte)(pixels[i].r * 255);

        }

        return new ImageMsg {
            header = new HeaderMsg
            {
                frame_id = chosen_camera.name,
                stamp = new TimeMsg
                {
                    sec = msg_timestamp.Seconds,
                    nanosec = msg_timestamp.NanoSeconds,
                }
            },
            height = (uint)chosen_camera.pixelHeight,
            width = (uint)chosen_camera.pixelWidth,
            encoding = "bgr8",
            is_bigendian = 0,
            // Check this later, width of image in bytes
            // step, how many bits to look at before starting next row.
            step = (uint)((chosen_camera.pixelWidth)*3),
            data = image_data

        };

    }
}