//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.AdsDv
{
    [Serializable]
    public class AI2VCUBrakeMsg : Message
    {
        public const string k_RosMessageName = "ads_dv_msgs/AI2VCUBrake";
        public override string RosMessageName => k_RosMessageName;

        //  Requested hydraulic brake pressure [0 - 100] %
        public byte hyd_pressure_request_pct;

        public AI2VCUBrakeMsg()
        {
            this.hyd_pressure_request_pct = 0;
        }

        public AI2VCUBrakeMsg(byte hyd_pressure_request_pct)
        {
            this.hyd_pressure_request_pct = hyd_pressure_request_pct;
        }

        public static AI2VCUBrakeMsg Deserialize(MessageDeserializer deserializer) => new AI2VCUBrakeMsg(deserializer);

        private AI2VCUBrakeMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.hyd_pressure_request_pct);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.hyd_pressure_request_pct);
        }

        public override string ToString()
        {
            return "AI2VCUBrakeMsg: " +
            "\nhyd_pressure_request_pct: " + hyd_pressure_request_pct.ToString();
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
