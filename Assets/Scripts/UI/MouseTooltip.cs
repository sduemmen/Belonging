using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseTooltip : MonoBehaviour
    {
        private bool visible;
        
        public void Show(GameObject tooltipContent)
        {
            tooltipContent.transform.SetParent(this.transform, false);
            visible = true;
        }

        public void Hide()
        {
            if (transform.childCount == 0) return;
            
            Destroy(transform.GetChild(0).gameObject);
            visible = false;
        }

        private void Update()
        {
            if (visible) transform.position = Mouse.current.position.ReadValue();
        }
    }
}