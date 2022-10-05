using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuCameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 _animationOffset = new Vector3(1, 0, 1);
        [SerializeField] private float _animationSpeed;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Update()
        {
            _transform.position += _animationOffset * (Time.deltaTime * _animationSpeed);
        }
    }
}
