using UnityEngine;

public static class Mouse2D
{
    public static Vector2 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(vec.x, vec.y);
    }
}
