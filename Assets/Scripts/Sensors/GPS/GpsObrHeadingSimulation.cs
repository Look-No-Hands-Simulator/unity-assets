using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;

// Custom msgs
using RosMessageTypes.Obr;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;

public class GpsObrHeadingSimulation
{
    GameObject gps_sensor_link;


    bool noise_activation;

    GaussianGenerator gaussian_generator;

    double[] covariance_matrix;


    public GpsObrHeadingSimulation(GameObject gps_sensor_link_param, bool noise_activation_param) {

        gps_sensor_link = gps_sensor_link_param;

        noise_activation = noise_activation_param;

    }

    public ArdusimpleHeadingMsg get_heading_msg() {


        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        return new ArdusimpleHeadingMsg{

            header = new HeaderMsg
                {
                    frame_id = gps_sensor_link.name,
                    stamp = new TimeMsg
                    {
                        sec = msg_timestamp.Seconds,
                        nanosec = msg_timestamp.NanoSeconds,
                    }
                },

            relpos_n = gps_sensor_link.transform.position.z,
            relpos_e = gps_sensor_link.transform.position.x,
            relpos_d = gps_sensor_link.transform.position.y,
            relpos_length = CarConfig.VEHICLE_LENGTH, // m of car length, is this correct?
            relpos_heading = gps_sensor_link.transform.eulerAngles.y

        };

    }


}

   