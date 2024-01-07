using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
public class ImuSimulation : MonoBehaviour
{
    GameObject imu_sensor_link;

    Rigidbody imu_physical;

    DateTime last_msg_timestamp;

    Vector3 last_velocity;

    GaussianGenerator gaussian_generator; 

    bool noise_activation;

    public ImuSimulation(GameObject imu_sensor_link_object, bool noise_activation_param) {
        imu_sensor_link = imu_sensor_link_object;
        noise_activation = noise_activation_param;
        imu_physical = imu_sensor_link.GetComponent<Rigidbody>();
        last_msg_timestamp = DateTime.Now;
        last_velocity = new Vector3
            {
                x = imu_physical.velocity.x,
                y = imu_physical.velocity.y,
                z = imu_physical.velocity.z
            }; 
        // Numbers between 0 and 0.03, 1% standard deviation
        gaussian_generator = new GaussianGenerator(0, 0.01);
        

    }

    public ImuMsg get_imu_msg() {

        DateTime timestamp_now = DateTime.Now;

        Vector3 velocity_now = new Vector3 
        {
            x = imu_physical.velocity.x,
            y = imu_physical.velocity.y,
            z = imu_physical.velocity.z
        };
        // Change in meters per second per second (change of a change in speed)
        Vector3 acceleration = (velocity_now - last_velocity) / (float)(timestamp_now - last_msg_timestamp).TotalSeconds;

        // Update time and velocity changes for next time
        last_msg_timestamp = timestamp_now;
        last_velocity = velocity_now;
        
        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        Vector3 angular_velocity = imu_physical.angularVelocity;
        Quaternion orientation = imu_physical.rotation;

        if (noise_activation == true) {
            angular_velocity = gaussian_generator.add_noise(angular_velocity);
            orientation = gaussian_generator.add_noise(orientation);
            acceleration = gaussian_generator.add_noise(acceleration);
        }

        // Prepare msg
        var msg = new ImuMsg
        {
            header = new HeaderMsg
            {
                frame_id = imu_sensor_link.name,
                stamp = new TimeMsg
                {
                    sec = msg_timestamp.Seconds,
                    nanosec = msg_timestamp.NanoSeconds,
                }
            },
            orientation = new QuaternionMsg
            {
                x = orientation.x,
                y = orientation.y,
                z = orientation.z,
                w = orientation.w
            },
            angular_velocity = new Vector3Msg
            {
                x = angular_velocity.x,
                y = angular_velocity.y,
                z = angular_velocity.z
            } ,
            linear_acceleration = new Vector3Msg
            {
                x = acceleration.x,
                y = acceleration.y,
                z = acceleration.z
            }
        };

        return msg;
    }
}