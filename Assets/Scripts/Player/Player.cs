using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private InputSystem_Actions inputActions;
    private Animator animator;


    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Movement();
        
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward.normalized * inputDirection.z + camRight.normalized * inputDirection.x).normalized;

        animator.SetFloat("Speed", moveInput.magnitude);

        if (moveDir == Vector3.zero) return;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
    }

}
