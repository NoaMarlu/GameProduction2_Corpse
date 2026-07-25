using UnityEngine;

public class ClearBlock : MonoBehaviour
{


    public Stage targetStage;
    private int gridX;
    private int gridY;

    //スプライト用
    private SpriteRenderer spr;
    public Sprite blockSpr;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();  
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

    public void CheckBlock()
    {
        UpdateState();
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
