using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Player player;
    private Vector2 horizontalInput = Vector2.zero;
    private Vector3 verticalVelocity = Vector3.zero;
    private float accerlation = 0.5f;
    private float speed = 0f;
    private float speedMax = 1f;
    private Vector3 movement;
    private bool isGrounded;
    private bool cantStandUp;
    private bool crouchInputPressed;
    private bool jumpLastFrame;
    public LayerMask headCollisonLayer;
    public LayerMask footCollisonLayer;

    private float posX, posY, posZ;
    private float rotX, rotY, rotZ;

    [SerializeField] private AK.Wwise.Switch switchCrouch;
    [SerializeField] private AK.Wwise.Switch switchSnow;
    [SerializeField] private AK.Wwise.Switch switchStone;

    //public AkSwitch switchSnowAudio;
    //public AkSwitch switchCrouchAudio;
    //public AkSwitch switchStoneAudio;

    private bool isOnSnow;
    private bool isOnStone;
    private bool isOnCrouch;

    private void Start()
    {     
        player = GetComponent<Player>();
        speedMax = player.MoveSpeed;
    }

    public void ToggleMovement()
    {
        player.InputEnabled = !player.InputEnabled;
        verticalVelocity = new Vector3(posX, posY, posZ);
        //player.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
        //player.playerPivot.rotation = Quaternion.Euler(0, player.playerPivot.rotation.y, player.playerPivot.rotation.z);
        player.ActivateIdleAnimation();
        player.Controller.Move(verticalVelocity);
    }

    public void SetPosX(float _posX)
    {
        posX = _posX;
    }

    public void SetPosY(float _posY)
    {
        posY = _posY;
    }
    public void SetPosZ(float _posZ)
    {
        posZ = _posZ;
    }

    public void SetRotX(float _rotX)
    {
        rotX = _rotX;
    }

    public void SetRotY(float _rotY)
    {
        rotY = _rotY;
    }

    public void SetRotZ(float _rotZ)
    {
        rotZ = _rotZ;
    }


    private void Update()
    {
        if (player.InputEnabled)
        {

            jumpLastFrame = false;
            bool wasGrounded = isGrounded;
            GroundCheck();

            if (isGrounded && !wasGrounded)
            {
                if (!player.IsCrouching)
                {
                    player.jumpLandAudio.HandleEvent(player.jumpLandAudio.gameObject);
                }                
            }

            JumpCheck();
            CrouchCheck();
            OnMovementPressed();
            player.PlayAnimations(speed);
        }
    }

    private void JumpCheck()
    {
        if (player.IsJumping)
        {
            
            if (player.Controller.isGrounded)
            {
                jumpLastFrame = true;
                verticalVelocity.y = Mathf.Sqrt(-2 * player.JumpForce * -player.Gravity);
            }
            player.IsJumping = false;          
        }
    }

    private void GroundCheck()
    {
        if (player.Controller.isGrounded)
        {
            PlayFootStepSound(horizontalInput);
            isGrounded = true;
            verticalVelocity.y += 0;
        }
        else
        {
            isGrounded = false;
            verticalVelocity.y += -player.Gravity * Time.deltaTime;  
        }
    }

    private void CrouchCheck()
    {
        if (!CheckForHeadCollision() && !crouchInputPressed && !player.IsSprinting)
        {
            player.IsCrouching = false;
            speedMax = player.MoveSpeed;
            player.step_Distance = player.walk_step_Distance;
            ResetHeight();
        }
    }

    public void PlayFootStepSound(Vector2 move)
    {
        if (player.IsJumping) return;

        FootRayCastCheck();

        if (move == new Vector2(0, 0))
        {
            player.accumulated_Distance = 0f;
            return;
        }

        player.accumulated_Distance += Time.deltaTime;

        if (player.accumulated_Distance > player.step_Distance)
        {
            //player.footstepSound.Play();
            player.accumulated_Distance = 0f;            
            player.playFootStepsAudio.HandleEvent(player.playFootStepsAudio.gameObject);
        }
    }

    private void FootRayCastCheck()
    {
        if (player.IsCrouching)
        {
            switchCrouch.SetValue(player.gameObject);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(player.rayCastFoot.position, player.rayCastFoot.TransformDirection(Vector3.down), out hit, 0.5f))
        {
            //Debug.Log(hit.transform.tag);
            if (hit.transform.CompareTag("Snow"))
            {
                switchSnow.SetValue(player.playFootStepsAudio.gameObject);
                Debug.DrawRay(player.rayCastFoot.position, player.rayCastFoot.TransformDirection(Vector3.down) * 2f, Color.red);
            }
            else
            {
                switchStone.SetValue(player.playFootStepsAudio.gameObject);               
                Debug.DrawRay(player.rayCastFoot.position, player.rayCastFoot.TransformDirection(Vector3.down) * 2f, Color.blue);
            }
        }
        else
        {
            switchStone.SetValue(player.playFootStepsAudio.gameObject);
        }
    }


    private void OnMovementPressed()
    {
        if (!player.InputEnabled) return;

        UpdateSpeedValue();
        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed;
        movement = horizontalVelocity + verticalVelocity;
        movement *= Time.deltaTime;
        player.Controller.Move(movement);      
    }

    public void UpdateSpeedValue()
    {
        if (horizontalInput == Vector2.zero)
        {
            speed = 0;
        }
        speed += accerlation;
        if (speed > speedMax)
        {
            speed = speedMax;
        }
    }

    public void ReceiveInput(Vector2 _horizontalInput)
    {
        horizontalInput = _horizontalInput;
    }

    public void InputJumpPressed()
    {
        if (player.IsCrouching) return;
        player.IsJumping = true;
        player.jumpStartAudio.HandleEvent(player.jumpStartAudio.gameObject);
    }

    public void InputSprintPressed()
    {
        if (player.IsCrouching) return;

        player.IsSprinting = true;
        speedMax = player.SprintSpeed;
        player.step_Distance = player.sprint_step_Distance;
    }
    public void InputSprintReleased()
    {
        player.IsSprinting = false;
        speedMax = player.MoveSpeed;
        player.step_Distance = player.walk_step_Distance;
    }

    public void InputCrouchPressed()
    {    
        if (player.IsJumping) return;
        if (player.IsSprinting) return;

        cantStandUp = false;
        player.IsCrouching = true;
        crouchInputPressed = true;
        speedMax = player.CrouchSpeed;
        player.step_Distance = player.crouch_step_Distance;
        ReduceHeight();
    }
    public void InputCrouchReleased()
    {
        player.IsCrouching = false;
        crouchInputPressed = false;

        /*if (!cantStandUp)
        {
            speedMax = player.MoveSpeed;
            player.step_Distance = player.walk_step_Distance;
            ResetHeight();
        }*/
    }

    private bool CheckForHeadCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(player.headCollisonPoint.position, 0.65f, headCollisonLayer);

        if (colliders.Length != 0)
        {        
            return true;            
        }
        return false;
    }

    void ReduceHeight()
    {
        player.Controller.height = player.ReducedHeight;
    }

    void ResetHeight()
    {
        //player.Controller.height = Mathf.SmoothStep(player.ReducedHeight, player.OriginalHeight, 2f);
        player.Controller.height = player.OriginalHeight;
    }
}
