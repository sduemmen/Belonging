using System;
using UnityEngine;
using Utility;

namespace Player.Input
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _rotationDamping;

        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private CharacterController _characterController;
        private Animator _animator;
        private Transform _transform;
        private float _currentLookDirectionAngle;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            // align players rotation by taking into account current camera rotation and movement input
            Vector2 movementInput = InputController.MovementInput;
            bool playerIsMoving = movementInput != Vector2.zero;
            float lookDirection;

            if (playerIsMoving)
            {
                movementInput = MathUtilities.RotateVector2Deg(new Vector2(-movementInput.x, movementInput.y), _cameraTarget.eulerAngles.y);
                lookDirection = Mathf.Acos(Vector2.Dot(Vector2.up, movementInput));
                lookDirection *= Mathf.Sign(movementInput.x);
                _currentLookDirectionAngle = lookDirection;
            }
            else
            {
                lookDirection = _currentLookDirectionAngle;
            }

            Quaternion lookDirectionRotation = Quaternion.Euler(0, lookDirection * -Mathf.Rad2Deg, 0);

            _transform.rotation = Quaternion.Lerp(_transform.rotation, lookDirectionRotation, _rotationDamping);

            // update players position
            if (playerIsMoving)
            {
                float speed = _movementSpeed * Time.fixedDeltaTime;
                _characterController.SimpleMove(lookDirectionRotation * _characterController.transform.forward * speed);
                _animator.ResetTrigger("Idle");
                _animator.SetTrigger("Walking");
            }
            else
            {
                _animator.ResetTrigger("Walking");
                _animator.SetTrigger("Idle");
            }
        }
    }
}