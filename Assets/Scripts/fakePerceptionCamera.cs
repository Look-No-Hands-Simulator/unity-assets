using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventPasser
{

}

public class fakePerceptionCamera : MonoBehaviour
{
    GameObject[] cones = null;
    Camera carCam;
    TrackGeneration trackGeneration;
    bool trackGen = true;

    // Start is called before the first frame update
    void Start()
    {
        // Find gameobjects in scene
        //GameObject camera = GameObject.Find("ads-dv/ads-cam");
        //Camera carCam = camera.GetComponent(Camera);
        //GameObject blue = GameObject.Find("blueCone");
        //GameObject yellow = GameObject.Find("yellowCone");
        //GameObject orange = GameObject.Find("orangeCone");
        //GameObject big = GameObject.Find("bigorangeCone");
        //GameObject adsdv = GameObject.Find("ads-dv");

        // Get camera
        carCam = GameObject.Find("ads-cam").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        // Check if track updated from trackGen script
        GameObject empty = GameObject.Find("EmptyInit");
        trackGeneration = empty.GetComponent<TrackGeneration>();

        // Check for updated track
        trackGen = trackGeneration.getTrackChange();

        if (cones != null)
        {
            fakePerception();
        }

        // Run again to find all cones if track updates
        if (trackGen == true) // trackGen is obsolete @
        {
            // This is inefficient as it creates a new array every ms @
            cones = GameObject.FindGameObjectsWithTag("Cone");
        }

    }

    bool isVisible(Renderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(carCam);

        if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void fakePerception()
    {
        string[] colours = { "orange", "blue", "yellow", "big" };
        string currentCol = "error";
        // identify cones
        foreach (var cone in cones)
        {
            if (isVisible(cone.GetComponentInChildren<Renderer>()))
            {
                // get colour
                foreach (var col in colours)
                {
                    if (cone.name.Contains(col)) {
                        currentCol = col;
                    }
                }

                Debug.Log(currentCol + " cone detected");
            }
        }

        // get distance of cones from camera

        // get position of cones (ground truth)

        // store positions

    }

    //void checkCameraFeed()
    //{
    //    // Check if cone bounding boxes are within camera frustum
    //    // or compute angle of vector to object 

    //}

    //void publishPosition()
    //{

    //}



}
