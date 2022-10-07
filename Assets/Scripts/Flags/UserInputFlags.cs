using UnityEngine;
using UnityEngine.InputSystem;

namespace Flags
{
    public static class UserInputFlags
    {
        public static bool OPEN_INVENTORY_KEY_WAS_PRESSED => Keyboard.current.tabKey.wasPressedThisFrame;
        public static bool CLOSE_INVENTORY_KEY_WAS_PRESSED => Keyboard.current.tabKey.wasPressedThisFrame;
        public static bool SUPPRESS_CAMERA_ROTATION_KEY_PRESSED => Keyboard.current.leftShiftKey.isPressed;
        public static bool SUPPRESS_CAMERA_ROTATION_KEY_RELEASED => Keyboard.current.leftShiftKey.wasReleasedThisFrame;
        public static bool OPEN_PAUSE_MENU_KEY_WAS_PRESSED => Keyboard.current.escapeKey.wasPressedThisFrame;
        public static bool CLOSE_PAUSE_MENU_KEY_WAS_PRESSED => Keyboard.current.escapeKey.wasPressedThisFrame;
        public static bool SELECT_SLOT1_KEY_WAS_PRESSED => Keyboard.current.digit1Key.wasPressedThisFrame;
        public static bool SELECT_SLOT2_KEY_WAS_PRESSED => Keyboard.current.digit2Key.wasPressedThisFrame;
        public static bool SELECT_SLOT3_KEY_WAS_PRESSED => Keyboard.current.digit3Key.wasPressedThisFrame;

        public static bool LEFT_MOUSE_BUTTON_WAS_PRESSED => Input.GetKeyDown(KeyCode.Mouse0);
        public static bool RIGHT_MOUSE_BUTTON_WAS_PRESSED => Input.GetKeyDown(KeyCode.Mouse1);
    }
}