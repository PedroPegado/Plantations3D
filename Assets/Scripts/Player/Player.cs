using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public Transform cameraTransform;

    [Header("Inventário de Sementes")]
    public PlayerInventory playerInventory;

    private CharacterController controller;
    private Vector2 moveInput;
    private InputSystem inputActions;
    private Animator animator;

    private float currentSpeed;

    private SeedPickup currentSeedPickup;
    private bool isPickingUp = false;
    private PlantingSpot currentPlantingSpot;

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

        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Sprint.started -= OnSprintStart;
        inputActions.Player.Sprint.canceled -= OnSprintEnd;

        inputActions.Player.Interact.performed -= OnInteract; 

        inputActions.Player.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        currentSpeed = walkSpeed;
        animator.SetBool("IsSprinting", false);

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory não atribuído ao PlayerMovement no Inspector!", this);
        }
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

    private float GetAnimationClipLength(string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 1f;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentSeedPickup != null && !isPickingUp)
        {
            SeedController seedController = currentSeedPickup.GetComponentInParent<SeedController>();
            if (seedController != null && seedController.selectedSeed != null)
            {
                isPickingUp = true;
                animator.SetBool("IsPickingUp", true);

                float pickupDuration = GetAnimationClipLength("Pickup");
                StartCoroutine(FinishPickup(pickupDuration, seedController));
            }
        }

        if (currentPlantingSpot != null)
        {
            if (currentPlantingSpot.isReadyToHarvest)
            {
                SeedData harvestedItem = currentPlantingSpot.HarvestPlant();
                if (harvestedItem != null)
                {
                    playerInventory.AddSeed(harvestedItem, 1); 
                    Debug.Log($"Você colheu 1x {harvestedItem.seedName}.");
                }
            }
            else if (!currentPlantingSpot.isOccupied)
            {
                HotbarUI hotbarUI = FindObjectOfType<HotbarUI>(); 
                if (hotbarUI != null)
                {
                    SeedData selectedSeed = hotbarUI.GetSelectedSeed();
                    if (selectedSeed != null)
                    {
                        if (playerInventory.GetSeedQuantity(selectedSeed) > 0)
                        {
                            bool planted = currentPlantingSpot.TryPlantSeed(selectedSeed);
                            if (planted)
                            {
                                playerInventory.RemoveSeed(selectedSeed, 1); 
                                InteractionManager.Instance.Hide();
                            }
                        }
                        else
                        {
                            Debug.Log($"Você não tem mais {selectedSeed.seedName} para plantar!");
                        }
                    }
                    else
                    {
                        Debug.Log("Nenhuma semente selecionada na hotbar para plantar.");
                    }
                }
                else
                {
                    Debug.LogWarning("HotbarUI não encontrada na cena. Não é possível plantar.");
                }
            }
            else if (currentPlantingSpot.isGrowing)
            {
                Debug.Log("A planta ainda está crescendo. Não pode interagir.");
            }
            return;
        }
        Debug.Log("Nenhum item ou local de plantio para interagir.");
    }

    private IEnumerator FinishPickup(float delay, SeedController seedController)
    {
        yield return new WaitForSeconds(delay);

        bool added = playerInventory.AddSeed(seedController.selectedSeed, 1);

        if (added)
        {
            Debug.Log($"Você pegou 1x {seedController.selectedSeed.seedName}.");
            InteractionManager.Instance.Hide();

            Destroy(currentSeedPickup.transform.parent.gameObject);
            currentSeedPickup = null;

            if (QuestUIManager.Instance != null)
            {
                QuestUIManager.Instance.OnSeedPickedUp();
            }
        }
        else
        {
            Debug.Log($"Não foi possível pegar {seedController.selectedSeed.seedName}. Inventário cheio!");
        }

        currentSeedPickup = null;
        isPickingUp = false;
        animator.SetBool("IsPickingUp", false);
    }


    private void Movement()
    {
        if (isPickingUp) return;
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

    public void SetCurrentSeedPickup(SeedPickup seed)
    {
        currentSeedPickup = seed;
    }

    public void ClearCurrentSeedPickup()
    {
        currentSeedPickup = null;
    }

    public void SetCurrentPlantingSpot(PlantingSpot spot)
    {
        currentPlantingSpot = spot;
    }

    public void ClearCurrentPlantingSpot()
    {
        currentPlantingSpot = null;
    }

    public string GetSelectedSeedName()
    {
        HotbarUI hotbarUI = FindObjectOfType<HotbarUI>();
        if (hotbarUI != null)
        {
            SeedData selectedSeed = hotbarUI.GetSelectedSeed();
            if (selectedSeed != null)
            {
                return selectedSeed.seedName;
            }
        }
        return "nada"; 
    }
}