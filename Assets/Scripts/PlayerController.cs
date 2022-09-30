using UnityEngine;
using static Settings.PlayerSettings;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public MovementSettings movementSettings;
    public InputManager inputManager;

    public Transform cameraController;
    private Transform _transform;
    private CharacterController _characterController;
    private float currentRotationAngle;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
       HandleMovement();
    }

    private void HandleMovement()
    {
        // align players rotation by taking into account current camera rotation and movement input
        Vector2 movementInput = inputManager.MovementInput;
        bool playerIsMoving = movementInput != Vector2.zero;
        float angle;
        
        if (playerIsMoving) {
            movementInput = Math.RotateVector2Deg(new Vector2(-movementInput.x, movementInput.y), cameraController.eulerAngles.y);
            angle = Mathf.Acos(Vector2.Dot(Vector2.up, movementInput));
            angle *= Mathf.Sign(movementInput.x);
            currentRotationAngle = angle;
        } else {
            angle = currentRotationAngle;
        }
        
        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(0, angle * -Mathf.Rad2Deg, 0), movementSettings.rotationDamping);
        
        // update players position
        if (playerIsMoving) {
            _characterController.Move(_transform.forward * (movementSettings.movementSpeed * Time.fixedDeltaTime));
        }
    }
}
