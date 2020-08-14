using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[ExecuteInEditMode]
public class LightController : MonoBehaviour
{
    [SerializeField]
    private Light directionalLight;

    [SerializeField]
    private LightingPreset preset;

    [SerializeField, Range(0, 24)]
    private float timeOfDay;

    private void OnValidate()
    {
        if (directionalLight != null)
        {
            return;
        }
        if (RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                }
            }
        }
    }

    private void UpdateLighting(float timeOfDay)
    {
        float timePercent = timeOfDay / 24f;
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90, 170f, 0));
        }
    }


    /// <summary>
    /// Sets the time of the day which will determine the sun position.
    /// </summary>
    /// <param name="timeOfDay">Time should be in 24 hour format and minutes should be fraction after the hour i.e. time = hour + minutes/60 </param>
    public void SetTimeOfDay(float timeOfDay)
    {
        this.timeOfDay = timeOfDay;
    }

    // Update is called once per frame
    void Update()
    {
        if (preset == null)
        {
            Debug.LogError("Lighting preset is null");
            return;
        }

        if (Application.isPlaying)
        {
            timeOfDay = EnvironmentController.Instance.GetCurrentTime();
            //Debug.Log("Current time: " + timeOfDay + " Runtime: " + Time.time);
            UpdateLighting(timeOfDay);
        }
    }
}
