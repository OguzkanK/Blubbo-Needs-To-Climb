using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    [SerializeField] private GameObject playerArrow;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private bool crouch;
    [SerializeField] private float cutJumpHeight;
    [SerializeField] [Range(0, 1)] private float jumpPressedRememberTime = 0.2f;
    
    public CharacterController controller;
    private float _horizontalMove;
    private bool _jump;
    private float _jumpPressedRemember = 0;
    
    public Rigidbody2D rigidBody2D;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update() {

        playerArrow.SetActive(playerTransform.position.y >= 5f);

        _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        _jumpPressedRemember -= Time.deltaTime;
        if(Input.GetButtonDown("Jump"))
        {
            _jumpPressedRemember = jumpPressedRememberTime;
        }
        if(Input.GetButtonUp("Jump"))
        {
            if (rigidBody2D.velocity.y > 0)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, rigidBody2D.velocity.y * cutJumpHeight);
            }
        }

        if(Input.GetButtonDown("Crouch")){
            crouch = true;
        }
        else if(Input.GetButtonUp("Crouch")){
            crouch = false;
        }

        if(Input.GetButtonDown("Quit")){
            Application.Quit();
        }
    }

    public void ResetJumpRememberTime()
    {
        _jumpPressedRemember = 0;
    }

    void FixedUpdate()
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, crouch, (_jumpPressedRemember > 0));
    }
}
