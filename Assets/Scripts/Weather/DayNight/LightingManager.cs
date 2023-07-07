using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    // References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    // Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    private float previousStaticTimeOfDay = 0;

    private int time_dampener = 20;

    private void Update() {
        //Debug.Log("Something is updating!");
        if (Preset == null) {
            return;
        }

        // Only do following if application is playing
        if (Application.isPlaying) {
            // deltaTime is interval in seconds from last frame to current one
            // This will make it change the lighting itself as time/frames go on
            TimeOfDay += Time.deltaTime / this.time_dampener;
            //TimeOfDay -= 24;
            // Divide by 24 so we can pass a 0 to 1 value
            TimeOfDay %= 24; // Clamp between 0-24
            UpdateLighting(TimeOfDay / 24f);
        } else {
            // TODO: Be wary if the previousStaticTimeOfDay may cause bugs
            if (TimeOfDay != this.previousStaticTimeOfDay) {
                // Do this in the editor when slider is moved
                // Debug.Log("Checkinggg");
                UpdateLighting(TimeOfDay / 24f);
                previousStaticTimeOfDay = TimeOfDay;
            }
        }
    }


    // Input variable ranges from 0-1
    private void UpdateLighting(float timePercent) {
        // Evaluate all different gradients in preset and set to render against
        // the time of day 0-1
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        //Debug.Log("Entered updatelighting func");
        
        if(DirectionalLight != null) {
            //Debug.Log("There is a light!");
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            // Could add vector to angle wish to rotate the sky/light

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) -90f, 170f, 0));


        }
    }


    // Called every time we change something in script or inspector
    private void OnValidate() {
        // Make sure there is a directional light
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null) {
            DirectionalLight = RenderSettings.sun;
        }
        else {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights) {
                if (light.type == LightType.Directional) {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

}
