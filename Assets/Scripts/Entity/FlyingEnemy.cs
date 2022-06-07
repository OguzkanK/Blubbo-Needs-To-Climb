using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private Rigidbody2D rigidbody2D;
    private Vector3 _velocity = Vector3.zero;
    public string parentPlatformName;
    [SerializeField] private Transform raycastPoint;

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector2(moveSpeed * Time.fixedDeltaTime, 0);
        rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref _velocity, 0.1f);
    }

    private void Update()
    {
        RaycastHit2D platformCheck = Physics2D.Raycast(raycastPoint.position, Vector2.down);
        if(!platformCheck)
            Flip();
        else if (!platformCheck.transform.name.Equals(parentPlatformName) && platformCheck.transform.gameObject.layer != 6)
        {
            Flip();
        }
    }

    private void Flip()
    {
        moveSpeed *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        
        if (Player.Instance.playerRigidbody.velocity.y < 0)
        {
            Die();
            Player.Instance.BouncePlayerUp(10f);
        }
        else
        {
            Player.Instance.TakeDamage(15f * Mathf.Sign(col.transform.position.x - transform.position.x), 10f);
        }
    }
    
    private void Die()
    {
        GameManager.Instance.AddScore(500);
        transform.parent = null;
        Destroy(gameObject);
    }
}
