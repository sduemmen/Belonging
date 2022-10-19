using UnityEngine;

namespace Flags
{
    public static class UserInputFlags
    {
        public static bool LEFT_MOUSE_BUTTON_WAS_PRESSED => Input.GetKeyDown(KeyCode.Mouse0);
    }
}