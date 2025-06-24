using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector2 moveInput;
    private InputSystem inputActions;
    private Animator animator;

    private float currentSpeed;

    private void Awake()
    {
        inputActions = new InputSystem();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Sprint.started += OnSprintStart;
        inputActions.Player.Sprint.canceled += OnSprintEnd;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Sprint.started -= OnSprintStart;
        inputActions.Player.Sprint.canceled -= OnSprintEnd;

        inputActions.Player.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        currentSpeed = walkSpeed;
        animator.SetBool("IsSprinting", false);
    }

    private void Update()
    {
        Movement();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnSprintStart(InputAction.CallbackContext context)
    {
        currentSpeed = sprintSpeed;
        animator.SetBool("IsSprinting", true);
    }

    private void OnSprintEnd(InputAction.CallbackContext context)
    {
        currentSpeed = walkSpeed;
        animator.SetBool("IsSprinting", false);
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

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
    }
}
