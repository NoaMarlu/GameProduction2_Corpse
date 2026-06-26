using UnityEngine;
using static GridManager;

public class Player : MonoBehaviour
{

    //グリッド情報
    GridManager gridManager;
    public int gridX;
    public int gridY;

    //入力処理
    public Vector2Int lastDirection;//最後の入力方向

    void Start()
    {
        TurnManager.Instance.SetPlayer(this);
        SnapToGrid();
    }

    //座標の設定
    void SnapToGrid() { transform.position = gridManager.GridToWorld(gridX, gridY); }
    //移動入力処理
    public void PlayerInput(Vector2Int direction)
    {
        Vector2Int dir = Vector2Int.zero;
        //入力
        if (Input.GetKeyDown(KeyCode.W)) { dir = Vector2Int.up; }
        if (Input.GetKeyDown(KeyCode.A)) { dir = Vector2Int.left; }
        if (Input.GetKeyDown(KeyCode.S)) { dir = Vector2Int.down; }
        if (Input.GetKeyDown(KeyCode.D)) { dir = Vector2Int.right; }

        //directionが変わったら通知
        if(dir != Vector2Int.zero)
        {
            TurnManager.Instance.isPlayerInput(dir);
        }
    }
    //左右上下のセルに移動可能かを取得
    public bool CanMove(Vector2Int direction)
    {
        //セル情報の取得
        int targetX = gridX + direction.x;
        int targetY = gridY + direction.y;
        Cell targetCell = gridManager.GetCell(targetX, targetY);

        if (targetCell == null || !targetCell.isWalk) return false;
        return true;
    }
    //移動方向の保存
    public void SetMoveDirection(Vector2Int direction) { lastDirection = direction; }
    //移動の実行
    public void PlayerMove()
    {
        int targetX = gridX + lastDirection.x;
        int targetY = gridY + lastDirection.y;

        //再度判定を取る（ダメージ判定のない壁になる敵が出てきた時用）
        var targetCell = gridManager.GetCell(targetX, targetY);
        if (targetCell == null || !targetCell.isWalk)
        {
            MoveCancel(targetCell);
            return;
        }

        //位置変更
        gridX = targetX;
        gridY = targetY;
        SnapToGrid();

    }

    private void MoveCancel(GridManager.Cell cancelCell)
    {
        //ダメージ処理などの分岐は後で書く
        Debug.Log("移動がキャンセルされました");
    }

}
