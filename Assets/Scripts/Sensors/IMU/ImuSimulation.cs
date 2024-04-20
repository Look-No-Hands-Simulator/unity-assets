using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
public class ImuSimulation
{
    GameObject imu_sensor_link;

    Rigidbody imu_physical;

    DateTime last_msg_timestamp;

    Vector3 last_velocity;

    Vector3 last_orientation;

    Vector3 last_position;

    GaussianGenerator gaussian_generator; 

    bool noise_activation;

    // 1% standard deviation for covariance observations
    const double INS_STANDARD_DEVIATION = 0.01;

    public ImuSimulation(GameObject imu_sensor_link_object, bool noise_activation_param) {

        imu_sensor_link = imu_sensor_link_object;
        noise_activation = noise_activation_param;
        imu_physical = imu_sensor_link.GetComponent<Rigidbody>();
        last_msg_timestamp = DateTime.Now;

        // linear
        last_velocity = new Vector3
        {
                x = imu_physical.velocity.x,
                y = imu_physical.velocity.y,
                z = imu_physical.velocity.z
        }; 

        last_orientation = new Vector3 
        {
            x = imu_physical.rotation.x,
            y = imu_physical.rotation.y,
            z = imu_physical.rotation.z

        };

        last_position = imu_sensor_link.transform.position;


        // Numbers between 0 and 0.03, 1% standard deviation
        // To get std of measurement we need to multiply measurement by the assumed std of 1% then square it to get covariance of independent variable
        gaussian_generator = new GaussianGenerator(0, INS_STANDARD_DEVIATION);

    }

    public ImuMsg get_imu_msg() {

        DateTime timestamp_now = DateTime.Now;

        Vector3 position_now = imu_sensor_link.transform.position;

        Vector3 velocity_now = (position_now - last_position) / (float)(timestamp_now - last_msg_timestamp).TotalSeconds; 
        //Debug.Log("Velocity_now: " + velocity_now);

        // Vector3 velocity_now = new Vector3 
        // {
        //     x = imu_physical.velocity.x,
        //     y = imu_physical.velocity.y,
        //     z = imu_physical.velocity.z
        // };

        Vector3 orientation_now = new Vector3 
        {
            x = imu_physical.rotation.x,
            y = imu_physical.rotation.y,
            z = imu_physical.rotation.z

        };


        // Change in meters per second per second (change of a change in speed)
        Vector3 acceleration = (velocity_now - last_velocity) / (float)(timestamp_now - last_msg_timestamp).TotalSeconds;

        Vector3 angular_velocity = (orientation_now - last_orientation) / (float)(timestamp_now - last_msg_timestamp).TotalSeconds;

        // Update time and velocity changes for next time
        last_msg_timestamp = timestamp_now;
        last_velocity = velocity_now;
        last_position = position_now;

        // Update orientation
        last_orientation = orientation_now;
        
        // Get Unix time, how long since Jan 1st 1970?
        TimeStamp msg_timestamp = new TimeStamp(Clock.time);

        //Vector3 angular_velocity = imu_physical.angularVelocity;
        //Debug.Log("Angular velocity: " + angular_velocity);
        Quaternion orientation = imu_physical.rotation;
        //Debug.Log("Orientation: " + orientation);

        if (noise_activation == true) {
            angular_velocity = gaussian_generator.add_noise_scale(angular_velocity);
            orientation = gaussian_generator.add_noise_scale(orientation);
            acceleration = gaussian_generator.add_noise_scale(acceleration);
        }

        // Double[9] array
        double[] orientation_covariance_calc = new double[9]{Math.Pow((orientation.x*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,
        Math.Pow((orientation.y*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,Math.Pow((orientation.z*INS_STANDARD_DEVIATION),2)};

        double[] angular_velocity_covariance_calc = new double[9]{Math.Pow((angular_velocity.x*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,
        Math.Pow((angular_velocity.y*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,Math.Pow((angular_velocity.z*INS_STANDARD_DEVIATION),2)};

        double[] linear_acceleration_covariance_calc = new double[9]{Math.Pow((acceleration.x*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,
        Math.Pow((acceleration.y*INS_STANDARD_DEVIATION),2),0.0,0.0,0.0,Math.Pow((acceleration.z*INS_STANDARD_DEVIATION),2)};

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
            orientation = new QuaternionMsg // radians per second
            {
                x = orientation.x,
                y = orientation.y,
                z = orientation.z,
                w = orientation.w
            },
            orientation_covariance = orientation_covariance_calc,
            angular_velocity = new Vector3Msg
            {
                x = angular_velocity.x,
                y = angular_velocity.y,
                z = angular_velocity.z
            } ,
            angular_velocity_covariance = angular_velocity_covariance_calc,
            linear_acceleration = new Vector3Msg
            {
                x = acceleration.x,
                y = acceleration.y,
                z = acceleration.z
            },
            linear_acceleration_covariance = linear_acceleration_covariance_calc
        };

        return msg;
    }
}