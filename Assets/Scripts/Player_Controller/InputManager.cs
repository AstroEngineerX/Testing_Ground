using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerController playerController;// Reference to the PlayerController input actions
    public Vector2 movementInput;// Stores the movement input values from the PlayerController & show it in inspector for debugging

    public float verticalInput;//Give InputManager capacity to capture & send inputs to PlayerMovement
    public float horizontalInput;//Store vertical & horizontal of the Vector2

    AnimatorManager animatorManager;
    public float moveAmount;

    PlayerMovement playerMovement;
    public bool shiftInput;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (playerController == null)// If playerController is not initialized assign it
        {
            playerController = new PlayerController();// Initialize the PlayerController input actions
            playerController.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();//Receive movement input
            playerController.PlayerActions.Shift.performed += i => shiftInput = true;
            playerController.PlayerActions.Shift.canceled += i => shiftInput = false;
        }
        playerController.Enable();
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;//stores the movement of X & Y from the public Vector2 movementInput Variable declared above
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerMovement.isRunning);
    }

    public void HandleAllInputs()// Called in PlayerManager. That's why it's public
    {
        HandleMovementInput();
        HandleRunningInput();
    }

    public void HandleRunningInput()
    {
        if (shiftInput && moveAmount > 0.5f)
        {
            playerMovement.isRunning = true;
        }
        else
        {
            playerMovement.isRunning = false;
        }
    }
}
