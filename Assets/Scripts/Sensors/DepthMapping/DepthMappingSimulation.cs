// using UnityEngine;
// using UnityEngine.Serialization;
// using System.Collections.Generic;
// using System;
// using System.IO;
// using System.Linq;

// using RosMessageTypes.Geometry;
// using RosMessageTypes.Sensor;
// using RosMessageTypes.Std;
// using RosMessageTypes.BuiltinInterfaces;

// using Unity.Robotics.Core;
// using Unity.Robotics.ROSTCPConnector;
// public class DepthMappingSimulation : MonoBehaviour
// {
//     Camera camera; 
//     int max_range;
//     RenderTexture depth_texture; 
//     bool printed;

//     float[] depth_values;

//     public DepthMappingSimulation(Camera camera_param, int max_range_param) {
//         camera = camera_param;
//         camera.depthTextureMode = DepthTextureMode.Depth;
//         max_range = max_range_param;
//         // Create depth texture, 24 bits for depth
//         depth_texture = new RenderTexture(camera.pixelWidth,camera.pixelHeight,24,RenderTextureFormat.Depth);
//         //depth_texture.enableRandomWrite = true;
//         depth_texture.Create();
//         //camera.targetTexture = depth_texture;
//         printed = false;
//     }

//     public ImageMsg get_img_msg() {
//         // Get Unix time, how long since Jan 1st 1970?
//         TimeStamp msg_timestamp = new TimeStamp(Clock.time);

//         int width = camera.targetTexture.width;
//         int height = camera.targetTexture.height;


//         RenderTexture current_texture = RenderTexture.active;
//         Texture2D captured_texture = new Texture2D(width, height,TextureFormat.RFloat, false);
//         RenderTexture.active = camera.targetTexture;
//         Graphics.Blit(depth_texture,depth_values,camera.targetTexture);
//         captured_texture.ReadPixels(new Rect(0,0,width, height),0,0);
//         captured_texture.Apply();
//         RenderTexture.active = current_texture;

//         //Color[] pixels = captured_texture.GetPixels();
//         float[] depth_values = captured_texture.GetRawTextureData<float>().ToArray();

//         if (!printed) {
//             Debug.Log("Maxdepth: " + depth_values.Max());
//             // Put output into file to check for errors
//             StreamWriter writer = new StreamWriter("depthvalues.txt");
//             for (int j = 0; j < depth_values.Length; j++) {
//                 // End of row
//                 if (j > 0 && j % width == 0) {
//                     writer.WriteLine(" ");
//                 }
//                 writer.Write(depth_values[j] + " ");
//             }
//             writer.Close();
//         }

//         //float[] image_data = new float[depth_values.Length];
//         byte[] image_bytes = new byte[sizeof(float)*depth_values.Length];

//         // for (int i = 0; i < depth_values.Length; i++) {
//         //     // Make sure pixel is in correct row and space for the i value
//         //     image_data[i] = depth_values[depth_values.Length - (width * (1 + i / width)) + (i % width)];
//         //     // Only red value holds depth
//         //     //image_data[i] = (byte)(pixel * 255.0f);
//         // }

//         // if (!printed) {
//         //     // Put output into file to check for errors
//         //     StreamWriter writer = new StreamWriter("imagedata.txt");
//         //     for (int j = 0; j < image_data.Length; j++) {
//         //         // End of row
//         //         if (j > 0 && j % width == 0) {
//         //             writer.WriteLine(" ");
//         //         }
//         //         writer.Write(image_data[j] + " ");
//         //     }
//         //     writer.Close();
//         // }

//         // Copy from image data floats into bytes array
//         // Copies image_data into image_bytes byte by byte
//         //System.Buffer.BlockCopy(depth_values, 0, image_bytes, 0, image_bytes.Length);


//         printed = true;

//         return new ImageMsg {
//             header = new HeaderMsg
//             {
//                 frame_id = camera.name,
//                 stamp = new TimeMsg
//                 {
//                     sec = msg_timestamp.Seconds,
//                     nanosec = msg_timestamp.NanoSeconds,
//                 }
//             },
//             height = (uint)height,
//             width = (uint)width,
//             encoding = "32FC1",
//             is_bigendian = 0,
//             // Check this later, width of image in bytes
//             // step, how many bits to look at before starting next row.
//             step = (uint)width,
//             data = image_bytes

//         };

//     }
    
// }