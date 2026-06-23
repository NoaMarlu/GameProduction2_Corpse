using UnityEngine;
using static GridManager;

public class Player : MonoBehaviour
{
    GridManager gridManager;
    public int gridX;
    public int gridY;

    private void Start()
    {
        SnapToGrid();
    }

    //座標の設定
    void SnapToGrid() { transform.position = gridManager.GridToWorld(gridX, gridY); }
    //移動処理
    public bool PlayerMove(Vector2Int direction)
    {
        int targetX = gridX + direction.x;
        int targetY = gridY + direction.y;

        Cell targetCell = gridManager.GetCell(targetX, targetY);
        //歩けなかったらfalseを返すよん
        if (targetCell == null ||! targetCell.isWalk) return false;

        //位置変換
        gridX = targetCell.x;
        gridY = targetCell.y;
        SnapToGrid();

        //マネージャーに通知
        TurnManager.Instance.OnPlayerMoved();

        return true;

    }

}
