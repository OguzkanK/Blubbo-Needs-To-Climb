using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 targetPoint;
    public Vector3 otherPoint;
    public float platformSpeed = 5f;
    [SerializeField] private float platformWaitTimeDefault = 0.3f;
    public float platformWaitTime = 0f;
    private Rigidbody2D _rigidbody2D;
    private Vector3 _velocity = Vector3.zero;
    
    private void Start()
    {
        platformWaitTime = platformWaitTimeDefault;
        transform.position = otherPoint;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player.Instance.transform.parent = transform;
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player.Instance.transform.parent = null;
        }
    }

    private void Update()
    {
        platformWaitTime -= Time.deltaTime;
        if(Mathf.Abs(targetPoint.x - transform.position.x) > 0.05f &&
           Mathf.Abs(targetPoint.y - transform.position.y) > 0.05f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref _velocity, platformSpeed);
            platformWaitTime = platformWaitTimeDefault;
        }
        else if(platformWaitTime <= 0)
            (targetPoint, otherPoint) = (otherPoint, targetPoint);
    }
}
