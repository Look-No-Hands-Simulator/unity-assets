using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayAndNight : MonoBehaviour
{

    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;


    Vector3 rot = Vector3.zero;
    float degpersec = 6;

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
