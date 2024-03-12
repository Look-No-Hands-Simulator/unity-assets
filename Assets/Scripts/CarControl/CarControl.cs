using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Robotics.ROSTCPConnector;
// Custom namespace msgs
using RosMessageTypes.AdsDv;


public class CarControl : MonoBehaviour
{
    public ADS_DV_State adsdvStateObject;


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
    public string ai2vcuDriveFReverseTopic = "/AI2VCUDriveFReverse";
    public string ai2vcuBrakeTopic = "/AI2VCUBrake";

    AI2VCUPublisher ai2vcuPublisherNode;

    float actuateLeftSteer;
    float actuateRightSteer;

    ushort actuateThrottleFrontForce;

    ushort brakingPercent;

    bool reverse;

    public bool reverseOn;


    // // Start is called before the first frame update
    void Start()
    {
        foreach (WheelElements element in wheelData) {
            RotateTyresInitial(element.leftWheel);
            RotateTyresInitial(element.rightWheel);
        }

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = massCenter.localPosition;

        // Create all of the publishers by creating an object to do this, these publishers can publish the keyboard control outputs
        ai2vcuPublisherNode = new AI2VCUPublisher(ai2vcuSteerTopic,ai2vcuDriveFTopic,ai2vcuBrakeTopic,ai2vcuDriveFReverseTopic);

        // Subscribe to publishers that publish actuation commands
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUSteerMsg>(ai2vcuSteerTopic, ActuateSteering);
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUDriveFMsg>(ai2vcuDriveFTopic, ActuateThrottle);
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUDriveFMsg>(ai2vcuDriveFReverseTopic, ActuateThrottleReverse);
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUBrakeMsg>(ai2vcuBrakeTopic, ActuateBraking);

        this.reverse = false;

    }

    void ActuateSteering(AI2VCUSteerMsg steerMsg) {
        // Code below recalculates left and right steer from a hypothetical middle wheel
        // TODO: Create subscriber to control the wheels, publish middle steer, create AI2VCUPublisher and publish middle steer in here in this script

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING || adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_READY) {
            
            short middleSteer = steerMsg.steer_request_deg;

            //float steerFraction = (middleSteer * 2) / (maxInnerSteeringAngle + maxOuterSteeringAngle);

            /// Scale/normalize the steerFraction relative to the maximum steering angles of the outer
            // and inner wheels depending on the direction of the middle steering angle
            // the steerFraction will give the angle for a hypothetical middle
            // wheel which can then be * by the max of inner and outer to give ackermann steering
            float steerFraction;
            if (middleSteer > 0) {
                // Inner wheel as we turn left with positive number
                steerFraction = middleSteer / maxInnerSteeringAngle;
            } else {
                // Outer wheel as we turn right with negative number
                steerFraction = middleSteer / maxOuterSteeringAngle;
            }

            /// Set the angles to the wheels
            if (middleSteer > 0) {
                // Positive steering angle indicates turn to the left
                this.actuateRightSteer = steerFraction * maxInnerSteeringAngle;
                this.actuateLeftSteer = steerFraction * maxOuterSteeringAngle;
            } else {
                // Negative steering angle indicates turn to the right
                this.actuateRightSteer = steerFraction * maxOuterSteeringAngle;
                this.actuateLeftSteer = steerFraction * maxInnerSteeringAngle;
            }



        // Update state
        // Get value off wheel collider or use these values
        // TODO: Is this correct?
        //////////////
        adsdvStateObject.Actual_steer_angle = middleSteer;
        adsdvStateObject.Steer_angle_request = steerMsg.steer_request_deg;

        }

    }

    void ActuateThrottle(AI2VCUDriveFMsg driveFMsg) {

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING) {
            this.reverse = false;
            this.actuateThrottleFrontForce = driveFMsg.front_axle_trq_request_nm;
            this.brakingPercent = 0;
        }

        
    }

    void ActuateThrottleReverse(AI2VCUDriveFMsg driveFMsg) {

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING) {
            this.reverse = true;
            this.actuateThrottleFrontForce = driveFMsg.front_axle_trq_request_nm;
            this.brakingPercent = 0;
        }
        
    }


    private void FixedUpdate() {

        // Fraction of how much torque you could be applying because Input.GetAxis is between 0-1
        /// This is the problem -> vertical is a ushort so it gets converted to unsigned meaning it is never negative
        float speed = (Input.GetAxis("Vertical")*maxTorque);
        float steer = Input.GetAxis("Horizontal")*maxSteerAngle;
        float brake = Input.GetAxis("Brake");

        //Debug.Log("Check input space key:" + brake);

        float leftSteer;
        float rightSteer;
        float middleSteer;
        //float steerFraction;
        
        // Steering keyboard control
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

        // Throttle keyboard control
        if (Input.GetAxis("Vertical") > 0 && brake <= 0) {
            // Forwards
            //Debug.Log("Throttle: " + speed);
            ai2vcuPublisherNode.PublishAI2VCUDriveFMsg((ushort)(speed), this.maxRPM);
        } else if (Input.GetAxis("Vertical") < 0 && brake <= 0) {
            // Reversing
            // Publish msg but the motorTorque will have to be negative when the wheel colliders are actuated in Unity and
            // therefore not be actuated from the topic since it can't publish negative, unless the topic was reverse
            ai2vcuPublisherNode.PublishAI2VCUDriveFMsgReverse((ushort)(Math.Abs(speed)), this.maxRPM);

        }
        // Implement this 
        //Debug.Log("Braking: " + Input.GetAxis("Vertical"));
        ai2vcuPublisherNode.PublishAI2VCUBrakeMsg((byte)(brake*100));


        foreach (WheelElements element in wheelData) {

            if (element.shouldSteer == true) {
                element.leftWheel.steerAngle = this.actuateLeftSteer;
                element.rightWheel.steerAngle = this.actuateRightSteer;
            }
            // Only apply throttle if there is no braking so we can't accelerate and brake at the same time
            // The problem is if we accelerate really hard then press the brakes the brakes won't reset back to 0 so this won't run through
            if (this.brakingPercent == 0 && element.addWheelTorque == true && this.reverse == false) {
                element.leftWheel.motorTorque = this.actuateThrottleFrontForce;
                element.rightWheel.motorTorque = this.actuateThrottleFrontForce;
                // When the brakes are applied they don't automatically set the brakeTorque back to 0 so we need to make sure we do that
                element.leftWheel.brakeTorque = 0;
                element.rightWheel.brakeTorque = 0;

            } 
            else if (this.reverse == true && this.brakingPercent == 0 && element.addWheelTorque == true && this.reverseOn == true) {
                element.leftWheel.motorTorque = ((float)(this.actuateThrottleFrontForce)) * -1;
                element.rightWheel.motorTorque = ((float)(this.actuateThrottleFrontForce)) * -1;
                element.leftWheel.brakeTorque = 0;
                element.rightWheel.brakeTorque = 0;

            } 
            // else if (this.brakingPercent == 0 && element.leftWheel.brakeTorque != 0) {
            //     element.leftWheel.brakeTorque = 0;
            //     element.rightWheel.brakeTorque = 0;
            //     element.leftWheel.motorTorque = 0;
            //     element.rightWheel.motorTorque = 0;
            //     element.leftWheel.motorTorque = 0;
            //     element.rightWheel.motorTorque = 0;
            // }
            else {
                // Brakes on
                element.leftWheel.motorTorque = 0;
                element.rightWheel.motorTorque = 0;
            }

            // If we release the brakes then accelpercent = 0 but brakepercent also = 0 so...

            // Debug.Log("Motor torque left wheel: " + element.leftWheel.motorTorque);
            // Debug.Log("Motor torque right wheel: " + element.rightWheel.motorTorque);
            // Move tyres
            DoTyres(element.leftWheel);
            DoTyres(element.rightWheel);
        }
    }

    void ActuateBraking(AI2VCUBrakeMsg brakeMsg) {
        byte brakingPercentRequest = brakeMsg.hyd_pressure_request_pct;
        float maxBrakingTorque = maxTorque * 4;
        this.brakingPercent = (ushort)brakingPercentRequest;
        float brakingTorque;

        //Debug.Log("Braking Percent: " + this.brakingPercent);

        if (brakingPercentRequest > 0) {
            // Stops car from moving forwards after braking bug, but check if car was rolling since the brake needs a release button for safety
            this.actuateThrottleFrontForce = 0;
            foreach (WheelElements element in wheelData) {

                //Debug.Log("Brakes fast: " + (maxBrakingTorque * brakingPercentRequest / 100));
                element.leftWheel.brakeTorque = maxBrakingTorque * brakingPercentRequest / 100;
                element.rightWheel.brakeTorque = maxBrakingTorque * brakingPercentRequest / 100;

                // Left wheel
                // Adjust this so the car cannot go backwards when braking at high force because we use a reverse force rather than a braking plate
                // / rpm by 60 to get rps
                // // 0.2 is a fifth of a second
                // if (element.leftWheel.rpm / 60 > 0.2) {
                //     // More than 4.5 mph
                //     // Apply max braking
                //     // Get percentage of torque and make it negative because it's a negative force against the wheel

                // } else {
                //     // Apply partial braking when brake has nearly stopped, this is to prevent vehicle going backwards
                //     // Multiply braking torque by number of spins per second of wheel, makes braking force smaller as wheel has nearly stopped
                //     element.leftWheel.brakeTorque = (maxBrakingTorque * (brakingPercentRequest / 100)) * Math.Max(element.leftWheel.rpm / 60, 0) * 5;

                //     Debug.Log("Brakes slow: " + (maxBrakingTorque * brakingPercentRequest / 100) * Math.Max(element.leftWheel.rpm / 60, 0) * 5);
                // }
                // // Right wheel
                // if (element.rightWheel.rpm / 60 > 0.2) {
                //     // More than 4.5 mph
                //     // Apply max braking
                    

                // } else {
                //     // Apply partial braking
                //     element.rightWheel.brakeTorque = (maxBrakingTorque * (brakingPercentRequest / 100)) * (element.rightWheel.rpm / 60) * 5;

                // }

                // Move tyres
                DoTyres(element.leftWheel);
                DoTyres(element.rightWheel);
            }
        }

        // This is not necessary
        if (brakingPercentRequest == 0) {
            foreach (WheelElements element in wheelData) {
                element.leftWheel.brakeTorque = 0;
                element.rightWheel.brakeTorque = 0;
                element.leftWheel.motorTorque = 0;
                element.rightWheel.motorTorque = 0;
            }
        }
        


    }


    void DoTyres(WheelCollider collider) {
        // This moves the tyre 3D models along with the wheel colliders

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