using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButtons : MonoBehaviour
{
    public void DisableBtns()
    {
        
        for (int i = 0; i < transform.childCount; i++)
        {
            // Make sure to hide the buttons as well as disable
            transform.GetChild(i).gameObject.SetActive(false);

        }


    }
}