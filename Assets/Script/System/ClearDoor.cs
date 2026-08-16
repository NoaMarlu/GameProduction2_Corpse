using DG.Tweening;
using UnityEngine;

public class ClearDoor : MonoBehaviour
{

    public Stage targetStage; //クリア条件となるステージ
    private int gridX;
    private int gridY;
    private bool isOpen = false;

    //スプライト
    private SpriteRenderer spr;
    public Sprite[] doorSprites;
    public float frameDuration = 0.05f;

    private float currentFrame = 0f;
    private Tween frameTween;

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

        TurnManager.Instance.AddClearDoor(this);
        currentFrame = 0f;
        UpdateSprite();
        AddState(false);
    }

    //クリアチェック
    public void CheckDoor()
    {
        bool isCleared = (targetStage != null && targetStage.isCleared);

        if(isCleared != isOpen)
        {
            AddState(isCleared);
            PlayFrameAnimation();
        }
    }

    void AddState(bool open)
    {
        isOpen = open;
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) cell.isWalk = open;
    }
    void PlayFrameAnimation()
    {
        frameTween?.Kill();

        float targetFrame = isOpen ? (doorSprites.Length - 1) :0f;
        float distance = Mathf.Abs(targetFrame - currentFrame);
        float duration = distance * frameDuration;

        frameTween = DOTween.To(
            () => currentFrame,
            x => {
                currentFrame = x;
                UpdateSprite();
            },
            targetFrame,
            duration
            ).SetEase(Ease.Linear);
    }
    void UpdateSprite()
    {
        if (doorSprites == null || doorSprites.Length == 0) return;
        int index = Mathf.RoundToInt(currentFrame);
        index = Mathf.Clamp(index, 0, doorSprites.Length - 1);
        spr.sprite = doorSprites[index];
    }

}
