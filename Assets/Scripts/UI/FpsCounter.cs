using TMPro;
using UnityEngine;

namespace UI
{
    public class FpsCounter : MonoBehaviour
    {
        public TextMeshProUGUI fpsLabel;
        public float deltaTime;
 
        void Update () {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsLabel.text = Mathf.Ceil (fps).ToString();
        }
    }
}
