using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public GameObject m_particles;
    
    private Light m_light;

    public Vector3 m_originalPosition;

    public bool m_enabled = false;

    
    private void Awake()
    {
        m_particles.SetActive(false);
        m_light = GetComponent<Light>();
        m_originalPosition = transform.position;
    }

    private void Update()
    {
        if (m_enabled)
        {
            m_particles.SetActive(true);
            
            float t = Time.time * 2.5f;
            float y0 = 2.3f * Mathf.Sin(3.3f * t) + 7.3f * Mathf.Sin(1.2f * t) + 2.71f * Mathf.Sin(-1.7f * t);
            float y1 = y0 / 12;
            float y2 = Mathf.Abs(y1);

            m_light.intensity = 1.5f + y2;
            transform.position = m_originalPosition + new Vector3(y1, y1, y1) / 100;
        }
    }
}