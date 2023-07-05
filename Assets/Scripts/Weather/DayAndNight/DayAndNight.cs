using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class DayAndNight : MonoBehaviour
{

    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;


    Vector3 rot = Vector3.zero;
    float degpersec = 6;

    

    // Update is called once per frame
        private void Update() {
        if(Preset == null) {
            return;

            if (Application.isPlaying) {
                // deltaTime is interval in seconds from last frame to current one
                // Time of day + 
                TimeOfDay += Time.deltaTime;
                // Modulus
                TimeOfDay %= 24; // Clamp between 0-24
                //UpdateLighting(TimeOfDay / 24f);
            } else {
                //UpdateLighting(TimeOfDay / 24f);
            }
        }
    }
}
