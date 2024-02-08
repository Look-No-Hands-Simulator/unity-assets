using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcherViewer : MonoBehaviour
{
    public GameObject highCam;
    public GameObject topCam;
    public GameObject frontCam;
    public GameObject backCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            highCam.SetActive(true);
            topCam.SetActive(false);
            frontCam.SetActive(false);
            backCam.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            highCam.SetActive(false);
            topCam.SetActive(true);
            frontCam.SetActive(false);
            backCam.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            highCam.SetActive(false);
            topCam.SetActive(false);
            frontCam.SetActive(true);
            backCam.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            highCam.SetActive(false);
            topCam.SetActive(false);
            frontCam.SetActive(false);
            backCam.SetActive(true);
        }
        
    }

    public void SetHighCamActive() {
        highCam.SetActive(true);
        topCam.SetActive(false);
        frontCam.SetActive(false);
        backCam.SetActive(false);
    }
}
