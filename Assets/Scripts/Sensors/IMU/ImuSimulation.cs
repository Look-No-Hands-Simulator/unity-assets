using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
public class ImuSimulation : MonoBehaviour
{
    GameObject imu_sensor_link;

    TimeStamp last_msg_timestamp;

    Vector3 last_velocity;

    ImuSimulation(GameObject imu_sensor_link) {
        imu_sensor_link = imu_sensor_link;
        last_msg_timestamp = new TimeStamp(Clock.time);
        last_velocity = new Vector3
            {
                x = imu_sensor_link.velocity.x,
                y = imu_sensor_link.velocity.y,
                z = imu_sensor_link.velocity.z
            }; 
        

    }

    public ImuMsg get_imu_msg() {

        var timestamp_now = new TimeStamp(Clock.time);

        Vector3 velocity_now = new Vector3 
        {
            x = imu_sensor_link.velocity.x,
            y = imu_sensor_link.velocity.y,
            z = imu_sensor_link.velocity.z
        } 
        // Change in meters per second per second (change of a change in speed)
        Vector3 acceleration = (velocity_now - last_velocity) / (timestamp_now.Subtract(last_msg_timestamp).Seconds);

        // Update time and velocity changes for next time
        last_msg_timestamp = timestamp_now;
        last_velocity = velocity_now;
        
        // Prepare msg
        var msg = new ImuMsg
        {
            header = new HeaderMsg
            {
                frame_id = imu_sensor_link.name,
                stamp = new TimeMsg
                {
                    sec = timestamp_now.Seconds,
                    nanosec = timestamp_now.NanoSeconds,
                }
            },
            orientation = new QuaternionMsg
            {
                x = imu_sensor_link.rotation.x,
                y = imu_sensor_link.rotation.y,
                z = imu_sensor_link.rotation.z,
                w = imu_sensor_link.rotation.w
            },
            angular_velocity = new Vector3Msg 
            {
                x = imu_sensor_link.angular_velocity.x,
                y = imu_sensor_link.angular_velocity.y,
                z = imu_sensor_link.angular_velocity.z
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