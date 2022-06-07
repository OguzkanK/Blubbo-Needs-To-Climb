using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EntityGenerator : MonoBehaviour
{
    public static EntityGenerator Instance;
    [SerializeField] private Transform waterCanParent;
    [SerializeField] private GameObject waterCanObject;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void GenerateWaterCan(Vector3 canPosition)
    {
        GameObject waterCan = Instantiate(waterCanObject, canPosition, Quaternion.identity, waterCanParent);
    }

    public void PopulateClouds(int enemyIndex, Vector3 spawnPos, Transform parentPlatform)
    {
        GameObject spawnedEnemy = Instantiate(GameManager.Instance.globalData.entitiesList[enemyIndex], spawnPos,
            quaternion.identity);
        Transform spawnedEnemyTransform = spawnedEnemy.GetComponent<Transform>();
        spawnedEnemyTransform.parent = parentPlatform;
        if(enemyIndex == 1)
        {
            FlyingEnemy flyingEnemy = spawnedEnemy.GetComponent<FlyingEnemy>();
            flyingEnemy.parentPlatformName = parentPlatform.name;
        }
    }
}
