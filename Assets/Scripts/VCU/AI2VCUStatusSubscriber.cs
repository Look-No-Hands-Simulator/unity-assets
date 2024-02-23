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
using Unity.Robotics.ROSTCPConnector;

public class AI2VCUStatusSubscriber: MonoBehaviour
{

    public ADS_DV_State adsdvState;

    public string ai2vcuStatusTopic = "/AI2VCUStatus";

    //ROSConnection ros;

    void Start() {
        // ros = ROSConnection.GetOrCreateInstance();
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUStatusMsg>(ai2vcuStatusTopic, AI2VCUSubscriberManager);

        // ros.RegisterPublisher<VCU2AIWheelspeedsMsg>(vcu2ai_status_topic);

    }

    void Update() {

        // VCU2AIStatusMsg vcu2ai_status_msg = adsdv_state.get_vcu2aiStatus_msg();

        // ros.Publish(vcu2ai_status_topic, vcu2ai_status_msg);
    }

    public void AI2VCUSubscriberManager(AI2VCUStatusMsg statusMsg) {
        // Get values from the msg and assign them into the ADS_DV_State 

        // this.handshake = false;
        // this.estop_request = false;
        // this.mission_status = 0;
        // this.direction_request = 0;
        // this.lap_counter = 0;
        // this.cones_count_actual = 0;
        // this.cones_count_all = 0;
        // this.veh_speed_actual_kmh = 0;
        // this.veh_speed_demand_kmh = 0;


    }
}