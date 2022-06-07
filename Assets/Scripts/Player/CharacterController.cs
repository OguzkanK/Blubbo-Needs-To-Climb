using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float movementSmoothing = 1f;
    [SerializeField] private float movementAcceleration = 10f;
    [SerializeField] private float crouchedMovementSpeed = 0.55f;
    [SerializeField] private LayerMask whatIsGround;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Transform ceilingCheck;
	//[SerializeField] private Collider2D crouchDisableCollider;
    [SerializeField] private bool airControl = true;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    
    [SerializeField] public UnityEvent onLandEvent;
    [SerializeField] float groundedRememberTime = 0.2f;
    
    public float groundedRemember = 0;

    private const float GroundedRadius = 0.2f;
    private const float CeilingRadius = 0.25f;
    private bool _isFacingRight = true;
    private bool _wasCrouching = false;
    private Vector3 _velocity = Vector3.zero;

	[System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent onCrouchEvent;
    private static readonly int IsJumping = Animator.StringToHash("isJumping");

    private void Start()
    {
        onLandEvent ??= new UnityEvent();
        onCrouchEvent ??= new BoolEvent();
    }
    
    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;
        groundedRemember -= Time.deltaTime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, whatIsGround);
        foreach(Collider2D col in colliders){
            if(col.gameObject != gameObject){
                isGrounded = true;
                if(!isJumping)
                    groundedRemember = groundedRememberTime;
                Platform platform = col.gameObject.GetComponent<Platform>();
                if (platform)
                {
                    if (col.transform.childCount == 0 && platform.platformAttributes.platformType == PlatformAttributes.PlatformType.Regular)
                    {
                        Player.Instance.lastRegularPlatformPos = platform.platformAttributes.position;
                    }
                }
                if(!wasGrounded)
                {
                    isJumping = false;
                    onLandEvent.Invoke();
                }
            }
        }
    }

    public void Move(float move, bool crouch, bool jump){
        // If crouching, check to see if the player can stand up
        //if(!crouch){
        //    if(Physics2D.OverlapCircle(_ceilingCheck.position, _ceilingRadius, _whatIsGround)){
        //        crouch = true;
        //    }
        //}

        if(isGrounded ||airControl){
            if(crouch){
                if(_wasCrouching){
                    _wasCrouching = true;
                    onCrouchEvent.Invoke(true);
                }
                
                move *= crouchedMovementSpeed;
                
                // if(crouchDisableCollider != null){
                //     crouchDisableCollider.enabled = false;
                // }
            }
            else{
                // if(crouchDisableCollider != null){
                //     crouchDisableCollider.enabled = true;
                // }

                if(_wasCrouching){
                    _wasCrouching = false;
                    onCrouchEvent.Invoke(false);
                }
            }
            
            Vector3 targetVelocity = new Vector2(move * movementAcceleration, _rigidbody2D.velocity.y);
            
            _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity, movementSmoothing);

            if(move > 0 && !_isFacingRight){
                Flip();
            }
            if(move < 0 && _isFacingRight){
                Flip();
            }

            if(groundedRemember > 0 && jump){
                PlayerMovement.Instance.ResetJumpRememberTime();
                groundedRemember = 0;
                isGrounded = false;
                isJumping = true;
                _rigidbody2D.velocity = new Vector3(_rigidbody2D.velocity.x, jumpForce, 0);
                Player.Instance.PlayJumpAnimation();
            }
        }
    }

    public void WhatIsPlayerOn(){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, whatIsGround);
        foreach(Collider2D collider in colliders)
        {
            Platform platform = collider.gameObject.GetComponent<Platform>();
            if (platform.platformAttributes.platformType == PlatformAttributes.PlatformType.Snowing)
            {
                movementSmoothing = 0.4f;
                movementAcceleration = 15f;
            }
            else
            {
                movementSmoothing = 0.15f;
                movementAcceleration = 10f;
            }
        }
    }

    private void Flip(){
        _isFacingRight = !_isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
