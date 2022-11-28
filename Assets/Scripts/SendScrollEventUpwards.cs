using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SendScrollEventUpwards : MonoBehaviour, IScrollHandler
{
    public ScrollRect m_targetScrollRect;

    private void Start()
    {
        m_targetScrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnScroll(PointerEventData eventData)
    {
        m_targetScrollRect.OnScroll(eventData);
    }
}