using UnityEngine;

public class WaterCanTutorial : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag($"Player")) return;
        Destroy(gameObject);
        tutorialManager.CollectWater(gameObject, transform);
    }
}
