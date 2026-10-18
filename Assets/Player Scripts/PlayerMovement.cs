using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveAction;
    CharacterController charCon;

    public float moveSpeed = 4f;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Awake()
    {
        charCon = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        /*if (move != Vector3.zero)
            transform.forward = move;*/

        charCon.Move(move * moveSpeed * Time.deltaTime);
    }
}
