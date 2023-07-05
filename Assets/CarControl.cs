using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public List<WheelElements> wheelData;

    public float maxTorque;
    public float maxSteerAngle = 30;

    private Rigidbody rb;
    public Transform massCenter;

    private void FixedUpdate() {

        float speed = Input.GetAxis("Vertical")*maxTorque;
        float steer = Input.GetAxis("Horizontal")*maxSteerAngle;

        foreach (WheelElements element in wheelData) {

            if (element.shouldSteer == true) {
                element.leftWheel.steerAngle = steer;
                element.rightWheel.steerAngle = steer;
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