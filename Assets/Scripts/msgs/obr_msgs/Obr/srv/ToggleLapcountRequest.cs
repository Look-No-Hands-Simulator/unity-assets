//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Obr
{
    [Serializable]
    public class ToggleLapcountRequest : Message
    {
        public const string k_RosMessageName = "obr_msgs/ToggleLapcount";
        public override string RosMessageName => k_RosMessageName;

        public bool set_to;

        public ToggleLapcountRequest()
        {
            this.set_to = false;
        }

        public ToggleLapcountRequest(bool set_to)
        {
            this.set_to = set_to;
        }

        public static ToggleLapcountRequest Deserialize(MessageDeserializer deserializer) => new ToggleLapcountRequest(deserializer);

        private ToggleLapcountRequest(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.set_to);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.set_to);
        }

        public override string ToString()
        {
            return "ToggleLapcountRequest: " +
            "\nset_to: " + set_to.ToString();
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