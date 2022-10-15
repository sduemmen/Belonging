using System;
using UnityEngine;
using UnityEngine.Events;

namespace unity_ui_extensions.Scripts.Controls.ColorPicker.Events
{
    [Serializable]
    public class ColorChangedEvent : UnityEvent<Color>
    {

    }
}