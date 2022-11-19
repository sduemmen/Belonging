using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private static GameCamera _instance;

    public static GameCamera Instance {
        get {
            if (_instance == null)
            {
                _instance = (GameCamera)FindObjectOfType(typeof(GameCamera));
            }

            return _instance;
        }
    }

    public Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }
}