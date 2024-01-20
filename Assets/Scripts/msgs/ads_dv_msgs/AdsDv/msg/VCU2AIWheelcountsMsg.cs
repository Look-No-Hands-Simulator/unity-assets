//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.AdsDv
{
    [Serializable]
    public class VCU2AIWheelcountsMsg : Message
    {
        public const string k_RosMessageName = "ads_dv_msgs/VCU2AIWheelcounts";
        public override string RosMessageName => k_RosMessageName;

        //  Front left wheel speed pulse count [0 - 65535]
        public ushort fl_pulse_count;
        //  Front right wheel speed pulse count [0 - 65535]
        public ushort fr_pulse_count;
        //  Rear left wheel speed pulse count [0 - 65535]
        public ushort rl_pulse_count;
        //  Rear right wheel speed pulse count [0 - 65535]
        public ushort rr_pulse_count;

        public VCU2AIWheelcountsMsg()
        {
            this.fl_pulse_count = 0;
            this.fr_pulse_count = 0;
            this.rl_pulse_count = 0;
            this.rr_pulse_count = 0;
        }

        public VCU2AIWheelcountsMsg(ushort fl_pulse_count, ushort fr_pulse_count, ushort rl_pulse_count, ushort rr_pulse_count)
        {
            this.fl_pulse_count = fl_pulse_count;
            this.fr_pulse_count = fr_pulse_count;
            this.rl_pulse_count = rl_pulse_count;
            this.rr_pulse_count = rr_pulse_count;
        }

        public static VCU2AIWheelcountsMsg Deserialize(MessageDeserializer deserializer) => new VCU2AIWheelcountsMsg(deserializer);

        private VCU2AIWheelcountsMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.fl_pulse_count);
            deserializer.Read(out this.fr_pulse_count);
            deserializer.Read(out this.rl_pulse_count);
            deserializer.Read(out this.rr_pulse_count);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.fl_pulse_count);
            serializer.Write(this.fr_pulse_count);
            serializer.Write(this.rl_pulse_count);
            serializer.Write(this.rr_pulse_count);
        }

        public override string ToString()
        {
            return "VCU2AIWheelcountsMsg: " +
            "\nfl_pulse_count: " + fl_pulse_count.ToString() +
            "\nfr_pulse_count: " + fr_pulse_count.ToString() +
            "\nrl_pulse_count: " + rl_pulse_count.ToString() +
            "\nrr_pulse_count: " + rr_pulse_count.ToString();
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