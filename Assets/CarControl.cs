using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public List<WheelElements> wheelData;

    public float maxTorque;
    public float maxSteerAngle = 28;

    public float maxInnerSteeringAngle = 28F;
    public float maxOuterSteeringAngle = 24.6F;

    private Rigidbody rb;
    public Transform massCenter;

    private int counter; // Every fixed update counter 50 then we can log data

    [SerializeField] private float force = 10;
    [SerializeField] private float speed = 5;


    private void FixedUpdate() {

        float speed = Input.GetAxis("Vertical")*maxTorque;
        float steer = Input.GetAxis("Horizontal")*maxSteerAngle;

        float leftSteer;
        float rightSteer;

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

        foreach (WheelElements element in wheelData) {

            if (element.shouldSteer == true) {
                element.leftWheel.steerAngle = leftSteer;
                element.rightWheel.steerAngle = rightSteer;
            }
            if (element.addWheelTorque == true) {
                element.leftWheel.motorTorque = speed;
                element.rightWheel.motorTorque = speed;
            }

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
    // // Start is called before the first frame update
    void Start()
    {
        foreach (WheelElements element in wheelData) {
            RotateTyresInitial(element.leftWheel);
            RotateTyresInitial(element.rightWheel);
        }

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = massCenter.localPosition;
        
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