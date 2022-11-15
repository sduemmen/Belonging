using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.MouseHover
{
    public enum TooltipAlignment    // relative from cursor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
    
    [Serializable, CreateAssetMenu(menuName = "Behaviours/UI/Hover/Tooltip")]
    public class TooltipHoverBehaviour : ScriptableObject, IMouseHoverBehaviour
    {
        private static int _spaceBetweenCursor = 5;
        
        [SerializeField] private Tooltip _tooltipPrefab;
        [SerializeField, Tooltip("The time until the tooltip is displayed in seconds")] private float _timeUntilDisplayed;
        [SerializeField] private bool _useCustomOffset;
        [SerializeField, ShowIf("_useCustomOffset")] private Vector3 _tooltipOffset;
        [SerializeField, ShowIf("_useCustomOffset")] private Vector3 _tooltipRotation;
        [SerializeField, HideIf("_useCustomOffset")] private TooltipAlignment _alignment;
        
        public void OnHoverEnter(Hoverable hoverable)
        {
            hoverable.ActiveTooltip = hoverable.StartCoroutine(OnHoverEnterDelayed(hoverable));
        }

        public void OnHoverLeave(Hoverable hoverable)
        {
            hoverable.StopCoroutine(hoverable.ActiveTooltip);
            MouseTooltip.Instance.Hide();
        }

        private IEnumerator OnHoverEnterDelayed(Hoverable hoverable)
        {
            yield return new WaitForSecondsRealtime(_timeUntilDisplayed);
            
            Tooltip tooltipInstance = Instantiate(_tooltipPrefab, Vector3.up * 10000, Quaternion.identity);
            hoverable.OnTooltipVisible(tooltipInstance);
            
            Vector3 offset = _useCustomOffset ? _tooltipOffset : GetOffsetFromAlignment(_alignment, tooltipInstance.gameObject);
            Vector3 rotation = _useCustomOffset ? _tooltipRotation : Vector3.zero;
            
            tooltipInstance.transform.position = offset;
            tooltipInstance.transform.rotation = Quaternion.Euler(rotation);

            MouseTooltip.Instance.Show(tooltipInstance.gameObject);
        }

        private static Vector3 GetOffsetFromAlignment(TooltipAlignment alignment, GameObject tooltip)
        {
            Rect rect = ((RectTransform)tooltip.transform).rect;
            float widthOffset = rect.width / 2;
            float heightOffset = rect.height / 2;

            switch (alignment)
            {
                case TooltipAlignment.TopLeft:
                    return new Vector3(-(widthOffset + _spaceBetweenCursor), heightOffset + _spaceBetweenCursor, 0);
                case TooltipAlignment.TopRight:
                    return new Vector3(widthOffset + _spaceBetweenCursor, heightOffset + _spaceBetweenCursor, 0);
                case TooltipAlignment.BottomLeft:
                    return new Vector3(-(widthOffset + _spaceBetweenCursor), -(heightOffset + _spaceBetweenCursor), 0);
                case TooltipAlignment.BottomRight:
                    return new Vector3(widthOffset + _spaceBetweenCursor, -(heightOffset + _spaceBetweenCursor), 0);
                default:
                    return Vector3.zero;
            }
        }
    }
}