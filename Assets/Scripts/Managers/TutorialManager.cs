using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] private GameObject beanstalk;
    [SerializeField] private GameObject victoryLadder;
    [SerializeField] private GameObject WinTrigger;
    [SerializeField] private List<Image> waterCanBar;
    
    public GlobalData globalData;

    [SerializeField] private int _waterCans = 0;

    private void Start()
    {
        globalData.stageNumber = 0;
        globalData.playerHealth = 3;
        globalData.playerLives = 3;
        globalData.totalScore = 0;
    }

    public void WinCheck()
    {
        beanstalk.SetActive(false);
        victoryLadder.SetActive(true);
    }
    
    public void CollectWater(GameObject waterCan, Transform waterCanTransform)
    {
        _waterCans++;
        UpdateWaterCanUI();
        waterCanTransform.parent = null;
        Destroy(waterCan);
    }

    public void UpdateWaterCanUI()
    {
        waterCanBar[_waterCans - 1].color = Color.white;

        if (_waterCans == 2)
        {
            WinTrigger.SetActive(true);
        }
    }
}
