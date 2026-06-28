using UnityEngine;
using UnityEngine.InputSystem;
using static GridManager;

public class Player : MonoBehaviour
{

    //グリッド情報
    public int gridX;
    public int gridY;

    //入力処理
    public Vector2Int lastDirection;//最後の入力方向

    //リセット用
    private int initGridX;
    private int initGridY;

    void Start()
    {
        TurnManager.Instance.SetPlayer(gameObject.GetComponent<Player>());
        //リセット用
        initGridX = gridX;
        initGridY = gridY;
        SetPos();
        SnapToGrid();
    }
    void Update()
    {
        PlayerInput();
    }

    //座標の設定
    void SnapToGrid() { transform.position = GridManager.Instance.GridToWorld(gridX, gridY); }
    //移動入力処理
    public void PlayerInput()
    {
        Vector2Int dir = Vector2Int.zero;
        //入力
        if (Keyboard.current.wKey.wasPressedThisFrame) { dir = Vector2Int.up; }
        if (Keyboard.current.aKey.wasPressedThisFrame) { dir = Vector2Int.left; }
        if (Keyboard.current.sKey.wasPressedThisFrame) { dir = Vector2Int.down; }
        if (Keyboard.current.dKey.wasPressedThisFrame) { dir = Vector2Int.right; }

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
        Cell targetCell = GridManager.Instance.GetCell(targetX, targetY);

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
        var targetCell = GridManager.Instance.GetCell(targetX, targetY);
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
    //エネミー実行後に移動をキャンセルする場合
    private void MoveCancel(GridManager.Cell cancelCell)
    {
        //ダメージ処理などの分岐は後で書く
        Debug.Log("移動がキャンセルされました");
    }
    //位置を綺麗に修正
    void SetPos()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
    }
    //リセット
    public void PlayerReset()
    {
        gridX = initGridX;
        gridY = initGridY;
        lastDirection = Vector2Int.zero;
        SnapToGrid();
    }

}
