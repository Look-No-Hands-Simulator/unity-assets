using UnityEngine;
// Custom namespace msgs
using RosMessageTypes.AdsDv;

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

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

    public enum WhichCamera {
        Left,
        Right
    }

    public StereoCameraSimulation(Camera left_camera_param, Camera right_camera_param) {
        left_camera = left_camera_param;
        right_camera = right_camera_param;
        
    }

    public ImageMsg get_image_msg(WhichCamera camera_choice) {
        Camera chosen_camera;
        if (camera_choice == WhichCamera.Left) {
            chosen_camera = left_camera;
        } else {
            chosen_camera = right_camera;
        }

        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        RenderTexture texture = chosen_camera.activeTexture;
        Texture2D captured_texture = new Texture2D(chosen_camera.width, chosen_camera.height);
        RenderTexture.active = texture;
        chosen_camera.Render();
        captured_texture.readPixels(new Rect(0,0,texture.width,texture.height),0,0);
        captured_texture.Apply();
        RenderTexture.Active = null;

        // ALl colours and pixels (rgb) in image texture
        Color[] pixels = captured_texture.GetPixels();
        byte[] image_data = new byte[pixels.Length*3];

        // Iterate through pixels and record each 3 bytes
        // create a byte for each r,g,b in each pixel
        for (i = 0; i < pixels.Length; i++) {
            byte[i*3] = pixels[i].b * 255;
            byte[i*3+1] = pixels[i].g * 255;
            byte[i*3+2] = pixels[i].r * 255;

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
            height = chosen_camera.pixelHeight,
            width = chosen_camera.pixelWidth,
            encoding = "bgr8",
            is_bigendian = 0,
            // Check this later, width of image in bytes
            step = (uint)((chosen_camera.pixelWidth)*3),
            data = image_data

        };

    }
}