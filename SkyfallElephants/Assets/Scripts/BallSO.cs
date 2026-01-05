using UnityEngine;

[CreateAssetMenu(fileName = "BallSO", menuName = "ScriptableObjects/BallSO", order = 1)]
public class BallSO : ScriptableObject
{
    public GameObject ballPf;
    public int pointValue = 1;
}
