using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
// Custom namespace msgs
using RosMessageTypes.AdsDv;


public class CarControl : MonoBehaviour
{
    public List<WheelElements> wheelData;

    public short maxTorque;
    public ushort maxRPM;
    public float maxSteerAngle = 28;

    public float maxInnerSteeringAngle = 28F;
    public float maxOuterSteeringAngle = 24.6F;

    private Rigidbody rb;
    public Transform massCenter;

    private int counter; // Every fixed update counter 50 then we can log data

    [SerializeField] private float force = 10;
    //[SerializeField] private float speed = 5;


    // Publisher info
    public string ai2vcuSteerTopic = "/AI2VCUSteer";
    public string ai2vcuDriveFTopic = "/AI2VCUDriveF";

    AI2VCUPublisher ai2vcuPublisherNode;

    float actuateLeftSteer;
    float actuateRightSteer;

    ushort actuateThrottleFrontForce;


    // // Start is called before the first frame update
    void Start()
    {
        foreach (WheelElements element in wheelData) {
            RotateTyresInitial(element.leftWheel);
            RotateTyresInitial(element.rightWheel);
        }

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = massCenter.localPosition;

        // Create all of the publishers by creating an object to do this
        ai2vcuPublisherNode = new AI2VCUPublisher(ai2vcuSteerTopic,ai2vcuDriveFTopic);

        // Subscribe to publishers that publish actuation commands
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUSteerMsg>(ai2vcuSteerTopic, ActuateSteering);
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUDriveFMsg>(ai2vcuDriveFTopic, ActuateThrottle);

    }

    void ActuateSteering(AI2VCUSteerMsg steerMsg) {
        // Code below recalculates left and right steer from a hypothetical middle wheel
        // TODO: Create subscriber to control the wheels, publish middle steer, create AI2VCUPublisher and publish middle steer in here in this script
        short middleSteer = steerMsg.steer_request_deg;
        float steerFraction = (middleSteer * 2) / (maxInnerSteeringAngle + maxOuterSteeringAngle);
        if (middleSteer > 0) {
            this.actuateRightSteer = steerFraction * maxInnerSteeringAngle;
            this.actuateLeftSteer = steerFraction * maxOuterSteeringAngle;
        } else {
            this.actuateRightSteer = steerFraction * maxOuterSteeringAngle;
            this.actuateLeftSteer = steerFraction * maxInnerSteeringAngle;
        }
    }

    void ActuateThrottle(AI2VCUDriveFMsg driveFMsg) {
        this.actuateThrottleFrontForce = driveFMsg.front_axle_trq_request_nm;
    }


    private void FixedUpdate() {

        ushort speed = (ushort)(Input.GetAxis("Vertical")*maxTorque);
        float steer = Input.GetAxis("Horizontal")*maxSteerAngle;

        float leftSteer;
        float rightSteer;
        float middleSteer;
        //float steerFraction;

        if (Input.GetAxis("Horizontal") < 0) {
            leftSteer = Input.GetAxis("Horizontal")*maxInnerSteeringAngle;
            rightSteer = Input.GetAxis("Horizontal")*maxOuterSteeringAngle;
        } else {
            rightSteer = Input.GetAxis("Horizontal")*maxInnerSteeringAngle;
            leftSteer = Input.GetAxis("Horizontal")*maxOuterSteeringAngle;
        }

        // if (++counter%50 == 0) {
        //     Debug.Log("Horizontal: " + Input.GetAxis("Horizontal"));
        //     counter = 0;
        // }

        // Middle steer calculations for the keys publishing while preserving ackermann steering of seperate left and right wheel angles
        middleSteer = (leftSteer + rightSteer) / 2;
        
        ai2vcuPublisherNode.PublishAI2VCUSteerMsg((short)(middleSteer));
        ai2vcuPublisherNode.PublishAI2VCUDriveFMsg((speed), this.maxRPM);

        foreach (WheelElements element in wheelData) {

            if (element.shouldSteer == true) {
                element.leftWheel.steerAngle = this.actuateLeftSteer;
                element.rightWheel.steerAngle = this.actuateRightSteer;
            }
            if (element.addWheelTorque == true) {
                element.leftWheel.motorTorque = this.actuateThrottleFrontForce;
                element.rightWheel.motorTorque = this.actuateThrottleFrontForce;
            }

            // Move tyres
            DoTyres(element.leftWheel);
            DoTyres(element.rightWheel);
        }
    }


    void DoTyres(WheelCollider collider) {

        if (collider.transform.childCount == 0) {
            return;
        }
        
        Transform tyre = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        tyre.transform.position = position;
        tyre.transform.rotation = rotation;
    }

    void RotateTyresInitial(WheelCollider collider) {
        // Fix bug of wheels turning 90 degrees on start and rolling in wrong dimension
        if (collider.transform.childCount == 0) {
            return;
        }

        Transform tyre = collider.transform.GetChild(0);

        Quaternion rotation = new Quaternion(1f,0f,90f,0f);

        tyre.transform.rotation = rotation;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody rbo = hit.collider.attachedRigidbody;
        if (rbo == null || rbo.isKinematic) return;
        // rb should be the Object or character controller of the car and wheels
        rbo.AddForceAtPosition(force * this.rb.velocity.normalized, hit.point);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}

[System.Serializable]
public class WheelElements {

public WheelCollider leftWheel;
public WheelCollider rightWheel;

public bool addWheelTorque;
public bool shouldSteer;

}