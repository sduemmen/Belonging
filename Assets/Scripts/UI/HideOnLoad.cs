using UnityEngine;

namespace UI
{
    public class HideOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            this.gameObject.SetActive(false);
        }
    }
}
