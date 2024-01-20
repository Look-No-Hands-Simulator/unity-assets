//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.AdsDv
{
    [Serializable]
    public class VCU2AIDriveRMsg : Message
    {
        public const string k_RosMessageName = "ads_dv_msgs/VCU2AIDriveR";
        public override string RosMessageName => k_RosMessageName;

        //  Current rear axle torque [-195 - 195] Nm
        public short rear_axle_trq_nm;
        //  Requested rear axle torque [-195 - 195] Nm
        public short rear_axle_trq_request_nm;
        //  Maximum rear axle drive torque [0 - 195] Nm
        public ushort rear_axle_trq_max_nm;

        public VCU2AIDriveRMsg()
        {
            this.rear_axle_trq_nm = 0;
            this.rear_axle_trq_request_nm = 0;
            this.rear_axle_trq_max_nm = 0;
        }

        public VCU2AIDriveRMsg(short rear_axle_trq_nm, short rear_axle_trq_request_nm, ushort rear_axle_trq_max_nm)
        {
            this.rear_axle_trq_nm = rear_axle_trq_nm;
            this.rear_axle_trq_request_nm = rear_axle_trq_request_nm;
            this.rear_axle_trq_max_nm = rear_axle_trq_max_nm;
        }

        public static VCU2AIDriveRMsg Deserialize(MessageDeserializer deserializer) => new VCU2AIDriveRMsg(deserializer);

        private VCU2AIDriveRMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.rear_axle_trq_nm);
            deserializer.Read(out this.rear_axle_trq_request_nm);
            deserializer.Read(out this.rear_axle_trq_max_nm);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.rear_axle_trq_nm);
            serializer.Write(this.rear_axle_trq_request_nm);
            serializer.Write(this.rear_axle_trq_max_nm);
        }

        public override string ToString()
        {
            return "VCU2AIDriveRMsg: " +
            "\nrear_axle_trq_nm: " + rear_axle_trq_nm.ToString() +
            "\nrear_axle_trq_request_nm: " + rear_axle_trq_request_nm.ToString() +
            "\nrear_axle_trq_max_nm: " + rear_axle_trq_max_nm.ToString();
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