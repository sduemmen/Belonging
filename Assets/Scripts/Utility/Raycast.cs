using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Utility
{
    public class Raycast
    {
        public static bool MouseOverUI()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
                position = Mouse.current.position.ReadValue()
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Where(result => result.gameObject.layer == LayerMask.NameToLayer("UI")).ToArray().Length > 0;
        }

        public static bool GetMouseRayHit(Camera camera, LayerMask layerMask, out RaycastHit raycastHit, float distance)
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }
    }
}