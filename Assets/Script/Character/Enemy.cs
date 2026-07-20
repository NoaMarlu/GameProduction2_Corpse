using UnityEngine;
using System.Collections.Generic;


public class Enemy : MonoBehaviour
{

    //基本情報
    public int gridX;
    public int gridY;
    public Vector2Int lastDirection;
    private Vector2Int decideDirection;
    public Stage currentStage;

    public int level = 0;//レベルが高いと実行が早い

    //Inspector上で指定する移動モード
    public enum MoveMode 
    {
        None,         //移動なし
        RightToLeft,    //左右移動（右優先）
        UpToDown,       //上下移動（上優先）
    }
    public MoveMode moveMode;

    //Inspector上で指定する遺体の効果
    [System.Flags]
    public enum CorpseMode 
    {
        None    =0,//遺体効果なし
        Weight  =1<<0,//重し効果
        Decay =1<<1,//周囲1マスを腐敗
    }
    public CorpseMode corpseMode;

    //リセット用
    private int initGridX;//初期位置
    private int initGridY;
    private Vector2Int initDirection;//初期方向
    private bool isCorpse = false;

    //腐敗用
    public GameObject decayPrefab;//腐敗しているマス用のプレハブ
    private List<GameObject> decayObjects = new List<GameObject>();
    private int corpseGridX;//遺体化した座標
    private int corpseGridY;

    //スプライト用
    private CharacterVisual visual;
    private SpriteRenderer spr;
    private Sprite initSpr;
    private Animator animator;
    public Sprite dieSpr;

    //SE
    private AudioSource audioSource;
    public AudioClip damageClip;

    void Awake()
    {
        initSpr = GetComponent<SpriteRenderer>().sprite;
        audioSource = GetComponent<AudioSource>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //数値設定
        switch (moveMode)
        {
            case MoveMode.None:   /*処理なし*/    break;
            case MoveMode.RightToLeft: lastDirection =new Vector2Int(1,0); break;
            case MoveMode.UpToDown: lastDirection = new Vector2Int(0, 1); break;
        }
        visual = GetComponent<CharacterVisual>();
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
    //矢の衝突時に呼ぶ
    public void HitArrow()
    {
        ChangeCorpse();
    }

    /*遺体関連*/
    //遺体処理
    void ChangeCorpse() 
    {
        if (isCorpse) return;
        //遺体フラグ
        isCorpse = true;
        //遺体化した位置の記録（腐敗用）
        corpseGridX = gridX;
        corpseGridY = gridY;
        //移動停止
        TurnManager.Instance.RemoveEnemy(this);
        //SE
        audioSource.PlayOneShot(damageClip);
        //設定変更
        ApplyCorpseLogic();
        //アニメーション開始
        if (animator != null) animator.SetTrigger("Die");
        else ApplyCorpseVisual();
    }
    //見た目を遺体にする
    public void ApplyCorpseVisual() {  if (dieSpr != null) spr.sprite = dieSpr;  }
    //設定遺体化
    public void ApplyCorpseLogic()
    {
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) cell.isWalk = false;
        CorpseEffect();
    }
    //遺体効果
    void CorpseEffect()
    {
        if ((corpseMode & CorpseMode.Weight) != 0) ApplyWeight();
        if ((corpseMode & CorpseMode.Decay) != 0) ApplyDecay();
    }
    //セルに重しを適用
    void ApplyWeight()
    {
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) cell.type |= GridManager.GridType.Weight;
    }
    //セルに腐敗を適用
    void ApplyDecay()
    {
        //周囲8マスの3*3範囲
        for(int dx = -1;dx <= 1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                //位置情報取得
                int targetX = gridX + dx;
                int targetY = gridY + dy;
                var cell = GridManager.Instance.GetCell(targetX, targetY);
                if (cell == null) continue;

                cell.type |= GridManager.GridType.Decay;
                if(decayPrefab != null)
                {
                    //オブジェクトの生成
                    Vector3 worldPos = GridManager.Instance.GridToWorld(targetX, targetY);
                    GameObject decayObj = Instantiate(decayPrefab, worldPos, Quaternion.identity);
                    decayObjects.Add(decayObj);
                }
            }
        }
    }
    void RemoveDecay()
    {
        //腐敗してなかったらreturn
        if ((corpseMode & CorpseMode.Decay) == 0) return;

        //腐敗フラグを消す
        for(int dx = -1;dx <= 1; dx++)
        {
            for(int dy = -1;dy <= 1; dy++)
            {
                int targetX = corpseGridX + dx;
                int targetY = corpseGridY + dy;
                var cell = GridManager.Instance.GetCell(targetX, targetY);
                if(cell != null)cell.type &= ~GridManager.GridType.Decay;
            }
        }

        //生成した腐敗オブジェクトを削除する
        foreach(var obj in decayObjects)
        {
            if (obj != null) Destroy(obj);
        }
        decayObjects.Clear();

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
            if (corpseCell != null) corpseCell.type &= ~GridManager.GridType.Weight;
            RemoveDecay();
        }
        isCorpse = false;
        //位置
        gridX = initGridX;
        gridY = initGridY;
        lastDirection = initDirection;
        SnapToGrid();
        //見た目関連
        if (spr != null) spr.sprite = initSpr;
        //アニメーションを最初から
        if(animator != null)
        {
            animator.enabled = true;
            animator.ResetTrigger("Die");
            animator.Rebind();
            animator.Update(0f);
        }
        //ターンマネージャーに登録
        TurnManager.Instance.AddEnemy(this);
    }
    //遺体判定を返す
    public bool IsCorpse() { return isCorpse;  }
    //進むますにエネミーがいるか判定
    bool IsOtherEnemy(int x,int y)
    {
        foreach(var enemy in TurnManager.Instance.GetEnemies())
        {
            if (enemy == this) continue;
            if (enemy.gridX == x & enemy.gridY == y) return true;
        }
        return false;
    }
    //移動先の計算のみ
    public Vector2Int DecideMove()
    {
        Vector2Int dir = lastDirection;
        int targetX = gridX + dir.x;
        int targetY = gridY + dir.y;
        var targetCell = GridManager.Instance.GetCell(targetX, targetY);

        bool blocked = (targetCell == null || !targetCell.isWalk || IsOtherEnemy(targetX,targetY));

        if (blocked)
        {
            if (moveMode == MoveMode.RightToLeft) dir = new Vector2Int(-dir.x, 0);
            else if (moveMode == MoveMode.UpToDown) dir = new Vector2Int(0, -dir.y);

            targetX = gridX + dir.x;
            targetY = gridY + dir.y;
            targetCell = GridManager.Instance.GetCell(targetX,targetY);

            blocked = (targetCell == null || !targetCell.isWalk || IsOtherEnemy(targetX, targetY));
            if (blocked){
                decideDirection = lastDirection;
                return new Vector2Int(gridX, gridY); 
            }
        }
        decideDirection = dir;
        return new Vector2Int(targetX,targetY);
    }
    //移動を確定
    public void ConfilmMove(Vector2Int confirmPos)
    {
        if (confirmPos.x == gridX && confirmPos.y == gridY) return;
        lastDirection = decideDirection;
        gridX = confirmPos.x; 
        gridY = confirmPos.y;
        SnapToGrid();

        if (lastDirection.x != 0) spr.flipX = lastDirection.x > 0;
        visual?.PlayMoveStretch(lastDirection);
    }
    public void ConfilmDirectionOnly() { lastDirection = decideDirection; }


}
