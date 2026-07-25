using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;

public class ClearBlock : MonoBehaviour
{


    public Stage targetStage;
    private int gridX;
    private int gridY;

    //スプライト用
    private SpriteRenderer spr;
    public Sprite blockSpr;

    //点滅
    public float blinkInterval = 0.1f;
    public int blinkCount = 6;
    public float showDuration = 0.8f;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        if (spr != null) spr.enabled = false;
    }
    void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);
        TurnManager.Instance.AddClearBlock(this);
        UpdateState();
    }
    //ターンごとに呼ばれてチェックする

    public void CheckBlock()
    {
        UpdateState();
    }
    //マスの状態を確認
    public bool IsPosition(int x, int y)
    {
        if (gridX == x && gridY == y) return true;
        else return false;
    }
    //点滅演出
    public void PlayFlash()
    {
        if (spr == null || blockSpr == null) return;
        spr.sprite= blockSpr;
        spr.DOKill();//実行中の演出が会ったら停止

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => spr.enabled = true);

        for(int i = 0;i<blinkCount; i++)
        {
            seq.AppendCallback(() => spr.enabled = !spr.enabled);
            seq.AppendInterval(blinkInterval);
        }
        seq.AppendCallback(() => spr.enabled = false);//最後は非表示
    }
    void UpdateState()
    {
        bool allDefeated = (targetStage != null && targetStage.IsAllEnemyDefeated());

        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if(cell != null)cell.isWalk = allDefeated;

        if(spr != null)
        {
            if (allDefeated) spr.sprite = null;
            else if (!allDefeated && blockSpr != null) spr.sprite = blockSpr;
        }

    }

}
