using UnityEngine;

namespace UI.MainMenu
{
    public class HoverGlow : MonoBehaviour
    {
        private void Awake()
        {
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