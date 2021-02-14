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
    GameObject camObj;
    TrackGeneration trackGeneration;
    bool trackGen = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get camera
        camObj = GameObject.Find("ads-cam");
        carCam = GameObject.Find("ads-cam").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        // Check if track updated from trackGen script
        //GameObject empty = GameObject.Find("EmptyInit");
        //trackGeneration = empty.GetComponent<TrackGeneration>();

        // Check for updated track
        //trackGen = trackGeneration.getTrackChange();

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
                // get distance of cone from camera
                // get object heading vector
                var coneHeading = cone.transform.position - carCam.transform.position;
                // get dot product of heading with camera's forward vector
                var distance = Vector3.Dot(coneHeading, carCam.transform.forward);

                // get position of cone (ground truth)
                var conePos = cone.transform.position;

                // get angle with euler angles
                // https://forum.unity.com/threads/angle-between-camera-and-object.97028/
                var angle = Mathf.Atan2(cone.transform.position.z - carCam.transform.position.z, 
                    cone.transform.position.x - carCam.transform.position.x)*Mathf.Rad2Deg;

                // store cone information
                // problem is this will keep growing infinitely when you want to post values to ros node
                // as they come along so use a single array
                string[] globalConeInfo = { currentCol, cone.transform.position.x.ToString(), cone.transform.position.z.ToString(), 
                    distance.ToString(), angle.ToString() };

                //foreach (var item in globalConeInfo)
                //{
                //    Debug.Log(item);
                //}

                Vector3 relativePose = (carCam.transform).InverseTransformPoint(cone.transform.position);

                string[] relativeConeInfo = { currentCol, relativePose.x.ToString(), relativePose.z.ToString(),
                distance.ToString(), angle.ToString() };

                Debug.Log(relativePose.x);

                // Debug.Log(coneInfo);
                // Debug.Log("Distance" + distance);
                // Debug.Log(currentCol + " cone detected");
            }
        }

    }

    void storeInfo()
    {

    }

    //void checkCameraFeed()
    //{
    //    // Check if cone bounding boxes are within camera frustum
    //    // or compute angle of vector to object 

    //}

    //void publishPositionRos()
    //{

    //}



}
