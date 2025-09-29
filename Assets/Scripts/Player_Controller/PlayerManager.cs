using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;// Reference to the InputManager script
    PlayerMovement playerMovement;// Reference to the PlayerMovement script
    Animator animator;

    public bool isInteracting;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputManager.HandleAllInputs();// Call the method to handle all inputs from InputManager. Constantly checking for inputs
    }

    private void FixedUpdate()// Called at a fixed interval. Good for handling physics related stuff
    {
        playerMovement.HandleAllMovement();// Call the method to handle all movement from PlayerMovement
    }
    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
    }
}
