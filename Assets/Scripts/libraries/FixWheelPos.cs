using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixWheelPos : MonoBehaviour
{
    void UpdateWheelPos(WheelCollider col, Transform t) {
        Vector3 pos = t.position;
        Quaternion rot = t.rotation;
        col.GetWorldPose(out pos, out rot);
        rot = rot * Quaternion.Euler(new Vector3(0,90,0));
        print(rot);
        t.position = pos;
        t.rotation = rot;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
