using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool isActive;

    public void Open()
    {
        isActive = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}
