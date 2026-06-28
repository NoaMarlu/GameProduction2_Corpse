using UnityEngine;

public class Enemy : MonoBehaviour
{

    //グリッド情報
    public int gridX;
    public int gridY;

    public Vector2Int lastDirection;

    //Inspector上で指定する移動モード
    public enum MoveMode 
    {
        None,         //移動なし
        RightToLeft,    //左右移動（右優先）
        UpToDown,       //上下移動（上優先）
    }
    public MoveMode moveMode;

    //Inspector上で指定する遺体の効果
    public enum CorpseMode 
    {
        None,  //遺体効果なし
        Weight,//重し効果
    }
    public CorpseMode courpseMode;

    //リセット用
    private int initGridX;//初期位置
    private int initGridY;
    private Vector2Int initDirection;//初期方向
    private bool isCorpse = false;

    void Awake()
    {
        //数値設定
        switch (moveMode)
        {
            case MoveMode.None:   /*処理なし*/    break;
            case MoveMode.RightToLeft: lastDirection =new Vector2Int(1,0); break;
            case MoveMode.UpToDown: UpToDownFunc(); break;
        }
    }
    void Start()
    {
        SetPos();
        //リセット用
        initGridX = gridX;
        initGridY = gridY;
        initDirection = lastDirection;
    }

    //位置設定
    void SetPos()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
    }
    //位置変更
    void SnapToGrid(){  transform.position  =GridManager.Instance.GridToWorld(gridX, gridY); }
    //移動処理
    public void EnemyMove()
    {
        //移動モードにより動作を変更
        switch (moveMode) 
        {
            case MoveMode.None:   /*処理なし*/    break;
            case MoveMode.RightToLeft:  RightToLeftFunc();  break;
            case MoveMode.UpToDown:   UpToDownFunc();   break;
        }
    }

    //移動系関数
    void RightToLeftFunc()
    {
        //ターゲット検索
        int targetX = gridX + lastDirection.x;
        int targetY = gridY + lastDirection.y;
        var targetCell = GridManager.Instance.GetCell(targetX, targetY);

        //進めないなら方向転換
        if(targetCell == null || !targetCell.isWalk)
        {
            lastDirection = new Vector2Int(-lastDirection.x, 0);
            //ターゲット再検索
            targetX = gridX + lastDirection.x;
            targetY = gridY + lastDirection.y;
            targetCell = GridManager.Instance.GetCell(targetX, targetY);
            //再検索後も進めないならreturn
            if(targetCell == null || !targetCell.isWalk)return;
        }
        gridX = targetX;
        gridY = targetY;
        SnapToGrid();
    }
    void UpToDownFunc()
    {
        //ターゲット検索
        int targetX = gridX + lastDirection.x;
        int targetY = gridY + lastDirection.y;
        var targetCell = GridManager.Instance.GetCell(targetX, targetY);

        //進めないなら方向転換
        if (targetCell == null || !targetCell.isWalk)
        {
            lastDirection = new Vector2Int(0,-lastDirection.y);
            //ターゲット再検索
            targetX = gridX + lastDirection.x;
            targetY = gridY + lastDirection.y;
            targetCell = GridManager.Instance.GetCell(targetX, targetY);
            //再検索後も進めないならreturn
            if (targetCell == null || !targetCell.isWalk) return;
        }
        gridX = targetX;
        gridY = targetY;
        SnapToGrid();
    }
    //矢の衝突時に呼ぶ
    public void HitArrow()
    {
        ChangeCorpse();
    }
    //遺体処理
    void ChangeCorpse() 
    {
        //Prototype用
        GetComponent<SpriteRenderer>().color = Color.black;
        //壁判定
        var Cell = GridManager.Instance.GetCell(gridX, gridY);
        if(Cell != null)Cell.isWalk = false;
        //効果設定
        CorpseEffect();
        //遺体フラグ
        isCorpse = true;
        //移動停止
        TurnManager.Instance.RemoveEnemy(this);
    }
    //遺体効果
    void CorpseEffect()
    {
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        switch (courpseMode) 
        {
            case CorpseMode.None:   break;
            case CorpseMode.Weight:
                if (cell != null) cell.type |= GridManager.GridType.Weight;
                break;
        }
    }
    //ステージのアクティブ管理
    public void SetActive(bool active)
    {
        if (isCorpse) return;
        if(active)TurnManager.Instance.AddEnemy(this);
        else TurnManager.Instance.RemoveEnemy(this);
    }
    //リセット
    public void EnemyReset()
    {
        //遺体だったら
        if (isCorpse)
        {
            var corpseCell = GridManager.Instance.GetCell(gridX, gridY);
            if (corpseCell != null) corpseCell.isWalk = true;
        }
        isCorpse = false;
        //位置
        gridX = initGridX;
        gridY = initGridY;
        lastDirection = initDirection;
        SnapToGrid();
        //ターンマネージャーに登録
        TurnManager.Instance.AddEnemy(this);
    }
}
