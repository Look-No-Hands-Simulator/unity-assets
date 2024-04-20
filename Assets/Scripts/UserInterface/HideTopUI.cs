using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTopUI : MonoBehaviour
{
    public GameObject[] objs;

    public void HideAllUI()
    {
        // Check if any UI element is active
        bool anyActive = false;
        foreach (GameObject uiClicker in objs)
        {
            if (uiClicker.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        // Toggle the visibility of all UI elements based on the state of the first one
        foreach (GameObject uiClicker in objs)
        {
            uiClicker.SetActive(!anyActive);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        objs = GameObject.FindGameObjectsWithTag("UITopLayer");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HideAllUI();
        }
    }
}