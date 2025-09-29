using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    InputManager inputManager;// Reference to the InputManager script
    PlayerManager playerManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;//Store the direction of movement. Is Vector3 because player can move in x,y or x+y/y+x (Diagonally) 
    Transform cameraObject;// Reference to the main camera position
    Rigidbody playerRigidbody;// Reference to the player's Rigidbody component
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 7f;
    public float rotationSpeed = 14f;
    public bool isRunning;

    public float inAirTime;// Build up speed while falling
    public float leapingVelocity;// Velocity while leaping
    public float fallingVelocity;// Velocity while falling
    public float rayCastHeightOffset = 0.5f;// Height offset for raycasting to detect ground
    public LayerMask groundLayer;// Layer mask to identify ground objects
    public float maxDistance = 1;// Starting point for ground detection raycast
    public bool isGrounded;// Is the player grounded
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();// Get reference to InputManager script
        playerRigidbody = GetComponent<Rigidbody>();//Captures the Rigidbody component on the player
        cameraObject = Camera.main.transform;// Get reference to the main camera's transform. Where the camera is in the scene
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;//Guarantees Cam to be in front of player at all times(3rd person). Player will be in front of camera regardless of movement. takes in account the camera's forward/backward direction. verticalInput = W/S or Up/Down. Values between -1 & 1
        moveDirection += cameraObject.right * inputManager.horizontalInput;//+= adds verticalInput to horizontalInput since most likely we'll press two keys at same time (Integrates both movements) A / D or Left/Right. Values go over 1 since we're adding two values together
        moveDirection.y = 0;//Keeps player from moving Vertically
        moveDirection.Normalize();//Normalizing a vector makes its length 1. Prevents faster diagonal movement
        if (isRunning) moveDirection *= runningSpeed;
        else moveDirection *= walkingSpeed;
        Vector3 movementVelocity = moveDirection;//movementVelocity is declared as Vector3 to transform & pass the moveDirection to the rigidbody
        playerRigidbody.linearVelocity = movementVelocity;//Assigns movementVelocity to the player's Rigidbody (linear)Velocity
    }

    private void HandleRotation()//Force player to rotate then move in certain direction
    {
        Vector3 targetDirection = Vector3.zero;//stores the direction the player should face based on input
        targetDirection = cameraObject.forward * inputManager.verticalInput;//Makes Cam & Player face same direction.
        targetDirection.y = 0;//Keeps player from rotating Vertically
        targetDirection.Normalize();//Prevents faster movement
        if (targetDirection == Vector3.zero) targetDirection = transform.forward;//If targetDirection corresponds to initial orientation (Vector3.zero) then keep player facing forward

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);//Look at targetDirection & store it in targetRotation
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //Slerp = Spherical Linear Interpolation. Gradually changes a rotation over time to avoid making it look robotic.
        //Smoothly rotates from (Quaternion A) current rotation to (Quaternion B) target rotation at a float value.
        //Time.deltaTime makes it frame rate independent
        transform.rotation = playerRotation;//Assigns the new rotation to the player
    }

    public void HandleAllMovement()//Master function that handles all movement related functions. In case we want to call all movement functions at once
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting) return;
        HandleMovement();
        HandleRotation();
    }

    public void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffset;

        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
                animatorManager.PlayerTargetAnimation("Falling", true);

            inAirTime += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTime);//makes the player fall faster over time
        }

        if (Physics.SphereCast(rayCastOrigin, 0.1f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
                animatorManager.PlayerTargetAnimation("Landing", true);

                inAirTime = 0;//reset inAirTime once we land
                isGrounded = true;
                playerManager.isInteracting = false;
        }else
        {
            isGrounded = false;
        }
    }
}
