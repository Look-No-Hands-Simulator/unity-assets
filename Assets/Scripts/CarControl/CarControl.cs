using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Robotics.ROSTCPConnector;
// Custom namespace msgs
using RosMessageTypes.AdsDv;

public class SteeringFilter {

    // Moving average filter smooths the steering values
    // coming in from AI2VCUSteer
    // ChatGPT help

    private Queue<float> values = new Queue<float>();
    private int windowSize = 2; // Adjust this value as needed
    
    public float Filter(float value) {
        
        values.Enqueue(value);
        if (values.Count > windowSize) {
            values.Dequeue();
        }

        float sum = 0;
        foreach (float v in values) {
            sum += v;
        }

        return sum / values.Count;
    }
}


public class CarControl : MonoBehaviour
{

    public bool steeringSmoothing = true; // Switch between bang-bang and smoothed steering

    public bool invertSteering = true;

    private bool throttleRequest = false;

    public float update_interval = 0.2f;
    private float time_elapsed = 0.0f;


    public ADS_DV_State adsdvStateObject;


    public List<WheelElements> wheelData;

    public short maxTorque = 4000;
    public ushort maxRPM = 150;
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

    private SteeringFilter steeringFilter;


    // // Start is called before the first frame update
    void Start()
    {

        this.steeringFilter = new SteeringFilter();


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
        ROSConnection.GetOrCreateInstance().Subscribe<AI2VCUBrakeMsg>(ai2vcuBrakeTopic, ProcessBrakeMessage);

        this.reverse = false;

    }


    void ActuateSteering(AI2VCUSteerMsg steerMsg) {
        // Code below recalculates left and right steer from a hypothetical middle wheel
        // TODO: Create subscriber to control the wheels, publish middle steer, create AI2VCUPublisher and publish middle steer in here in this script


            //time_elapsed += Time.deltaTime;

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING || adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_READY) {
            
            short middleSteer = (short)(steerMsg.steer_request_deg);


            if (steeringSmoothing == true) {

                // Smooth the steering command
                middleSteer = (short)(steeringFilter.Filter(middleSteer));

            }


            if (invertSteering == true) {

                // OBR stack has inverted steering

                middleSteer = (short)(middleSteer * -1);

            }


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

                time_elapsed = 0;


            } else if (middleSteer < 0) {
                // Negative steering angle indicates turn to the right
                this.actuateRightSteer = steerFraction * maxOuterSteeringAngle;
                this.actuateLeftSteer = steerFraction * maxInnerSteeringAngle;

                time_elapsed = 0;

            } else {

                
                if (steeringSmoothing == true) {

                    // Timer ensures the car does a message steer action for at least 0.2 seconds before it is allowed to go straight given a 0 value

                    if (time_elapsed > update_interval) {

                    this.actuateRightSteer = 0;
                    this.actuateLeftSteer = 0;

                    }

                } else {

                    // 0 forward value

                    this.actuateRightSteer = 0;
                    this.actuateLeftSteer = 0;


                }





            }

            //Debug.Log("Leftsteer: " + this.actuateLeftSteer + " Rightsteer: " + this.actuateRightSteer + " Steer_angle_request: " + steerMsg.steer_request_deg);



        // Update state
        // Get value off wheel collider or use these values
        // TODO: Is this correct?
        //////////////
        adsdvStateObject.Actual_steer_angle = middleSteer;
        adsdvStateObject.Steer_angle_request = steerMsg.steer_request_deg;

        }

    }

    void ActuateThrottle(AI2VCUDriveFMsg driveFMsg) {

        // Set variables valid for front throttle and give it the
        // value in the driveFmsg

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING) {


            this.reverse = false;
            this.actuateThrottleFrontForce = driveFMsg.front_axle_trq_request_nm;
            this.maxRPM = driveFMsg.front_motor_speed_max_rpm;
            this.brakingPercent = 0;
            this.throttleRequest = true;

        }



        
    }

    void ActuateThrottleReverse(AI2VCUDriveFMsg driveFMsg) {

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_AS_DRIVING) {

            this.reverse = true;
            this.actuateThrottleFrontForce = driveFMsg.front_axle_trq_request_nm;
            this.maxRPM = driveFMsg.front_motor_speed_max_rpm;
            this.brakingPercent = 0;
            this.throttleRequest = true;



        }
        
    }


    private void FixedUpdate() {

        foreach (WheelElements element in wheelData) {

            // If the wheels are spinning at faster than maxRPM then don't apply
            // any more throttle

            if (Math.Abs(element.leftWheel.rpm) >= maxRPM || Math.Abs(element.rightWheel.rpm) >= maxRPM) {

                throttleRequest = false;
            }

        }

        // bool log = false;
        // time_elapsed += Time.deltaTime;
        // if (time_elapsed > update_interval) {

        //     log = true;

        //     Debug.Log("MaxRPM: " + this.maxRPM);
        //     Debug.Log("MaxTorque: " + this.maxTorque);
        //     Debug.Log("actuateThrottleFrontForce: " + actuateThrottleFrontForce);
        //     foreach (WheelElements element in wheelData) {
                
        //         if (element.addWheelTorque) {

        //             Debug.Log("Left wheel rpm: " + element.leftWheel.rpm);
        //             Debug.Log("Right wheel rpm: " + element.rightWheel.rpm);
        //         }
        //     }


        //     time_elapsed = 0;
        // }



        // Fraction of how much torque you could be applying because Input.GetAxis is between 0-1
        /// This is the problem -> vertical is a ushort so it gets converted to unsigned meaning it is never negative
        float speed = (Input.GetAxis("Vertical")*maxTorque);
        float steer = Input.GetAxis("Horizontal")*maxSteerAngle;
        float brake = Input.GetAxis("Brake");

        float leftSteer;
        float rightSteer;
        float middleSteer;
        
        // Steering keyboard control
        if (Input.GetAxis("Horizontal") < 0) {
            leftSteer = Input.GetAxis("Horizontal")*maxInnerSteeringAngle;
            rightSteer = Input.GetAxis("Horizontal")*maxOuterSteeringAngle;
        } else {
            rightSteer = Input.GetAxis("Horizontal")*maxInnerSteeringAngle;
            leftSteer = Input.GetAxis("Horizontal")*maxOuterSteeringAngle;
        }


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

        else if (Input.GetAxis("Brake") > 0) {
            ai2vcuPublisherNode.PublishAI2VCUBrakeMsg((byte)(brake*100));

        }

        if (adsdvStateObject.GetAsState() == ADS_DV_State.AS_STATE_EMERGENCY_BRAKE) {

            ActuateEmergencyBrake();
        }
        

        
        foreach (WheelElements element in wheelData) {

            // Change the wheel colliders and 3D models
            if (element.shouldSteer == true) {
                element.leftWheel.steerAngle = this.actuateLeftSteer;
                element.rightWheel.steerAngle = this.actuateRightSteer;
            }



            if (this.brakingPercent > 0) {

                float maxBrakingTorque = this.maxTorque * 4;

                // Stops car from moving forwards after braking bug, but check if car was rolling since the brake needs a release button for safety
                this.actuateThrottleFrontForce = 0;
                

                    element.leftWheel.brakeTorque = maxBrakingTorque * this.brakingPercent / 100;
                    element.rightWheel.brakeTorque = maxBrakingTorque * this.brakingPercent / 100;

                    // Move tyres
                    // DoTyres(element.leftWheel);
                    // DoTyres(element.rightWheel);
                
            }

            //
            else if (this.brakingPercent == 0) {
                
                    // element.leftWheel.brakeTorque = 0;
                    // element.rightWheel.brakeTorque = 0;
                    // element.leftWheel.motorTorque = 0;
                    // element.rightWheel.motorTorque = 0;
                
            }



            // Only apply throttle if there is no braking so we can't accelerate and brake at the same time
            // The problem is if we accelerate really hard then press the brakes the brakes won't reset back to 0 so this won't run through

            // Forwards
            if (this.brakingPercent == 0 && element.addWheelTorque == true && this.reverse == false) {





                if (element.leftWheel.rpm < this.maxRPM && element.rightWheel.rpm < this.maxRPM) {

                    // if (log) {

                    //     Debug.Log("Applying motor torque: ");
                    // }

                    element.leftWheel.motorTorque = this.actuateThrottleFrontForce;
                    element.rightWheel.motorTorque = this.actuateThrottleFrontForce;
                    // When the brakes are applied they don't automatically set the brakeTorque back to 0 so we need to make sure we do that
                    element.leftWheel.brakeTorque = 0;
                    element.rightWheel.brakeTorque = 0;
                }
                else {

                    // If one of the wheels is going at max RPM

                    element.leftWheel.motorTorque = 0;
                    element.rightWheel.motorTorque = 0;

                }
                

            } 
            // Reverse
            else if (this.reverse == true && this.brakingPercent == 0 && element.addWheelTorque == true && this.reverseOn == true) {

                if (element.leftWheel.rpm > (this.maxRPM * -1) && element.rightWheel.rpm > (this.maxRPM * -1)) {
                    element.leftWheel.motorTorque = ((float)(this.actuateThrottleFrontForce)) * -1;
                    element.rightWheel.motorTorque = ((float)(this.actuateThrottleFrontForce)) * -1;
                    element.leftWheel.brakeTorque = 0;
                    element.rightWheel.brakeTorque = 0;
                }

            } 
            // else if (this.brakingPercent == 0 && element.leftWheel.brakeTorque != 0 && element.rightWheel.brakeTorque != 0) {
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

            if (!throttleRequest || element.leftWheel.rpm >= maxRPM || element.rightWheel.rpm  >= maxRPM) {

                // if (log) {

                //     Debug.Log("Cancelling throttle torque");
                // }

                element.leftWheel.motorTorque = 0;
                element.rightWheel.motorTorque = 0;

            }

            // Debug.Log("Motor torque left wheel: " + element.leftWheel.motorTorque);
            // Debug.Log("Motor torque right wheel: " + element.rightWheel.motorTorque);
            // Move tyres
            DoTyres(element.leftWheel);
            DoTyres(element.rightWheel);

        }

        this.throttleRequest = false;
    }

    void ActuateEmergencyBrake() {

        this.brakingPercent = 100;

    }

    void ProcessBrakeMessage(AI2VCUBrakeMsg brakeMsg) {
        byte brakingPercentRequest = brakeMsg.hyd_pressure_request_pct;
        this.brakingPercent = (ushort)brakingPercentRequest;
    }

    void ActuateBraking() {
        // byte brakingPercentRequest = brakeMsg.hyd_pressure_request_pct;
        // this.brakingPercent = (ushort)brakingPercentRequest;
        

        if (this.brakingPercent > 0) {

            float maxBrakingTorque = this.maxTorque * 4;

            // Stops car from moving forwards after braking bug, but check if car was rolling since the brake needs a release button for safety
            this.actuateThrottleFrontForce = 0;
            foreach (WheelElements element in wheelData) {

                element.leftWheel.brakeTorque = maxBrakingTorque * this.brakingPercent / 100;
                element.rightWheel.brakeTorque = maxBrakingTorque * this.brakingPercent / 100;

                // Move tyres
                DoTyres(element.leftWheel);
                DoTyres(element.rightWheel);
            }
        }

        // This is not necessary
        else if (this.brakingPercent == 0) {
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