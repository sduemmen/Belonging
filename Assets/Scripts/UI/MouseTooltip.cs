using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseTooltip : MonoBehaviour
    {
        private static MouseTooltip _instance;
        private bool visible;

        public static MouseTooltip Instance {
            get {
                if (_instance == null)
                {
                    _instance = (MouseTooltip)FindObjectOfType(typeof(MouseTooltip));
                }
                
                return _instance;
            }
        }

        private void Update()
        {
            if (!visible) return;

            transform.position = Mouse.current.position.ReadValue();
        }

        public void Show(GameObject tooltip)
        {
            Transform t = transform;
            t.position = Mouse.current.position.ReadValue();
            tooltip.transform.SetParent(t, false);
            visible = true;
        }

        public void Hide()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            visible = false;
        }
    }
}