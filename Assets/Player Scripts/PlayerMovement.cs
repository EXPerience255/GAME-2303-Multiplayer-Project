using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    CharacterController charCon;

    public float moveSpeed = 4f;

    private Vector3 moveInput;
    private Vector2 lookInput;

    private void OnEnable()
    {
        if (IsLocalPlayer)
        {
            moveAction.action.Enable();
            lookAction.action.Enable();

            // Use performed and canceled for continuous movement tracking
            moveAction.action.performed += OnMovePerformed;
            moveAction.action.canceled += OnMoveCanceled;

            lookAction.action.performed += OnLookPerformed;
        }
    }

    private void OnDisable()
    {
        if (IsLocalPlayer)
        {
            moveAction.action.Disable();
            lookAction.action.Disable();

            // Use performed and canceled for continuous movement tracking
            moveAction.action.performed -= OnMovePerformed;
            moveAction.action.canceled -= OnMoveCanceled;

            // Use started or performed for one-shot triggers like jumping
            lookAction.action.performed -= OnLookPerformed;
        }
    }

    private void Awake()
    {

        charCon = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            Move(moveInput);
        }
    }

    void Move(Vector3 move)
    {
        charCon.Move(move * moveSpeed * Time.deltaTime);
    }
    void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x,
            transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint); //This LookAt is a function within the Transform class, it is NOT the same as this function here
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            // Read move input
            moveInput = moveAction.action.ReadValue<Vector2>();
            moveInput = new Vector3(moveInput.x, 0, moveInput.y);
            moveInput = Vector3.ClampMagnitude(moveInput, 1f);
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
            moveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            // Read look input
            lookInput = lookAction.action.ReadValue<Vector2>();

            // This uses a Raycast from the cursor onto the surface(floor/ground) to create a vector to the player.
            Ray ray = Camera.main.ScreenPointToRay(lookInput);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            float rayDistance; // From our mouse position to the groundPlane

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }
}
