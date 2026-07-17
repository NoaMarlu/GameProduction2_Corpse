using UnityEngine;
using DG.Tweening;

public class Switch : MonoBehaviour
{

    public bool isOn = false;
    private int gridX;
    private int gridY;

    //スプライト
    private SpriteRenderer spr;
    public Sprite[] switchSprites;

    //アニメーション
    public float frameDuration = 0.05f;
    private float currentFrame = 0f;
    private Tween frameTween;

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

        currentFrame = 0f;
        UpdateSprite();
    }

    //スイッチの状態チェック
    public void CheckSwitch()
    {
        bool OnTop = false;
        var player = TurnManager.Instance.GetPlayer();

        //プレイヤーがスイッチにいたら
        if (player.gridX == gridX && player.gridY == gridY) OnTop = true;

        //敵がスイッチにいたら
        foreach (var enemy in TurnManager.Instance.GetEnemies())
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

        bool changed = (isOn != OnTop);
        isOn = OnTop;
        if (changed) PlayFrameAnimation();

    }

    //アニメーション
    void PlayFrameAnimation()
    {
        frameTween?.Kill();

        float targetFrame = isOn ? (switchSprites.Length - 1) : 0f;
        float distance = Mathf.Abs(targetFrame - currentFrame);
        float duration = distance * frameDuration;

        frameTween = DOTween.To(
            () => currentFrame,
            x => { currentFrame = x; UpdateSprite(); },
            targetFrame,
            duration
            ).SetEase(Ease.Linear);
    }
    void UpdateSprite()
    {
        if (switchSprites == null || switchSprites.Length == 0) return;
        int index = Mathf.RoundToInt(currentFrame);
        index = Mathf.Clamp(index, 0, switchSprites.Length - 1);
        spr.sprite = switchSprites[index];
    }

}
