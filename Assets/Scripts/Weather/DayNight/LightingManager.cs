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

    private void Update() {
        if (Preset == null) {
            return;

            // Only do following if application is playing
            if (Application.isPlaying) {
                // deltaTime is interval in seconds from last frame to current one
                // This will make it change the lighting itself as time/frames go on
                TimeOfDay += Time.deltaTime;
                // Divide by 24 so we can pass a 0 to 1 value
                TimeOfDay %= 24; // Clamp between 0-24
                UpdateLighting(TimeOfDay / 24f);
            } else {
                // Do this in the editor when slider is moved
                UpdateLighting(TimeOfDay / 24f);
            }
        }
    }


    // Input variable ranges from 0-1
    private void UpdateLighting(float timePercent) {
        // Evaluate all different gradients in preset and set to render against
        // the time of day 0-1
        RenderSettings.ambientLight = Preset.AmbientColour.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColour.Evaluate(timePercent);
        
        if(DirectionalLight != null) {
            DirectionalLight.color = Preset.DirectionalColour.Evaluate(timePercent);
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