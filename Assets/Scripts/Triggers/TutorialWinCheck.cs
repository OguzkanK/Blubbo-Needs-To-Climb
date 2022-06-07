using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialWinCheck : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag($"Player")) return;
        tutorialManager.WinCheck();
    }
}
