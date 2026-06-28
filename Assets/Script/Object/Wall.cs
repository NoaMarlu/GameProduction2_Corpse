using UnityEngine;

public class Wall : MonoBehaviour
{

    private int gridX;
    private int gridY;

    void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        //セルを壁判定に変更
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) cell.isWalk = false;
        //位置変更
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);
    }

}
