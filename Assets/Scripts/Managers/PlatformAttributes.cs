using UnityEngine;


[CreateAssetMenu(fileName = "New Platform", menuName = "Platform", order = 0)]
public class PlatformAttributes : ScriptableObject
{
    public float length;
    public Vector2 position;
    
    public PlatformType platformType;
    public float bounciness;
    public enum PlatformType
    {
        Regular,
        Moving,
        Bouncy,
        Snowing,
        Raining,
        NonExistent,
        StartingLeaf
    }
}
