using System.Collections.Generic;
using System.Linq;
using BuildSystem;
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

        public static bool GetMouseRayHit(LayerMask layerMask, float distance, out RaycastHit raycastHit)
        {
            Ray ray = GameCamera.Instance.m_camera.ViewportPointToRay(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0));
            return Physics.Raycast(ray, out raycastHit, distance, layerMask);
        }

        public static bool SegmentRayCast(LayerMask layerMask, float distance, out RaycastHit hit, out Vector3 point, out Vector3 normal, out Segment segment)
        {
            bool objectHit = GetMouseRayHit(layerMask, distance, out hit);

            if (objectHit)
            {
                point = hit.point;
                normal = hit.normal;
                segment = hit.transform.GetComponent<Segment>();
            }
            else
            {
                point = Vector3.zero;
                normal = Vector3.zero;
                segment = null;
            }

            return objectHit;
        }
    }
}