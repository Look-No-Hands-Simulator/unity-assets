using UnityEngine;
using System.Collections;

public class SetTranslation : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
		//Set attached vehicle's position
		transform.position = new Vector3(UDPData.xFloat, UDPData.yFloat, UDPData.zFloat);

		//Set attached vehicle's rotation
		transform.rotation = new Quaternion(UDPData.pFloat, UDPData.qFloat, UDPData.rFloat, UDPData.wFloat);
		
		//Inverse rotation (TODO: tweak ads model rotation to no longer require this)
		transform.rotation = Quaternion.Inverse(transform.rotation);


	}
}
