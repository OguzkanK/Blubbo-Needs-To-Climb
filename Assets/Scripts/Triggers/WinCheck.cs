using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag($"Player")) return;
        GameManager.Instance.WinCheck();
    }
}
