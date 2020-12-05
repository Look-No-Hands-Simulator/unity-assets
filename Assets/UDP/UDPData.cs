using UnityEngine;
using System.Collections;

public class UDPData : MonoBehaviour
{
    
    //Bools keeping track of status of cone instantiation
    public static  bool coneInst = false;
    public static  bool readyToRun = false;
    
    //Vehicle position and rotation values obtained from Gazebo
    public static float xFloat;
    public static float yFloat;
    public static float zFloat;
    public static float pFloat;
    public static float qFloat;
    public static float rFloat;
    public static float wFloat;

    //Amount of blue and yellow cones in the current Gazebo track
    public static int blueCount;
    public static int yellowCount;

    //Blue cones x,y,z
    public static float[] blueX;
    public static float[] blueY;
    public static float[] blueZ;

    //Yellow Cones x,y,z
    public static float[] yellowX;
    public static float[] yellowY;
    public static float[] yellowZ;
    
}
