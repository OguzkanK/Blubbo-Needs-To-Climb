using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    //private GlobalData globalData = GameManager.Instance.globalData;
    public Rigidbody2D playerRigidbody;
    public Animator playerAnimator;
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    [SerializeField] private float invincibilityTimerDefault = 0f;
    [SerializeField] private Transform startingPlatform;
    private float _invincibilityTimer = 0f;

    public Vector2 lastRegularPlatformPos;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        lastRegularPlatformPos = new Vector3(0, -3.7f, 0);
    }

    private void Update()
    {
        if(_invincibilityTimer >= 0)
            _invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float knockbackX, float knockbackY)
    {
        if (_invincibilityTimer >= 0) return;
        if(knockbackX != 0 ||knockbackY != 0)
        {
            playerRigidbody.velocity = new Vector2(knockbackX, knockbackY);
        }
        GameManager.Instance.globalData.playerHealth--;
        if (GameManager.Instance.globalData.playerHealth <= 0)
        {
            Die();
        }
        GameManager.Instance.UpdateUI();

        _invincibilityTimer = invincibilityTimerDefault;
        // Play damage animation
    }

    public void Fall()
    {
        Die();
        
        // Particle effect from bottom

    }

    public void Heal()
    {
        GameManager.Instance.globalData.playerLives++;
        GameManager.Instance.UpdateUI();
        GameManager.Instance.AddScore(1000);
    }

    public void BouncePlayerUp(float bouncePower)
    {
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, bouncePower);
    }

    public void Die()
    {
        GameManager.Instance.globalData.playerLives--;
        if (GameManager.Instance.globalData.playerLives <= 0)
        {
            GameManager.Instance.EndGame();
        }
        else
        {
            transform.position = new Vector3(lastRegularPlatformPos.x,
                lastRegularPlatformPos.y + 1.5f, 0);
            GameManager.Instance.globalData.playerHealth = 3;
            GameManager.Instance.UpdateUI();
        }
    }

    public void PlayJumpAnimation()
    {
        playerAnimator.SetTrigger("isJumping");
    }
    
}
