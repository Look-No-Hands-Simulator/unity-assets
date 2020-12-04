using UnityEngine;
using System.Collections;

public class SetTranslation : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		print("updated car pos" + UDPData.xFloat);
		transform.position = new Vector3(UDPData.xFloat, UDPData.yFloat, UDPData.zFloat);

	}
}
