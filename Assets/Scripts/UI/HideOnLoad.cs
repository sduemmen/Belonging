using UnityEngine;

namespace UI
{
    public class HideOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}