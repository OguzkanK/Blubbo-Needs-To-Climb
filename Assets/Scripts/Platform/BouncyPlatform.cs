using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    public float bouncePower;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag($"Player") && col.gameObject.transform.position.y > transform.position.y)
        {
            Player.Instance.BouncePlayerUp(bouncePower);
            Player.Instance.PlayJumpAnimation();
        }
    }
}
