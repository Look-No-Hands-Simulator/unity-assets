//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Obr
{
    [Serializable]
    public class SendStatusResponse : Message
    {
        public const string k_RosMessageName = "obr_msgs/SendStatus";
        public override string RosMessageName => k_RosMessageName;

        public bool success;

        public SendStatusResponse()
        {
            this.success = false;
        }

        public SendStatusResponse(bool success)
        {
            this.success = success;
        }

        public static SendStatusResponse Deserialize(MessageDeserializer deserializer) => new SendStatusResponse(deserializer);

        private SendStatusResponse(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.success);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.success);
        }

        public override string ToString()
        {
            return "SendStatusResponse: " +
            "\nsuccess: " + success.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Response);
        }
    }
}
