using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditKeyBind : MonoBehaviour
{
    public string m_keyBindName;
    
    public TextMeshProUGUI m_keyBindLabel;
    
    public Button m_editKeyBindButton;
    
    public KeyCode m_key;

    public bool m_editing;

    public float m_maxEditTime = 6f;


    private void Awake()
    {
        m_editKeyBindButton.onClick.AddListener(() => {
            m_editing = true;
            m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Listening...";
            m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Italic;
        });
    }

    private void OnDisable()
    {
        m_editing = false;
        m_keyBindLabel.text = m_key.ToString();
        m_keyBindLabel.fontStyle = FontStyles.Normal;
    }

    private void Update()
    {
        if (m_editing)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in InputSystem.keyCodes)
                {
                    if (Input.GetKey(keyCode)) {
                        InputSystem.Instance.EditKeyBind(m_keyBindName, keyCode);
                        m_key = keyCode;
                        m_maxEditTime = 6f;
                        m_editing = false;
                        m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_key.ToString();
                        m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                    }
                }
            }
            
            m_maxEditTime -= Time.deltaTime;
            
            if (m_maxEditTime <= 0)
            {
                m_maxEditTime = 6f;
                m_editing = false;
                m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_key.ToString();
                m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            }
        }
    }

    public void Initialize(InputSystem.KeyBind keyBind)
    {
        m_keyBindName = keyBind.m_name;
        m_key = keyBind.m_key;
        m_keyBindLabel.text = keyBind.m_name;
        m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_key.ToString();
        m_editKeyBindButton.transform.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
    }
}