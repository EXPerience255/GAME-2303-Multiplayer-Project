using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    CharacterController charCon;

    public float moveSpeed = 4f;

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    private void Awake()
    {
        charCon = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // Read move input
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        charCon.Move(move * moveSpeed * Time.deltaTime);

        /*if (move != Vector3.zero)
            transform.forward = move;*/

        // Read look input
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

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
    void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x,
            transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint); //This LookAt is a function within the Transform class, it is NOT the same as this function here
    }
}
