using UnityEngine;
using System.Collections;

public class SetConePose : MonoBehaviour
{
    public bool ran = false;

    void Update()
    {
        //Only run if has not run before and once cone position UDP thread has run
        if (UDPData.coneInst == false && UDPData.readyToRun == true){
            
            //Loop through blue cone arrays and instantiate
            for (int i = 0; i < UDPData.blueCount; i++)
            {
                //Load conemodel from resources folder & set position
                Instantiate(Resources.Load<GameObject>("cones/blueCone"),
                    new Vector3(UDPData.blueX[i], UDPData.blueY[i], UDPData.blueZ[i]), Quaternion.identity);
                
            }
            
            //Loop through yellow cone arrays and instantiate
            for (int i = 0; i < UDPData.yellowCount; i++)
            {
                //Load cone model from resources folder & set position
                Instantiate(Resources.Load<GameObject>("cones/yellowCone"),
                    new Vector3(UDPData.yellowX[i], UDPData.yellowY[i], UDPData.yellowZ[i]), Quaternion.identity);

                
            }
            
            //Set bool to true once cones have been spawned (redundant)
            UDPData.coneInst = true;
            
            //Disable Update() once cones have spawned
            enabled = false; 
            
        }
    }
}