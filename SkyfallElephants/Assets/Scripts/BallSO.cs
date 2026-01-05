using UnityEngine;

[CreateAssetMenu(fileName = "BallSO", menuName = "ScriptableObjects/BallSO", order = 1)]
public class BallSO : ScriptableObject
{
    [Header("Behavior")]
    public BallBehavior behavior = BallBehavior.Normal;

    [Header("Prefab")]
    //public GameObject ballPf;
    public Color ballColor = Color.white;

    [Header("Scoring")]
    public int pointValue = 1;

    [Header("Physics")]
    public float gravityScale = 1.5f;
    public float maxHorizontalSpeed = 3f;
    public float horizontalAcceleration = 6f;

    [Header("Visual")]
    public float scaleMultiplier = 1f;

    //[Header("Special Behavior Parameters")]
    //public float behaviorTriggerHeight = 4f;
    //public float behaviorStrength = 1.4f;
}
public enum BallBehavior
{
    Normal,
    Heavy,
    Floater,
    LateDriftBoost,
    Split
}