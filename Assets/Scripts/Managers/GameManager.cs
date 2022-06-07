using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject beanstalk;
    [SerializeField] private GameObject victoryLadder;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject GameScreen;
    [SerializeField] private GameObject scoreCrown;
    [SerializeField] private GameObject floorCrown;
    [SerializeField] private TMP_Text stageNumber;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text playerLivesText;
    [SerializeField] private TMP_Text scoreUIText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private TMP_Text topFloorText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private List<Image> waterCanBar;

    public static GameManager Instance;
    public GlobalData globalData;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        globalData.waterCans = 0;
        globalData.stageNumber++;
        UpdateWaterCanUI();
        UpdateUI();
    }

    public void WinCheck()
    {
        if (globalData.waterCans != 2) return;
        beanstalk.SetActive(false);
        victoryLadder.SetActive(true);
    }

    public void LoadNewStage()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void UpdateUI()
    {
        playerHealthText.text = $"Health: {globalData.playerHealth}";
        playerLivesText.text = $"Lives: {globalData.playerLives}";
        stageNumber.text = $"Stage: {globalData.stageNumber}";
        scoreUIText.text = $"Score: {globalData.totalScore}";
    }

    public void AddScore(int scoreVal)
    {
        globalData.totalScore += scoreVal;
        scoreUIText.text = $"Score: {globalData.totalScore}";
    }

    public void UpdateWaterCanUI()
    {
        if (globalData.waterCans == 0)
        {
            foreach (var t in waterCanBar)
            {
                t.color = Color.black;
            }
        }
        else
            waterCanBar[globalData.waterCans - 1].color = Color.white;
    }

    public void EndGame()
    {
        Destroy(Player);
        if (globalData.stageNumber > PlayerPrefs.GetInt("TopFloor", 0))
        {
            floorCrown.SetActive(true);
            PlayerPrefs.SetInt("TopFloor", globalData.stageNumber);
        }
        if (globalData.totalScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            scoreCrown.SetActive(true);
            PlayerPrefs.SetInt("HighScore", globalData.totalScore);
        }
        
        topFloorText.text = $"Top Floor: {PlayerPrefs.GetInt("TopFloor")}";
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore")}";
        scoreText.text = $"Score: {globalData.totalScore}";
        floorText.text = $"Floor: {globalData.stageNumber}";
        GameOverScreen.SetActive(true);
        GameScreen.SetActive(false);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }
    
    public void CollectWater(GameObject waterCan, Transform waterCanTransform)
    {
        globalData.waterCans++;
        UpdateWaterCanUI();
        waterCanTransform.parent = null;
        Destroy(waterCan);
    }
}
