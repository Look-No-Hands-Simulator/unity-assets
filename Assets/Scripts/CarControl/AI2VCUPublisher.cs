using UnityEngine;

// Custom namespace msgs
using RosMessageTypes.AdsDv;

using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;

public class AI2VCUPublisher
{
    string ai2vcuSteerTopic;
    string ai2vcuDriveFTopic;
    string ai2vcuBrakeTopic;
    string ai2vcuDriveFReverseTopic;
    ROSConnection ros;

    public AI2VCUPublisher(string steerTopicName, string ai2vcuDriveFTopicName, string ai2vcuBrakeTopicName, string ai2vcuDriveFReverseTopicName) {
        ai2vcuSteerTopic = steerTopicName;
        ai2vcuDriveFTopic = ai2vcuDriveFTopicName;
        ai2vcuBrakeTopic = ai2vcuBrakeTopicName;
        ai2vcuDriveFReverseTopic = ai2vcuDriveFReverseTopicName;
        ros = ROSConnection.GetOrCreateInstance();
        // Register publisher names
        ros.RegisterPublisher<AI2VCUSteerMsg>(ai2vcuSteerTopic);
        ros.RegisterPublisher<AI2VCUDriveFMsg>(ai2vcuDriveFTopic);
        ros.RegisterPublisher<AI2VCUDriveFMsg>(ai2vcuDriveFReverseTopic);
        ros.RegisterPublisher<AI2VCUBrakeMsg>(ai2vcuBrakeTopic);
    }

    public void PublishAI2VCUSteerMsg(short steerDegrees) {
            
        ros.Publish(ai2vcuSteerTopic, new AI2VCUSteerMsg {
            steer_request_deg = steerDegrees
            }
        );

    }

    public void PublishAI2VCUDriveFMsg(ushort torqueNm, ushort maxMotorRPM) {

        ros.Publish(ai2vcuDriveFTopic, new AI2VCUDriveFMsg {
            front_axle_trq_request_nm = torqueNm,
            front_motor_speed_max_rpm = maxMotorRPM
            }
        );
    }

    public void PublishAI2VCUDriveFMsgReverse(ushort torqueNm, ushort maxMotorRPM) {

        ros.Publish(ai2vcuDriveFReverseTopic, new AI2VCUDriveFMsg {
            front_axle_trq_request_nm = torqueNm,
            front_motor_speed_max_rpm = maxMotorRPM
            }
        );
    }

    public void PublishAI2VCUBrakeMsg(byte brakePercent) {
        ros.Publish(ai2vcuBrakeTopic, new AI2VCUBrakeMsg {
            hyd_pressure_request_pct = brakePercent
            }
        );
    }
    

    // void Update() {
    //     time_elapsed += Time.deltaTime;
    //     if (time_elapsed > min_publishing_time) {
    //         ImageMsg right_imagemsg = stereo_camera_simulation.get_image_msg(right_camera);
    //         ImageMsg left_imagemsg = stereo_camera_simulation.get_image_msg(left_camera);

    //         ros.Publish(rightcamera_topic, right_imagemsg);
    //         ros.Publish(leftcamera_topic, left_imagemsg);

    //         time_elapsed = 0.0f;
    //     }
        


    // }
}