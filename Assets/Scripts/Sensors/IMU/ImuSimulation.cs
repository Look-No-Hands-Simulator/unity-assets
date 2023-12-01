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

    public ImuSimulation(GameObject imu_sensor_link_object) {
        imu_sensor_link = imu_sensor_link_object;
        imu_physical = imu_sensor_link.GetComponent<Rigidbody>();
        last_msg_timestamp = DateTime.Now;
        last_velocity = new Vector3
            {
                x = imu_physical.velocity.x,
                y = imu_physical.velocity.y,
                z = imu_physical.velocity.z
            }; 
        

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
                x = imu_physical.rotation.x,
                y = imu_physical.rotation.y,
                z = imu_physical.rotation.z,
                w = imu_physical.rotation.w
            },
            angular_velocity = new Vector3Msg 
            {
                x = imu_physical.angularVelocity.x,
                y = imu_physical.angularVelocity.y,
                z = imu_physical.angularVelocity.z
            },
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