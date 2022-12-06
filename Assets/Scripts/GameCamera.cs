using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private static GameCamera instance;

    public static GameCamera Instance {
        get {
            if (instance == null)
            {
                instance = (GameCamera)FindObjectOfType(typeof(GameCamera));
            }

            return instance;
        }
    }

    public Player m_player;
        
    public Camera m_camera;

    public CinemachineVirtualCamera m_virtualCamera;

    private Cinemachine3rdPersonFollow m_3rdPersonFollow;

    private float m_cameraDistance = 10f;

    public float m_cameraDistanceAdjustmentSpeed = 15f;

    public float m_minXRotation = 80f;

    public float m_maxXRotation = 359f;

    public float m_lookSensitivity = 5f;

    public bool m_invertX;

    public bool m_invertY;

    public Vector3 m_lookRotation;

    private void Awake()
    {
        m_player = Player.Instance;
        m_camera = GetComponent<Camera>();
        
        CinemachineComponentBase componentBase = m_virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine3rdPersonFollow thirdPersonFollow)
        {
            m_3rdPersonFollow = thirdPersonFollow;
            m_3rdPersonFollow.CameraDistance = m_cameraDistance;
        }
    }

    private void LateUpdate()
    {
        UpdateCamera(Time.unscaledDeltaTime);
    }

    private void UpdateCamera(float dt)
    {
        if (Flags.UI_ELEMENT_OPEN || Flags.GAME_PAUSED || (Flags.SLOT_EQUIPPED && !InputSystem.GetKeyDown(InputSystem.KeyBinds.MouseWheelPress)))
        {
            return;
        }

        m_lookRotation.y = Input.GetAxis("Mouse X") * m_lookSensitivity * -1;
        m_lookRotation.x = Input.GetAxis("Mouse Y") * m_lookSensitivity;

        if (m_invertX)
        {
            m_lookRotation.x *= -1;
        }
        if (m_invertY)
        {
            m_lookRotation.y *= -1;
        }
        
        Vector3 newRotation = m_player.m_playerShoulder.eulerAngles - m_lookRotation;
        if (newRotation.x > m_minXRotation && newRotation.x < 180)
        {
            newRotation.x = m_minXRotation;
        }
        else if (newRotation.x < m_maxXRotation && newRotation.x >= 180)
        {
            newRotation.x = m_maxXRotation;
        }
        m_player.m_playerShoulder.eulerAngles = newRotation;
        
        if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Unlock_Camera) && newRotation.x > 0 && newRotation.x <= 80)
        {
            m_cameraDistance = Mathf.Max(1.5f, 3 * Mathf.Sqrt(newRotation.x));
            m_3rdPersonFollow.CameraDistance = Mathf.Lerp(m_3rdPersonFollow.CameraDistance, m_cameraDistance, dt * m_cameraDistanceAdjustmentSpeed);
        }
    }
}