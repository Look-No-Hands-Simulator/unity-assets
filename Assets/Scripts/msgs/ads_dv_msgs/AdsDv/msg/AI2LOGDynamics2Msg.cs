//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.AdsDv
{
    [Serializable]
    public class AI2LOGDynamics2Msg : Message
    {
        public const string k_RosMessageName = "ads_dv_msgs/AI2LOGDynamics2";
        public override string RosMessageName => k_RosMessageName;

        //  [-64 - 63.998] m/s^2
        public short accel_longitudinal_mps2;
        //  [-64 - 63.998] m/s^2
        public short accel_lateral_mps2;
        //  [-256 - 255.992] deg/s
        public short yaw_rate_degps;

        public AI2LOGDynamics2Msg()
        {
            this.accel_longitudinal_mps2 = 0;
            this.accel_lateral_mps2 = 0;
            this.yaw_rate_degps = 0;
        }

        public AI2LOGDynamics2Msg(short accel_longitudinal_mps2, short accel_lateral_mps2, short yaw_rate_degps)
        {
            this.accel_longitudinal_mps2 = accel_longitudinal_mps2;
            this.accel_lateral_mps2 = accel_lateral_mps2;
            this.yaw_rate_degps = yaw_rate_degps;
        }

        public static AI2LOGDynamics2Msg Deserialize(MessageDeserializer deserializer) => new AI2LOGDynamics2Msg(deserializer);

        private AI2LOGDynamics2Msg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.accel_longitudinal_mps2);
            deserializer.Read(out this.accel_lateral_mps2);
            deserializer.Read(out this.yaw_rate_degps);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.accel_longitudinal_mps2);
            serializer.Write(this.accel_lateral_mps2);
            serializer.Write(this.yaw_rate_degps);
        }

        public override string ToString()
        {
            return "AI2LOGDynamics2Msg: " +
            "\naccel_longitudinal_mps2: " + accel_longitudinal_mps2.ToString() +
            "\naccel_lateral_mps2: " + accel_lateral_mps2.ToString() +
            "\nyaw_rate_degps: " + yaw_rate_degps.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
