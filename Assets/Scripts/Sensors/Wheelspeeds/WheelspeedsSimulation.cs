using UnityEngine;
// Custom namespace msgs
using RosMessageTypes.AdsDv;

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;

public class WheelspeedsSimulation : MonoBehaviour
{
    WheelCollider fl_wheel_collider;
    WheelCollider fr_wheel_collider;
    WheelCollider bl_wheel_collider;
    WheelCollider br_wheel_collider;

    public WheelspeedsSimulation(GameObject fl_wheel_param, GameObject fr_wheel_param, GameObject bl_wheel_param, GameObject br_wheel_param) {
        fl_wheel_collider = fl_wheel_param.GetComponent<WheelCollider>();
        fr_wheel_collider = fr_wheel_param.GetComponent<WheelCollider>();
        bl_wheel_collider = bl_wheel_param.GetComponent<WheelCollider>();
        br_wheel_collider = br_wheel_param.GetComponent<WheelCollider>();
    }

    public VCU2AIWheelspeedsMsg get_vcu2aiwheelspeeds_msg() {

        return new VCU2AIWheelspeedsMsg {
            fl_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(fl_wheel_collider.rpm)),
            fr_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(fr_wheel_collider.rpm)),
            rl_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(bl_wheel_collider.rpm)),
            rr_wheel_speed_rpm = Convert.ToUInt16(Math.Abs(br_wheel_collider.rpm))
        };

    }
}