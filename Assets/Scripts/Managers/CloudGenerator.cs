using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
public class CloudGenerator : MonoBehaviour
{
    [SerializeField] private Transform platformsParent;
    [SerializeField] private GameObject platform; 
    [SerializeField] private float ceilingLimit = 3.5f;
    [SerializeField] private float groundLimit = -4.5f;
    [SerializeField] private float bouncyPlatformBouncePower = 20f;
    [SerializeField] private GameObject ghostPlatform;
    [SerializeField] private Transform ghostPlatformParent;
    private List<int> _platformsOnTheLeft = new List<int>();
    private List<int> _platformsOnTheRight = new List<int>();

    private int _difficulty; 
    private float _defaultCloudLength = 2f;
    private List<GameObject> platformGameObjects = new List<GameObject>();
    private List<Tuple<int, int>> vacantNodes = new List<Tuple<int, int>>();
    private List<Tuple<int, Vector3, Transform>> _entitiesToSpawn = new List<Tuple<int, Vector3, Transform>>();
    private Vector3 _leftFinal, _rightFinal;
    
    //private GlobalData globalManager = GameManager.Instance.globalData;

    [SerializeField] private List<Tuple<string, Tuple<float, float>, PlatformAttributes.PlatformType, Vector3, Sprite, int, Vector3>> _platformsToDraw =
        new List<Tuple<string, Tuple<float, float>, PlatformAttributes.PlatformType, Vector3, Sprite, int, Vector3>>();
    
    private void Start()
    {
        GenerateLevel();
    }

    public void ClearLevel(){
        foreach(var platform in platformGameObjects)
        {
            platform.transform.parent = null;
            Destroy(platform);
        }
        platformGameObjects.Clear();
        _platformsToDraw.Clear();
        vacantNodes.Clear();
    }

    public void GenerateLevel(){
        
        _platformsToDraw.Add(new Tuple<string, Tuple<float, float>, PlatformAttributes.PlatformType, Vector3, Sprite, int, Vector3>(
            $"StartingPlatform", 
            new Tuple<float, float>(1f, 1f),
            PlatformAttributes.PlatformType.StartingLeaf, 
            new Vector3(0, -4.5f, 0),
            null, 
            0,
            Vector3.zero
        ));
        vacantNodes.Add(new Tuple<int, int>( 0, 1));
        vacantNodes.Add(new Tuple<int, int>( 0, 2));

        _difficulty = (((GameManager.Instance.globalData.stageNumber / 10) + 1) * 5) + 10; 
        

        for(int index = 0; index < _difficulty; index++){
            int randomVacantNode = UnityEngine.Random.Range(0, vacantNodes.Count);
            
            SpawnPlatform($"Platform-{_platformsToDraw.Count}", vacantNodes[randomVacantNode].Item1, vacantNodes[randomVacantNode].Item2);

            vacantNodes.RemoveAt(randomVacantNode);
        }

        int tupleIndex = 0;
        foreach ((string item1, var item2, var item3, var item4, var item5, int item6, var item7) in _platformsToDraw)
        {
            if(tupleIndex == _platformsOnTheLeft[_platformsOnTheLeft.Count / 2] || tupleIndex == _platformsOnTheRight[_platformsOnTheRight.Count / 2])
                _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(2, new Vector3(item4.x, item4.y + 2f, item4.z), null));
            DrawPlatform(item1, item2.Item1, item2.Item2, item3, item4, item5,
                    item6, item7);
            tupleIndex++;
        }
        foreach ((int item1, Vector3 item2, Transform item3) in _entitiesToSpawn)
        {
            EntityGenerator.Instance.PopulateClouds(item1, item2, item3);
        }

        EntityGenerator.Instance.GenerateWaterCan(new Vector3(_leftFinal.x, _leftFinal.y + 2f, 0));
        EntityGenerator.Instance.GenerateWaterCan(new Vector3(_rightFinal.x, _rightFinal.y + 2f, 0));
    }
    
    private void SpawnPlatform(string platformName, int platformIndex, int leftOrRight){
        Vector3 cloudPosition, savedPos = Vector3.zero; 
        PlatformAttributes.PlatformType typeOfCreatedPlatform = PlatformAttributes.PlatformType.Regular;
        
        float randomPlatformLength = UnityEngine.Random.Range(_defaultCloudLength, _defaultCloudLength + 10);
        
        float platformYPlacementChance = 1f; 
        float platformYRng = 1f, upperYLimit = 6f;
        int upOrDown = 1;
        float randomYOffset = 1f; 
        
        float[] randomXOffsetRange = new float[]{2.5f, 6.5f};
        float randomXOffset = UnityEngine.Random.Range(randomXOffsetRange[0], randomXOffsetRange[1]);
        
        randomYOffset = (1 - (randomXOffset / randomXOffsetRange[1])) * upperYLimit;
        
        if (_platformsToDraw[platformIndex].Item4.y >= 0)
        {
            platformYRng = UnityEngine.Random.Range(0f, 1f);
            if (platformYRng < platformYPlacementChance)
            {
                upOrDown = -1;
            }
            else
            {
                upOrDown = 1;
                if (_platformsToDraw[platformIndex].Item4.y + randomYOffset > ceilingLimit - 1f)
                {
                    randomYOffset = (ceilingLimit - _platformsToDraw[platformIndex].Item4.y) / 3;
                }
            }
        }
        else
        {
            platformYPlacementChance = _platformsToDraw[platformIndex].Item4.y / groundLimit;
            platformYRng = UnityEngine.Random.Range(0f, 1f);
            if (platformYRng < platformYPlacementChance)
            {
                upOrDown = 1;
            }
            else
            {
                upOrDown = -1;

                if (_platformsToDraw[platformIndex].Item4.y + randomYOffset * -1 < groundLimit)
                {
                    randomYOffset = (_platformsToDraw[platformIndex].Item4.y - groundLimit) / 2;
                }
            }
        }

        if (_platformsToDraw[platformIndex].Item3 == PlatformAttributes.PlatformType.StartingLeaf)
        {
            randomYOffset = UnityEngine.Random.Range(2f, 3f);
            randomXOffset = UnityEngine.Random.Range(3f, 5f);
        }
        else if (_platformsToDraw[platformIndex].Item3 == PlatformAttributes.PlatformType.Moving)
        {
            randomYOffset = UnityEngine.Random.Range(2f, 3f);
            randomXOffset = UnityEngine.Random.Range(3f, 5f);
        }

        if (upOrDown > 0 && randomYOffset >= 3f)
        {
            _platformsToDraw[platformIndex] =
                new Tuple<string, Tuple<float, float>, PlatformAttributes.PlatformType, Vector3, Sprite, int, Vector3>(
                    $"Updated to bouncy {_platformsToDraw[platformIndex].Item1}",
                    _platformsToDraw[platformIndex].Item2,
                    PlatformAttributes.PlatformType.Bouncy,
                    _platformsToDraw[platformIndex].Item4,
                    GameManager.Instance.globalData.platformSprites[2],
                    _platformsToDraw[platformIndex].Item6,
                    _platformsToDraw[platformIndex].Item7
                );
        }
        else if (upOrDown < 0 && randomYOffset >= 3f)
        {
            typeOfCreatedPlatform = PlatformAttributes.PlatformType.Bouncy;
        }

        if (typeOfCreatedPlatform == PlatformAttributes.PlatformType.Regular)
        {
            float typeRandomizer = UnityEngine.Random.Range(0f, 1f);
            if (typeRandomizer >= 0.75f)
                typeOfCreatedPlatform = PlatformAttributes.PlatformType.Snowing;
            else if (typeRandomizer >= 0.5f && _platformsToDraw[platformIndex].Item3 != PlatformAttributes.PlatformType.Moving)
            {
                typeOfCreatedPlatform = PlatformAttributes.PlatformType.Moving;
            }
        }

        if (leftOrRight == 2)
        {
            cloudPosition = new Vector3(_platformsToDraw[platformIndex].Item4.x + (_platformsToDraw[platformIndex].Item2.Item1 / 2) + randomPlatformLength / 2 + randomXOffset, 
                _platformsToDraw[platformIndex].Item4.y + randomYOffset * upOrDown, 0);
        }
        else
        {
            cloudPosition = new Vector3(_platformsToDraw[platformIndex].Item4.x - (_platformsToDraw[platformIndex].Item2.Item1 / 2) - randomPlatformLength / 2 - randomXOffset, 
                _platformsToDraw[platformIndex].Item4.y + randomYOffset * upOrDown, 0);
        }

        Sprite platformSprite = null;

        switch (typeOfCreatedPlatform)
        {
            case PlatformAttributes.PlatformType.Regular:
                platformSprite = GameManager.Instance.globalData.platformSprites[1];
                break;
            case PlatformAttributes.PlatformType.Bouncy:
                platformSprite = GameManager.Instance.globalData.platformSprites[2];
                break;
            case PlatformAttributes.PlatformType.Snowing:
                platformSprite = GameManager.Instance.globalData.platformSprites[3];
                break;
            case PlatformAttributes.PlatformType.Moving:
                platformSprite = GameManager.Instance.globalData.platformSprites[4];
                savedPos = cloudPosition;
                if(cloudPosition.x > 0)
                   cloudPosition.x += UnityEngine.Random.Range(7f, 15f);
                else
                    cloudPosition.x -= UnityEngine.Random.Range(7f, 15f);
                cloudPosition.y -= -1;
                randomPlatformLength += 1f;;
                var targetGhost = Instantiate(ghostPlatform, cloudPosition, quaternion.identity, ghostPlatformParent);
                targetGhost.transform.localScale = new Vector3(randomPlatformLength / 3, 1f, 0);
                var otherGhost = Instantiate(ghostPlatform, savedPos, quaternion.identity, ghostPlatformParent);
                otherGhost.transform.localScale = new Vector3(randomPlatformLength / 3, 1f, 0);
                break;
        }

        _platformsToDraw.Add(new Tuple<string, Tuple<float, float>, PlatformAttributes.PlatformType, Vector3, Sprite, int, Vector3>(
            $"{typeOfCreatedPlatform}-{platformName}", 
            new Tuple<float, float>(randomPlatformLength / 3, 1f),
            typeOfCreatedPlatform, 
            cloudPosition,
            platformSprite, 
            leftOrRight,
            savedPos
            ));

        switch (leftOrRight)
        {
            case 1:
                _platformsOnTheLeft.Add(_platformsToDraw.Count);
                vacantNodes.Add(new Tuple<int, int>( _platformsToDraw.Count - 1, 1));
                _leftFinal = cloudPosition;
                break;
            case 2:
                _platformsOnTheRight.Add(_platformsToDraw.Count);
                vacantNodes.Add(new Tuple<int, int>( _platformsToDraw.Count - 1, 2));
                _rightFinal = cloudPosition;
                break;
        }
    }

    public void DrawPlatform(string platformName, float length, float height, PlatformAttributes.PlatformType platformType,
        Vector3 platformPosition, Sprite sprite, int leftOrRight, Vector3 savedPos){
        
        GameObject createdPlatformObject = Instantiate(platform, platformsParent);
        Transform createdPlatformTransform = createdPlatformObject.GetComponent<Transform>();
        Platform createdPlatform = createdPlatformObject.GetComponent<Platform>();
        
        PlatformAttributes platformAttributes = ScriptableObject.CreateInstance<PlatformAttributes>();
        
        createdPlatformObject.name = platformName;
        
        platformAttributes.length = length;
        platformAttributes.position = new Vector2(platformPosition.x, platformPosition.y);
        platformAttributes.platformType = platformType;

        createdPlatformTransform.position = platformPosition;
        createdPlatformTransform.localScale = new Vector3(length, height, 0);
        SpriteRenderer spriteRenderer = createdPlatformObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        float enemyRandomizer = 0;
        
           switch(platformType){
            case PlatformAttributes.PlatformType.Bouncy:
                createdPlatformObject.name = $"Created as bouncy {platformName}";
                createdPlatformObject.layer = 8;
                platformAttributes.bounciness = bouncyPlatformBouncePower;
                BoxCollider2D boxCollider2D = createdPlatformObject.GetComponent<BoxCollider2D>();
                boxCollider2D.isTrigger = true;
                BouncyPlatform bouncyPlatform = createdPlatformObject.AddComponent<BouncyPlatform>();
                bouncyPlatform.bouncePower = platformAttributes.bounciness;
                enemyRandomizer = UnityEngine.Random.Range(0f, 1f);
                if(enemyRandomizer >= 0.75f)
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(1, new Vector3(platformPosition.x, platformPosition.y + 1f, platformPosition.z), createdPlatformTransform));
                break;
            case PlatformAttributes.PlatformType.Moving:
                MovingPlatform movingPlatform = createdPlatformObject.AddComponent<MovingPlatform>();
                movingPlatform.targetPoint = platformPosition;
                movingPlatform.otherPoint = new Vector3(savedPos.x, savedPos.y, 0);
                movingPlatform.platformWaitTime = 1.5f;
                movingPlatform.platformSpeed = 20f / Mathf.Abs(platformPosition.x - savedPos.x);
                enemyRandomizer = UnityEngine.Random.Range(0f, 1f);
                if(enemyRandomizer >= 0.75f && length >= 1f)
                {
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x - length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                    if(length >= 1.7f)
                       _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x + length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                }
                else if(enemyRandomizer >= 0.5f && length >= 1f)
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(1, new Vector3(platformPosition.x, platformPosition.y + 1f, platformPosition.z), createdPlatformTransform));
                break;
            case PlatformAttributes.PlatformType.Regular:
                enemyRandomizer = UnityEngine.Random.Range(0f, 1f);
                if(enemyRandomizer >= 0.75f && length >= 1f)
                {
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x - length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                    if(length >= 1.7f)
                       _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x + length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                }
                else if(enemyRandomizer >= 0.5f && length >= 1f)
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(1, new Vector3(platformPosition.x, platformPosition.y + 1f, platformPosition.z), createdPlatformTransform));
                break;
            case PlatformAttributes.PlatformType.Snowing:
                enemyRandomizer = UnityEngine.Random.Range(0f, 1f);
                if(enemyRandomizer >= 0.75f && length >= 1f)
                {
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x - length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                    if(length >= 1.7f)
                       _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x + length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                }
                else if(enemyRandomizer >= 0.5f && length >= 1f)
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(1, new Vector3(platformPosition.x, platformPosition.y + 1f, platformPosition.z), createdPlatformTransform));
                break;
            case PlatformAttributes.PlatformType.Raining:
                enemyRandomizer = UnityEngine.Random.Range(0f, 1f);
                if(enemyRandomizer >= 0.75f && length >= 1f)
                {
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x - length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                    if(length >= 1.7f)
                        _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(0,
                        new Vector3(platformPosition.x + length / 2, platformPosition.y + 0.85f, platformPosition.z),
                        createdPlatformTransform));
                }
                else if(enemyRandomizer >= 0.5f && length >= 1f)
                    _entitiesToSpawn.Add(new Tuple<int, Vector3, Transform>(1, new Vector3(platformPosition.x, platformPosition.y + 1f, platformPosition.z), createdPlatformTransform));
                break;
            case PlatformAttributes.PlatformType.StartingLeaf:
            case PlatformAttributes.PlatformType.NonExistent:
                break;
           }

        createdPlatform.platformAttributes = platformAttributes;

        switch (leftOrRight)
        {
            case 1:
                vacantNodes.Add(new Tuple<int, int>( _platformsToDraw.Count, 1));
                _leftFinal = createdPlatformTransform.position;
                break;
            case 2:
                vacantNodes.Add(new Tuple<int, int>( _platformsToDraw.Count, 2));
                _rightFinal = createdPlatformTransform.position;
                break;
        }
        
        platformGameObjects.Add(createdPlatformObject);
    }
}