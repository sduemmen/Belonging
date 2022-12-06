using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsGUI : MonoBehaviour
{
    public EditKeyBind m_editKeyBindPrefab;

    public Button m_saveSettingsButton;

    public Button m_resetToDefaultButton;

    public Button m_openSettingsGUIButton;

    public Button m_backButton;

    public GameObject m_unsavedChangesHint;

    public UnityEvent m_onBack;

    private void Start()
    {
        foreach (InputSystem.KeyBind keyBind in InputSystem.Instance.m_keybindings.Values)
        {
            if (!keyBind.m_editable)
            {
                continue;
            }
            
            EditKeyBind editKeyBind = Instantiate(m_editKeyBindPrefab, this.transform, false);
            editKeyBind.Initialize(keyBind);
        }
        
        m_saveSettingsButton.onClick.AddListener(() => 
        {
            InputSystem.Instance.SaveKeyBindings();
        });

        m_resetToDefaultButton.onClick.AddListener(() => 
        {
            InputSystem.Instance.ResetKeyBindings(true);
            
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out EditKeyBind editKeyBind))
                {
                    editKeyBind.Initialize(InputSystem.Instance.GetKeyBind(editKeyBind.m_keyBindName));
                }
            }
        });
        
        m_openSettingsGUIButton.onClick.AddListener(() => 
        {
            InputSystem.Instance.LoadKeyBindings();
            
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out EditKeyBind editKeyBind))
                {
                    editKeyBind.Initialize(InputSystem.Instance.GetKeyBind(editKeyBind.m_keyBindName));
                }
            }
        });
        
        m_backButton.onClick.AddListener(() => 
        {
            if (InputSystem.Instance.m_hasUnsavedChanges)
            {
                m_unsavedChangesHint.SetActive(true);
            }
            else
            {
                m_onBack?.Invoke();
            }
        });
    }
}