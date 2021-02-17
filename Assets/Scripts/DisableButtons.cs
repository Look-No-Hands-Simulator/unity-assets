using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButtons : MonoBehaviour
{
    public void DisableBtns()
    {
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);

        }


    }
}