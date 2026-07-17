using UnityEngine;

public class Switch : MonoBehaviour
{

    public bool isOn = false;

    private int gridX;
    private int gridY;

    //スプライト
    private SpriteRenderer spr;
    public Sprite ONspr;
    public Sprite OFFspr;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        //位置設定
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);

        TurnManager.Instance.AddSwitch(this);
    }
    void Update()
    {
        ChangeSprite();    
    }

    //スイッチの状態チェック
    public void CheckSwitch()
    {
        bool OnTop = false;
        var player = TurnManager.Instance.GetPlayer();
        
        //プレイヤーがスイッチにいたら
        if(player.gridX == gridX && player.gridY == gridY)OnTop = true;

        //敵がスイッチにいたら
        foreach(var enemy in TurnManager.Instance.GetEnemies())
        {
            if (enemy.gridX == gridX && enemy.gridY == gridY) 
            {
                OnTop = true;
                break;
            }
        }

        //遺体がスイッチに乗ってたら
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null && (cell.type & GridManager.GridType.Weight) != 0) OnTop = true;

        isOn = OnTop;

    }

    void ChangeSprite()
    {
        if (isOn) spr.sprite = ONspr;
        else spr.sprite = OFFspr;
    }

}
