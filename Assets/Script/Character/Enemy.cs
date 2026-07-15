using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{

    //基本情報
    public int gridX;
    public int gridY;
    public Vector2Int lastDirection;

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

    //スプライト用
    private CharacterVisual visual;
    private SpriteRenderer spr;
    private Animator animator;
    public Sprite dieSpr;

    //SE
    private AudioSource audioSource;
    public AudioClip damageClip;

    void Awake()
    {
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

        //方向にあわせてスプライトを変更
        if (lastDirection.x != 0) GetComponent<SpriteRenderer>().flipX = lastDirection.x > 0;

        //移動アニメーション
        visual?.PlayMoveStretch(lastDirection);
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

    /*遺体関連*/
    //遺体処理
    void ChangeCorpse() 
    {
        if (isCorpse) return;
        //壁判定
        var Cell = GridManager.Instance.GetCell(gridX, gridY);
        if(Cell != null)Cell.isWalk = false;
        //効果設定
        CorpseEffect();
        //遺体フラグ
        isCorpse = true;
        //見た目関連
        if (dieSpr != null) spr.sprite = dieSpr;
        if(animator != null)animator.enabled = false;
        //SE
        audioSource.PlayOneShot(damageClip);
        //移動停止
        TurnManager.Instance.RemoveEnemy(this);
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
                int targetX = initGridX + dx;
                int targetY = initGridY + dy;
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
        if(animator != null)animator.enabled = true;
        //ターンマネージャーに登録
        TurnManager.Instance.AddEnemy(this);
    }
    //遺体判定を返す
    public bool IsCorpse() { return isCorpse;  }

}
