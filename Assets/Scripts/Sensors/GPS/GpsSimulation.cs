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

public class GpsSimulation
{
    GameObject gps_sensor_link;
    float lon_origin;
    float lat_origin;
    float alt_origin; 

    // Radian version
    double lat_origin_rad;
    double lon_origin_rad;

    // Earth radius in meters
    const double R = 6371000; 
    // Degrees around the world
    const double GPS_STANDARD_DEVIATION = 0.000003;
    // Meters
    const double ALTITUDE_STANDARD_DEVIATION = 0.322;

    bool noise_activation;

    GaussianGenerator gaussian_generator;

    double[] covariance_matrix;


    public GpsSimulation(GameObject gps_sensor_link_param, float lon_origin_param, float lat_origin_param, float alt_origin_param, 
        bool noise_activation_param) {

        gps_sensor_link = gps_sensor_link_param;
        lon_origin = lon_origin_param;
        lat_origin = lat_origin_param;
        alt_origin = alt_origin_param;

        // lattitude & longitude degrees convert to radians
        lat_origin_rad = lat_origin * (Math.PI / 180.0);
        lon_origin_rad = lon_origin * (Math.PI / 180.0);

        noise_activation = noise_activation_param;

        // Numbers between 0 and 0.03, 1% standard deviation
        // Error at equator would be 0 with a normal scale so it is adjusted
        // accuracy: https://www.gps.gov/systems/gps/performance/accuracy/#how-accurate
        gaussian_generator = new GaussianGenerator(0, GPS_STANDARD_DEVIATION);

        // Covariance 
        covariance_matrix = new double[]{Math.Pow(GPS_STANDARD_DEVIATION,2),0.0,0.0,0.0,Math.Pow(GPS_STANDARD_DEVIATION,2),0.0,0.0,0.0,
        Math.Pow(ALTITUDE_STANDARD_DEVIATION,2)};

    }

    public NavSatFixMsg get_navsatfix_msg() {

        // calculate new lat & long in radians around earthR
        // fraction of the Earth's circumference * radius conversion
        double lat_position_rad = lat_origin_rad +  (gps_sensor_link.transform.position.z / R) ; // * (Math.PI / 180.0);
        double lon_position_rad = lon_origin_rad +  (gps_sensor_link.transform.position.x / (R * Math.Cos(lat_origin_rad))) ; // * (Math.PI / 180.0);


        // convert angle in radians back to degrees around the earth because lat and lon measured in this way
        double lat = lat_position_rad * (180.0 / Math.PI);
        double lon = lon_position_rad * (180.0 / Math.PI);
        // y is meters above or below ground and the origin is the GPS location in meters above sea level
        double alt = alt_origin + gps_sensor_link.transform.position.y;

        // UnityEngine.Debug.Log("Transformed Coordinates, latlon: " + "lat: " + lat + "lon: " + lon);
        
        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);


        if (noise_activation == true) {
            // degrees around the world
            lat = gaussian_generator.add_noise(lat);
            // degrees around the world
            lon = gaussian_generator.add_noise(lon);
            // meters, gives a standard deviation of 32cm 
            alt = alt + gaussian_generator.next(0, ALTITUDE_STANDARD_DEVIATION);
        }


        // Prepare msg
        var msg = new NavSatFixMsg
        {
            header = new HeaderMsg
            {
                frame_id = gps_sensor_link.name,
                stamp = new TimeMsg
                {
                    sec = msg_timestamp.Seconds,
                    nanosec = msg_timestamp.NanoSeconds,
                }
            },
            status = new NavSatStatusMsg
            {
                // Unable to fix position = -1, unaugmented fix = 0, satellite-based augmentation = 1, ground-based augmentation = 2
                status = 0,
                // GPS = 1, GLONASS = 2, COMPASS = 4, GALILEO = 8   (Bitwise & to check which service we using, check which bit is filled in)
                service = 1
                
            },
            latitude = lat,
            longitude = lon,
            altitude = alt,
            // Diagonals are 0,4,8   or use 1.0e-5, accurate to nearest cm 0.01
            position_covariance = covariance_matrix,
            // 0 = unknown, 1 = approximated, 2 = diagonal known, 3 = known
            position_covariance_type = 1
        };

        return msg;

        
    }
}