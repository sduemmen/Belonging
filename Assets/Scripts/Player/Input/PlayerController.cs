using UnityEngine;
using Utility;

namespace Player.Input
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _rotationDamping;

        [SerializeField] private Transform _cameraTarget;
        private CharacterController _characterController;
        private Transform _transform;
        private float _currentLookDirectionAngle;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            // align players rotation by taking into account current camera rotation and movement input
            Vector2 movementInput = InputController.MovementInput;
            bool playerIsMoving = movementInput != Vector2.zero;
            float lookdirection;

            if (playerIsMoving)
            {
                movementInput = MathUtilities.RotateVector2Deg(new Vector2(-movementInput.x, movementInput.y), _cameraTarget.eulerAngles.y);
                lookdirection = Mathf.Acos(Vector2.Dot(Vector2.up, movementInput));
                lookdirection *= Mathf.Sign(movementInput.x);
                _currentLookDirectionAngle = lookdirection;
            }
            else
            {
                lookdirection = _currentLookDirectionAngle;
            }

            _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, lookdirection * -Mathf.Rad2Deg, 0), _rotationDamping);

            // update players position
            if (playerIsMoving)
            {
                _characterController.SimpleMove(_transform.forward * (_movementSpeed * Time.fixedDeltaTime));
            }
        }
    }
}