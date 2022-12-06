using System;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public class KeyBind
    {
        public string m_name;

        public KeyCode m_key;

        public bool m_editable;

        public bool m_pressed;

        public bool m_wasPressed;

        public float m_repeatDelay;

        public float m_timeSinceLastPress;
    }

    public static class KeyBinds
    {
        public const string Unlock_Camera = "Unlock Camera";
        public const string Sprint = "Sprint";
        public const string Attack = "Attack";
        public const string Cancel = "Cancel";
        public const string MouseWheelPress = "MouseWheelPress";
        public const string Toggle_Inventory = "Toggle Inventory";
        public const string Toggle_Quest_Display = "Toggle Quest Display";
        public const string EquipUnequip_Axe = "Equip/Unequip Axe";
        public const string EquipUnequip_Pickaxe = "Equip/Unequip Pickaxe";
        public const string Open_Build_Menu = "Open Build Menu";
        public const string Pause_Game = "Pause Game";
        public const string dev_no_build_cost = "dev_no_build_cost";
        public const string dev_unlock_all = "dev_unlock_all";
    }

    private static InputSystem instance;

    public static InputSystem Instance {
        get {
            if (instance == null)
            {
                instance = (InputSystem)FindObjectOfType(typeof(InputSystem));
            }

            return instance;
        }
    }

    public Dictionary<string, KeyBind> m_keybindings = new Dictionary<string, KeyBind>();

    public bool m_hasUnsavedChanges;

    private const float defaultRepeatDelay = .25f;

    public static readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == SceneManager.Scenes.GameScene)
        {
            SetCursorVisibilityAndLockState(false, CursorLockMode.Locked);
        }

        LoadKeyBindings();
    }

    private void Update()
    {
        foreach (KeyBind keyBind in m_keybindings.Values)
        {
            keyBind.m_wasPressed = keyBind.m_pressed;
            keyBind.m_timeSinceLastPress += Time.deltaTime;

            if (Input.GetKey(keyBind.m_key) && keyBind.m_timeSinceLastPress >= keyBind.m_repeatDelay)
            {
                keyBind.m_pressed = true;
                keyBind.m_timeSinceLastPress = 0;
            }
            else
            {
                keyBind.m_pressed = false;
            }
        }

        if (Flags.GAME_PAUSED || Flags.UI_ELEMENT_OPEN || Flags.AXE_EQUIPPED || Flags.PICKAXE_EQUIPPED || Flags.HAMMER_EQUIPPED)
        {
            SetCursorVisibilityAndLockState(true, CursorLockMode.None);
        }
        else
        {
            SetCursorVisibilityAndLockState(false, CursorLockMode.Locked);
        }
    }

    public static void SetCursorVisibilityAndLockState(bool visible, CursorLockMode lockState)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockState;
    }

    public KeyBind GetKeyBind(string keyBindName)
    {
        return m_keybindings.ContainsKey(keyBindName) ? m_keybindings[keyBindName] : null;
    }

    private bool GetKeyDownInternal(string keyBindName)
    {
        if (m_keybindings.ContainsKey(keyBindName))
        {
            return m_keybindings[keyBindName].m_pressed;
        }

        return false;
    }

    public static bool GetKeyDown(string keyBindName)
    {
        return Instance.GetKeyDownInternal(keyBindName);
    }

    public static bool GetKeysDown(params string[] keyBinds)
    {
        foreach (string keyBind in keyBinds)
        {
            if (GetKeyDown(keyBind))
            {
                return true;
            }
        }

        return false;
    }

    public void AddKeyBind(string keyBindName, KeyCode key, float repeatDelay = defaultRepeatDelay, bool editable = true)
    {
        if (m_keybindings.ContainsKey(keyBindName))
        {
            Debug.Log($"KeyBind {keyBindName} already present in keybind definitions");
        }
        else
        {
            KeyBind keyBind = new KeyBind {
                m_name = keyBindName,
                m_key = key,
                m_editable = editable,
                m_repeatDelay = repeatDelay,
            };
            m_keybindings.Add(keyBind.m_name, keyBind);
        }
    }

    public void EditKeyBind(string keyBindName, KeyCode key)
    {
        if (m_keybindings.ContainsKey(keyBindName))
        {
            m_keybindings[keyBindName].m_key = key;
            m_hasUnsavedChanges = true;
        }
        else
        {
            Debug.Log($"KeyBind {keyBindName} not found in keybind definitions");
        }
    }

    public void ResetKeyBindings(bool calledFromGUI = false)
    {
        m_keybindings = new Dictionary<string, KeyBind>();

        AddKeyBind(KeyBinds.Unlock_Camera, KeyCode.LeftControl, 0f);
        AddKeyBind(KeyBinds.Sprint, KeyCode.LeftShift, 0f);
        AddKeyBind(KeyBinds.Attack, KeyCode.Mouse0);
        AddKeyBind(KeyBinds.Cancel, KeyCode.Mouse1);
        AddKeyBind(KeyBinds.MouseWheelPress, KeyCode.Mouse2, 0f, false);
        AddKeyBind(KeyBinds.Toggle_Inventory, KeyCode.Tab);
        AddKeyBind(KeyBinds.Toggle_Quest_Display, KeyCode.Q);
        AddKeyBind(KeyBinds.EquipUnequip_Axe, KeyCode.Alpha1);
        AddKeyBind(KeyBinds.EquipUnequip_Pickaxe, KeyCode.Alpha2);
        AddKeyBind(KeyBinds.Open_Build_Menu, KeyCode.Alpha3);
        AddKeyBind(KeyBinds.Pause_Game, KeyCode.Escape);

        AddKeyBind(KeyBinds.dev_no_build_cost, KeyCode.O);
        AddKeyBind(KeyBinds.dev_unlock_all, KeyCode.P);

        if (calledFromGUI)
        {
            m_hasUnsavedChanges = true;
        }
    }

    public void LoadKeyBindings()
    {
        ResetKeyBindings();
        foreach (KeyBind keyBind in m_keybindings.Values)
        {
            int keyCode = PlayerPrefs.GetInt(keyBind.m_name, -1);

            if (keyCode != -1)
            {
                keyBind.m_key = (KeyCode)keyCode;
            }
        }
    }

    public void SaveKeyBindings()
    {
        foreach (KeyBind keyBind in m_keybindings.Values)
        {
            PlayerPrefs.SetInt(keyBind.m_name, (int)keyBind.m_key);
        }

        m_hasUnsavedChanges = false;
    }
}