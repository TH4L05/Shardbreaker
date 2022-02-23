using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Movement movement;
    private MouseLook mouseLook;
    private Shoot shoot;
    private Player player;
    [SerializeField] PauseMenu pauseMenuRef;

    private PlayerInput playerInput;
    private InputActionAsset inputMap;

    Vector2 horizontalInput;
    Vector2 mouseInput;


    public void GetComponentReferences()
    {
        playerInput = GetComponent<PlayerInput>();
        player = Game.instance.player.GetComponent<Player>();
        pauseMenuRef = FindObjectOfType<PauseMenu>();

        if (player)
        {           
            movement = player.GetComponent<Movement>();
            mouseLook = player.GetComponent<MouseLook>();
            shoot = player.GetComponent<Shoot>();
        }
        else
        {
            playerInput = null;
            movement = null;
            mouseLook = null;
            shoot = null;
        }
            
    }

    public void SetNUll()
    {
        playerInput = null;
        movement = null;
        mouseLook = null;
        shoot = null;
        pauseMenuRef = null;
    }


    private void Start()
    {
        //GetComponentReferences();
        SetInputActions();
    }

    private void SetInputActions()
    {
        if (playerInput != null)
        {
            playerInput.actions["Movement"].performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
            playerInput.actions["Jump"].performed += _ => movement.InputJumpPressed();
            playerInput.actions["PrimaryFire"].performed += _ => player.ReciveShootInput();
            playerInput.actions["PrimaryFire"].canceled += _ => player.ReciveShootInput();
            playerInput.actions["SecondaryFire"].performed += _ => player.UseAbility();
            playerInput.actions["Reload"].performed += _ => shoot.ReloadOnInput();
            playerInput.actions["Crouch"].performed += _ => movement.InputCrouchPressed();
            playerInput.actions["Crouch"].canceled += _ => movement.InputCrouchReleased();
            playerInput.actions["Sprint"].performed += _ => movement.InputSprintPressed();
            playerInput.actions["Sprint"].canceled += _ => movement.InputSprintReleased();
            playerInput.actions["AbilityChange"].performed += _ => player.SwitchAbility();
            playerInput.actions["Mouse"].performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        }
        

        if (pauseMenuRef != null)
        {
            playerInput.actions["TogglePause"].performed += _ => pauseMenuRef.ToggleMenu();
        }
    }

    private void FixedUpdate()
    {
        if (playerInput != null)
        {
            if (player.InputEnabled)
            {

                playerInput.actions["TogglePause"].Enable();


                movement.ReceiveInput(horizontalInput);
                mouseLook.ReceiveInput(mouseInput);
            }
            else
            {
                playerInput.actions["TogglePause"].Disable();
            }
        }
            
    }

    private void OnEnable()
    {
        StartCoroutine(Co_ActivateInputComponent());
    }

    private IEnumerator Co_ActivateInputComponent()
    {
        yield return new WaitForEndOfFrame();
        playerInput.enabled = false;
        yield return new WaitForSeconds(0.2f);
        playerInput.enabled = true;
    }


    /*private void OnEnable()
    {
        playerInput.ActivateInput();
    }
    private void OnDestroy()
    {
        playerInput.DeactivateInput();
    }*/
}
