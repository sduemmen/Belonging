using TMPro;
using UnityEngine;

namespace Utility
{
    public class FloatFloatText : MonoBehaviour
    {
        public TextMeshProUGUI label;

        public void SetText(float a, float b)
        {
            label.text = a + "/" + b;
        }
    }
}