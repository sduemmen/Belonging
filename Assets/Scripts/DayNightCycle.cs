using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light _sun;
    [SerializeField] private Light _moon;
    private Transform t_sun;
    private Transform t_moon;
    
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _latitudeGimbal;
    [SerializeField] private Transform _orbitGimbal;

    [SerializeField, Range(0.01f, 1f)] private float _secondsPerMinute = 0.625f;
    [SerializeField] private float _startTime = 12.0f;
    [SerializeField] private float _latitudeAngle = 45.0f;

    [SerializeField] private PostProcessVolume _volume;
    private Bloom _bloom;

    private Material _domeMaterial;
    private float _minute;
    private float _day;

    private const float TAU = 6.28318530718f;
    private const float PI = 3.14159265359f;

    private void Awake()
    {
        _domeMaterial = GetComponent<Renderer>().sharedMaterial;
        _latitudeGimbal.eulerAngles = new Vector3(Mathf.Clamp(_latitudeAngle, 0, 90), 0, 0);
        _volume.profile.TryGetSettings(out _bloom);

        t_sun = _orbitGimbal.GetChild(0);
        t_moon = _orbitGimbal.GetChild(1);
    }

    private void OnDrawGizmos()
    {
        if (t_sun != null && t_moon != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Vector3.zero, (transform.position - t_sun.position).normalized);
        
            Gizmos.color = Color.black;
            Gizmos.DrawRay(Vector3.zero, (transform.position - t_moon.position).normalized);
        }
    }
    
    private void Update() 
    {
        UpdateTimeOfDay();
        UpdateDomePosition();
    }

    private void UpdateDomePosition()
    {
        var position = _player.position;
        transform.position = new Vector3(position.x, -2, position.z);
    }
    
    private void UpdateTimeOfDay()
    {
        float smoothMinute = Time.time / _secondsPerMinute + _startTime * 60;
        _day = Mathf.Floor(smoothMinute / 1440) + 1;

        smoothMinute -= Mathf.Floor(smoothMinute / 1440) * 1440;
        _minute = Mathf.Round(smoothMinute);

        _orbitGimbal.localRotation = Quaternion.Euler(new Vector3(0, smoothMinute / 4, 0));
        float texOffset = Mathf.Cos(smoothMinute / 1440 * TAU) * 0.25f + 0.25f;
        Vector2 mainTextureOffset = new Vector2(Mathf.Round((texOffset - Mathf.Floor(texOffset / 360) * 360) * 1000) / 1000, 0);
        _domeMaterial.mainTextureOffset = mainTextureOffset;
        
        float xOffset = mainTextureOffset.x + .5f;
        Color domeAmbientLight = new Color(0, 0, 0, 1);

        int samples = (int)(Mathf.Clamp01(Mathf.Cos(mainTextureOffset.x * 2 * TAU) + 1.5f) * 100);
        for (int y = 0; y < samples; y++)
        {
            float yOffset = (float)y / 100;
            domeAmbientLight += ((Texture2D)_domeMaterial.mainTexture).GetPixelBilinear(xOffset, yOffset);
        }
        
        domeAmbientLight /= samples;

        Color lightColor = (_sun.color * _sun.intensity + _moon.color * _moon.intensity) / 2;
        Color ambientLight = (domeAmbientLight * .5f + lightColor * .5f);
        ambientLight.a = 1;
        
        float sunCycleModifier = Mathf.Clamp01(Mathf.Cos(_domeMaterial.mainTextureOffset.x * TAU));
        _sun.transform.rotation = _orbitGimbal.GetChild(1).rotation;
        _sun.shadowStrength = sunCycleModifier;
        _sun.intensity = sunCycleModifier;
        _sun.color = new Color(1, .22f * Mathf.Cos(mainTextureOffset.x * 2 * TAU) + .7f, .35f * Mathf.Cos(mainTextureOffset.x * 2 * TAU) + .55f, 1);
        // .2\cos\left(4\pi x\right)+.8
        // .5\cos\left(4\pi x\right)+.5
        
        float moonCycleModifier = Mathf.Clamp01(Mathf.Cos(_domeMaterial.mainTextureOffset.x * TAU - PI));
        _moon.transform.rotation = _orbitGimbal.GetChild(0).rotation;
        _moon.shadowStrength = moonCycleModifier * .5f; 
        _moon.intensity = moonCycleModifier;

        RenderSettings.ambientLight = ambientLight;
        RenderSettings.fogColor = _sun.color * sunCycleModifier;

        _bloom.intensity.value = 1f + 5 * moonCycleModifier;

        Vector3 pos = transform.position;
        Shader.SetGlobalVector("_InverseSunDirection", -(pos - t_sun.position).normalized);
        Shader.SetGlobalVector("_InverseMoonDirection", -(pos - t_moon.position).normalized);
    }
}