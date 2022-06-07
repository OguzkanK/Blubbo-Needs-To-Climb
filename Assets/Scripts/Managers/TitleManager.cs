using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TMP_Text topFloorText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private Toggle skipTutorial;
    
    public GlobalData globalData;

    
    private void Start()
    {
        skipTutorial.isOn = PlayerPrefs.GetInt("SkipTutorial", 0) == 1;
        topFloorText.text = $"Top Floor: {PlayerPrefs.GetInt("TopFloor", 0)}";
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
        globalData.stageNumber = 0;
        globalData.playerHealth = 3;
        globalData.playerLives = 3;
        globalData.totalScore = 0;
    }

    public void ExitGameButtonHandler()
    {
        Application.Quit();
    }

    public void StartButtonHandler()
    {
        if(skipTutorial.isOn == false)
            PlayerPrefs.SetInt("SkipTutorial", 1);
        SceneManager.LoadScene(skipTutorial.isOn ? 2 : 1);
    }

    public void ToggleTutorial()
    {
        PlayerPrefs.SetInt("SkipTutorial", skipTutorial.isOn ? 1 : 0);
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("TopFloor");
        PlayerPrefs.DeleteKey("HighScore");
        topFloorText.text = $"Top Floor: 0";
        highScoreText.text = $"HighScore: 0";
    }
}
