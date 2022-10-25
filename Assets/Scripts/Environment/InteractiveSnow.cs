using UnityEngine;

namespace Environment
{
    public class InteractiveSnow : MonoBehaviour
    {
        [SerializeField] private RenderTexture rt;
        [SerializeField] private Transform target;
        
        private void Awake()
        {
            Shader.SetGlobalTexture("_GlobalEffectRT", rt);
            Shader.SetGlobalFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);
        }
 
        private void Update()
        {
            var t = transform;
            var position = t.position;
            var targetPosition = target.transform.position;
            position = new Vector3(targetPosition.x, position.y, targetPosition.z);
            t.position = position;
            Shader.SetGlobalVector("_Position", position);
        }
    }
}
