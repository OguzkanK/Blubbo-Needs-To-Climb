using UnityEngine;

public class SpikedEnemy : MonoBehaviour
{
    public bool _isInAttackState = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float switchStateTimer = 2.5f;
    [SerializeField] private PolygonCollider2D attackCollider;

    private void Update()
    {
        switchStateTimer -= Time.deltaTime;

        if (switchStateTimer <= 0)
        {
            ChangeState();
            switchStateTimer = 5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        
        if (!_isInAttackState && Player.Instance.playerRigidbody.velocity.y < 0)
        {
            Die();
            Player.Instance.BouncePlayerUp(10f);
        }
        else
        {
            Player.Instance.TakeDamage(15f * Mathf.Sign(col.transform.position.x - transform.position.x), 10f);
        }
    }

    private void ChangeState()
    {
        if(!_isInAttackState)
        {
            spriteRenderer.sprite = GameManager.Instance.globalData.spikedEnemySprites[0];
            attackCollider.enabled = true;
            _isInAttackState = true;
        }
        else
        {
            spriteRenderer.sprite = GameManager.Instance.globalData.spikedEnemySprites[1];
            attackCollider.enabled = false;
            _isInAttackState = false;
        }
    }
    private void Die()
    {
        GameManager.Instance.AddScore(500);
        transform.parent = null;
        Destroy(gameObject);
    }
}