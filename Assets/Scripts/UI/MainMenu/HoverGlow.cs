using UnityEngine;

namespace UI.MainMenu
{
    public class HoverGlow : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            Disable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}