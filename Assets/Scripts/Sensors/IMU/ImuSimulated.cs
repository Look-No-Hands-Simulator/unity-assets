//using UnityEngine;

//using UnityEngine;

//public class CarAccelerometerSimulation : MonoBehaviour
// {
//     // Reference to the Rigidbody component
//     private Rigidbody rb;

//     // Update is called once per frame
//     void Start()
//     {
//         // Get the Rigidbody component attached to the GameObject
//         rb = GetComponent<Rigidbody>();
        
//         // Make sure the Rigidbody has constraints to prevent unwanted rotations
//         rb.freezeRotation = true;
//     }

//     void Update()
//     {
//         // Get the linear acceleration from the Rigidbody
//         Vector3 linearAcceleration = rb.acceleration;

//         // Apply acceleration to the GameObject
//         ApplyAcceleration(linearAcceleration);
//     }

//     void ApplyAcceleration(Vector3 acceleration)
//     {
//         // Do something with the acceleration data
//         // For example, you can print the values or use them for other calculations
//         Debug.Log("Acceleration X: " + acceleration.x);
//         Debug.Log("Acceleration Y: " + acceleration.y);
//         Debug.Log("Acceleration Z: " + acceleration.z);

//         // You can also use the acceleration to control other aspects of the car
//         // For instance, you might want to adjust the car's speed or apply steering forces
//     }
// }
