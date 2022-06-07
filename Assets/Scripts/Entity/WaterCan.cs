using UnityEngine;

public class WaterCan : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag($"Player")) return;
        GameManager.Instance.CollectWater(gameObject, transform);
    }
}
