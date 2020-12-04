using UnityEngine;
using System.Collections;

public class SetConePose : MonoBehaviour
{
    public bool ran = false;

    void Update()
    {
        if (UDPData.coneInst == false && UDPData.readyToRun == true){
            for (int i = 0; i < UDPData.blueCount; i++)
            {
                Instantiate(Resources.Load<GameObject>("cones/blueCone"),
                    new Vector3(UDPData.blueX[i], UDPData.blueY[i], UDPData.blueZ[i]), Quaternion.identity);
                //Debug.Log("Conecoords" + UDPData.blueX.Length);
                Debug.Log("Ran blue");

            }

            for (int i = 0; i < UDPData.yellowCount; i++)
            {
                Instantiate(Resources.Load<GameObject>("cones/yellowCone"),
                    new Vector3(UDPData.yellowX[i], UDPData.yellowY[i], UDPData.yellowZ[i]), Quaternion.identity);
                if (i > 1)
                {
                    Debug.Log("Stopped");
                }



            }

            UDPData.coneInst = true;
            enabled = false;
        }
    }
}