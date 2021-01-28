using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Threading;

/*
0. get camera
1. take picture every 0.2 seconds
2. compress picture to small size
3. upload picture to local folder
*/

public class CameraCaller : MonoBehaviour {
    public int resWidth = 60;
    public int resHeight = 20;
    public bool active = false;

    public void activateCam() {
        if (active == false) {
            active = true;
            while (active == true) {
                camLooper();
            }
        }
        else {
            active = false;
            stopCam();
        }
    }

    public static void camLooper() {

    }

    public static void stopCam() {

    }

    public void takeShot() {
        RenderTexture texture = new RenderTexture(this.resWidth,this.resHeight,24);
        StaticHold.carCam.targetTexture = texture;
        Texture2D screenShot = new Texture2D(this.resWidth,this.resHeight,
        TextureFormat.RGB24,false);
        StaticHold.carCam.Render();
        RenderTexture.active = texture;
        screenShot.ReadPixels(new Rect(0,0,this.resWidth,this.resHeight),0,0);
        StaticHold.carCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(texture);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = nameShot(this.resWidth,this.resHeight);
        System.IO.File.WriteAllBytes(filename,bytes);
        Debug.Log(string.Format("Screenshot in {0}",filename));

    }

    public string nameShot(int width, int height) {
        return string.Format("{0}/camera/shots/screen_{1}x{2}_{3}.png",Application.dataPath,width,height,System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    } 

    public static void saveShot() {

    }


}

public class StaticHold {
    public static CameraCaller camScript;
    public static Camera carCam;
}

public class cameraFeed : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StaticHold.carCam = GameObject.Find("ads-cam").GetComponent<Camera>();
        StaticHold.camScript = new CameraCaller(camScript,carCam);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) {
            StaticHold.camScript.activateCam();
        }
        
    }
}
