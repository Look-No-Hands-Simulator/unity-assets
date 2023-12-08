using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
public class GpsSimulation : MonoBehaviour
{
    GameObject gps_sensor_link;
    int utm_zone;
    float lon_origin;
    float lat_origin;

    // Radian version
    double lat_origin_rad;
    double lon_origin_rad;


    string cs2cs_path;
    string cs2cs_args; 

    const double R = 6371000; // Earth radius in meters


    public GpsSimulation(GameObject gps_sensor_link_param, int utm_zone_param, float lon_origin_param, float lat_origin_param) {
        gps_sensor_link = gps_sensor_link_param;
        utm_zone = utm_zone_param;
        lon_origin = lon_origin_param;
        lat_origin = lat_origin_param;
        // Assuming cs2cs is in the system PATH
        cs2cs_path = "cs2cs";
        // Replace with the appropriate projection and origin parameters
        cs2cs_args = $"+proj=utm +datum=WGS84 +lat_0={lat_origin} +lon_0={lon_origin} +units=m +to +proj=latlong +zone={utm_zone} +datum=WGS84 -f %.12f";


        // lattitude & longitude convert to radians
        lat_origin_rad = lat_origin * (Math.PI / 180.0);
        lon_origin_rad = lon_origin * (Math.PI / 180.0);

    }

    public void get_navsatfix_msg() {

        // calculate new lat & long in radians around earthR
        // fraction of the Earth's circumference * radius conversion
        double lat_position_rad = lat_origin_rad +  (gps_sensor_link.transform.position.z / R) ; // * (Math.PI / 180.0);
        double lon_position_rad = lon_origin_rad +  (gps_sensor_link.transform.position.x / (R * Math.Cos(lat_origin_rad))) ; // * (Math.PI / 180.0);


        // convert back to degrees
        double lat = lat_position_rad * (180.0 / Math.PI);
        double lon = lon_position_rad * (180.0 / Math.PI);

        UnityEngine.Debug.Log("Transformed Coordinates, latlon: " + "lat: " + lat + "lon: " + lon);


        // // cs2cs +proj=utm +zone=30 +datum=WGS84 +lat_0=52.073273 +lon_0=-1.014818 +units=m +to +proj=latlong +datum=WGS84

        // //assume x axis (pos) points eastward
        // //       z axis (pos) points northwards

        // // Create a string containing the input coordinates
        // string inputCoordinates = $"{gps_sensor_link.transform.position.x} {gps_sensor_link.transform.position.z}";  //input coords come from unity

        // // Print things and return strings
        // //////////////////////////////////////////////////
        // /// return void
        // /// convert unity coords into gps global coords relative to utm zone
        // /// public properties of utm zone and origin lat and long
        // /// 
        // // Start the cs2cs process
        // Process process = new Process();
        // process.StartInfo.FileName = cs2cs_path;
        // process.StartInfo.Arguments = cs2cs_args;
        // process.StartInfo.RedirectStandardInput = true;
        // process.StartInfo.RedirectStandardOutput = true;
        // process.StartInfo.UseShellExecute = false;
        // process.StartInfo.CreateNoWindow = true;

        // // Start the process
        // process.Start();

        // // Write the input coordinates to the cs2cs process
        // StreamWriter writer = process.StandardInput;
        // writer.WriteLine(inputCoordinates);
        // writer.Close();

        // // Read the output from the cs2cs process
        // // 3 floats string format (or double)
        // string output = process.StandardOutput.ReadToEnd();

        // // Close the cs2cs process
        // process.WaitForExit();
        // process.Close();

        // // Process the output as needed (e.g., parse UTM coordinates)
        // UnityEngine.Debug.Log("Transformed Coordinates: " + output);


        
    }
}