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

public class WheelspeedsSimulation
{
    WheelCollider fl_wheel_collider;
    WheelCollider fr_wheel_collider;
    WheelCollider bl_wheel_collider;
    WheelCollider br_wheel_collider;
    bool noise_activation;
    GaussianGenerator gaussian_generator;

    public WheelspeedsSimulation(GameObject fl_wheel_param, GameObject fr_wheel_param, GameObject bl_wheel_param, GameObject br_wheel_param, 
        bool noise_activation_param) {
        fl_wheel_collider = fl_wheel_param.GetComponent<WheelCollider>();
        fr_wheel_collider = fr_wheel_param.GetComponent<WheelCollider>();
        bl_wheel_collider = bl_wheel_param.GetComponent<WheelCollider>();
        br_wheel_collider = br_wheel_param.GetComponent<WheelCollider>();
        
        noise_activation = noise_activation_param;

        // Numbers between 0 and 0.03, 1% standard deviation
        gaussian_generator = new GaussianGenerator(0, 0.01);
    }

    public VCU2AIWheelspeedsMsg get_vcu2aiwheelspeeds_msg() {
        // Work out wheelspeeds
        double fl_wheel_speed_rpm_val = fl_wheel_collider.rpm;
        double fr_wheel_speed_rpm_val = fr_wheel_collider.rpm;
        double rl_wheel_speed_rpm_val = bl_wheel_collider.rpm;
        double rr_wheel_speed_rpm_val = br_wheel_collider.rpm;

        // Add or don't add noise
        if (noise_activation == true) {
            fl_wheel_speed_rpm_val = gaussian_generator.add_noise_scale(fl_wheel_speed_rpm_val);
            fr_wheel_speed_rpm_val = gaussian_generator.add_noise_scale(fr_wheel_speed_rpm_val);
            rl_wheel_speed_rpm_val = gaussian_generator.add_noise_scale(rl_wheel_speed_rpm_val);
            rr_wheel_speed_rpm_val = gaussian_generator.add_noise_scale(rr_wheel_speed_rpm_val);
        }

        return new VCU2AIWheelspeedsMsg {
            fl_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(fl_wheel_collider.rpm)),
            fr_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(fr_wheel_collider.rpm)),
            rl_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(bl_wheel_collider.rpm)),
            rr_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(br_wheel_collider.rpm))
        };

    }

    public VCU2AIWheelcountsMsg get_vcu2aiwheelcounts_msg() {

        // Need diameter of wheel, how many teeth there are, wheelspeeds degrees per second
        // each revolution has fixed no of teeth, each msg has fixed no of pulses, know when it arrived
        

        // (angular velocity / (2 * PI)) * WHEELSPEEDS_TEETH
        ushort fl_pulse_count = (ushort)((Math.Abs(fl_wheel_collider.rotationSpeed) * CarConfig.WHEELSPEEDS_TEETH) / 360); // out of 360 degrees since rotationSpeed is measured in degrees per second
        ushort fr_pulse_count = (ushort)((Math.Abs(fr_wheel_collider.rotationSpeed) * CarConfig.WHEELSPEEDS_TEETH) / 360);
        ushort rl_pulse_count = (ushort)((Math.Abs(bl_wheel_collider.rotationSpeed) * CarConfig.WHEELSPEEDS_TEETH) / 360);
        ushort rr_pulse_count = (ushort)((Math.Abs(br_wheel_collider.rotationSpeed) * CarConfig.WHEELSPEEDS_TEETH) / 360);

        

        return new VCU2AIWheelcountsMsg (
            fl_pulse_count,
            fr_pulse_count,
            rl_pulse_count,
            rr_pulse_count
        );



    }
}