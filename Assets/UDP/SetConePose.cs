using UnityEngine;
using System.Collections;

public class SetConePose : MonoBehaviour {

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(UDPData.xFloat, UDPData.yFloat, UDPData.zFloat);

    }
}