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

public class StereoCameraSimulation
{
    Camera left_camera;
    Camera right_camera;

    int resolution;

    public StereoCameraSimulation(Camera left_camera_param, Camera right_camera_param) {
        left_camera = left_camera_param;
        right_camera = right_camera_param;
        
    }

    public ImageMsg get_image_msg(Camera chosen_camera) {

        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        int width = chosen_camera.targetTexture.width;
        int height = chosen_camera.targetTexture.height;


        RenderTexture current_texture = RenderTexture.active;
        Texture2D captured_texture = new Texture2D(width, height);
        RenderTexture.active = chosen_camera.targetTexture;
        chosen_camera.Render();
        captured_texture.ReadPixels(new Rect(0,0,width, height),0,0);
        captured_texture.Apply();
        RenderTexture.active = current_texture;

        // All colours and pixels (rgb) in image texture
        // Create array of pixels (Color objects, rgb, 0-1 values) from captured texture


        // Already in 8 bit encoding
        Color32[] pixels = captured_texture.GetPixels32();



        //Color[] flipped_pixels = new Color[pixels.Length];

        // Populate flipped pixels to flip the pixel matrix from bottom left to top left order
        // total_px - n * rows + y coord
        // for (int i = 0; i < x * y; i++) { flipped_pixels[i] = pixels[x*y-(x*(1+1//x))+(i%x)] }
        // for (int i = 0; i < flipped_pixels.Length; i++) {
        //     flipped_pixels[flipped_pixels.Length - (chosen_camera.targetTexture.width * (1 + i / chosen_camera.targetTexture.width)) + (i % chosen_camera.targetTexture.width)] =
        //     pixels[i];
        // }



        // bgra
        byte[] image_data = new byte[pixels.Length*4];
        // first row pixels
        int img_size = pixels.Length*4;




        // Iterate through pixels and record each 3 bytes
        // create a byte for each r,g,b in each pixel


        for (int i = 0; i < pixels.Length; i++) {


            // bytes? ########### TODO
            // By * 0-1 by 255 we get a colour value between 0-255
            // Work from end of array to prevent upside down image


            Color32 pixel = pixels[pixels.Length - (width * (1 + i / width)) + (i % width)];
            image_data[i*4] = (byte)(pixel.b);
            image_data[i*4 + 1] = (byte)(pixel.g);
            image_data[i*4 + 2] = (byte)(pixel.r);
            image_data[i*4 + 3] = (byte)(pixel.a);

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
            height = (uint)height,
            width = (uint)width,
            encoding = "bgra8", // The encoding of ZED camera is bgra8, YOLO itself will have to remove the 8 alpha bits and make it 24 bits bgr, can use OpenCV
            is_bigendian = 0,
            // Check this later, width of image in bytes
            // step, how many bits to look at before starting next row.
            step = (uint)((width)*4),
            data = image_data

        };

    }
}