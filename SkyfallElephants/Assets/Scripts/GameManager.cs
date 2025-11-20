using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    [SerializeField] private int lives = 3;

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        lives = 3;
        ScoreManager.i.ResetScore();
    }
}
