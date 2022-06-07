using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Global Manager", menuName = "Global Manager", order = 1)]
public class GlobalData : ScriptableObject
{
    public int stageNumber = 0;
    public int waterCans = 0;
    public int totalScore;
    public int playerLives;
    public int playerHealth;
    public List<Sprite> platformSprites;
    public List<Sprite> spikedEnemySprites;
    public List<GameObject> entitiesList;
}