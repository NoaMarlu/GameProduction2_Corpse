using UnityEngine;

public class BossWeakPoint : MonoBehaviour
{
    public Boss boss;
    private int gridX, gridY;
    public bool isActive = true;

    void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;  
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);
    }

    //Ž©•ª‚ÌˆÊ’u‚ð•Ô‚·
    public bool IsAtPosition(int x, int y)
    {
        return gridX == x && gridY == y;
    }
    public void HitArrow()
    {
        if (!isActive) return;
        boss.OnWeakPointHit();
    }

}
