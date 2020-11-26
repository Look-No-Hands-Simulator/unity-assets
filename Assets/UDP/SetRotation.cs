using UnityEngine;
using System.Collections;

public class SetRotation : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.eulerAngles = new Vector3(UDPData.pFloat, UDPData.qFloat, UDPData.rFloat);

	}
}