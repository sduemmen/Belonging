using UnityEngine;

namespace Environment
{
    public class InteractiveSnow : MonoBehaviour
    {
        [SerializeField]
        RenderTexture rt;
        [SerializeField]
        Transform target;
        // Start is called before the first frame update
        void Awake()
        {
            Shader.SetGlobalTexture("_GlobalEffectRT", rt);
            Shader.SetGlobalFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);
        }
 
        private void Update()
        {
            transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            Shader.SetGlobalVector("_Position", transform.position);
        }
 
 
    }
}
