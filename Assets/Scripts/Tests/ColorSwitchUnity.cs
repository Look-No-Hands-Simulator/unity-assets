using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwitchUnity : MonoBehaviour
{

    public GameObject cube;
    
    void ColorChange()
    {
        cube.GetComponent<Renderer>().material.color = new Color32(0,204,102,100);
}
    
    // Start is called before the first frame update
    void Start()
    {
    
    	ColorChange();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
