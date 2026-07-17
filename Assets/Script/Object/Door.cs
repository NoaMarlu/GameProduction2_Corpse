using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class Door : MonoBehaviour
{

    public List<Switch> linkSwitches = new List<Switch>();

    private int gridX;
    private int gridY;
    private bool isOpen = false;

    //スプライト
    private SpriteRenderer spr;
    public Sprite[] doorSprites;

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

        TurnManager.Instance.AddDoor(this);
        currentFrame = 0f;
        UpdateSprite();
        AddState(false);
    }

    //ドアの状態チェック
    public void CheckDoor()
    {
        bool isOpen = true;

        foreach(var linkSwitch in linkSwitches)
        {
            if (!linkSwitch.isOn)
            {
                isOpen = false;
                break;
            }
        }
        if (linkSwitches.Count == 0) isOpen = false;
        AddState(isOpen);
    }
    //オープンならセルの状態を変化
    void AddState(bool open)
    {
        bool changed = (isOpen != open);
        isOpen = open;

        var cell = GridManager.Instance.GetCell(gridX, gridY);

        if (cell != null) cell.isWalk = open;
        if (changed) PlayFrameAnimation();
    }
    //アニメーション再生
    void PlayFrameAnimation()
    {
        frameTween?.Kill();
        float targetFrame = isOpen ? (doorSprites.Length - 1) : 0f;

        float distance = Mathf.Abs(targetFrame - currentFrame);
        float duration = distance * frameDuration;

        frameTween = DOTween.To(
            () => currentFrame,
            x => { currentFrame = x; UpdateSprite(); },
            targetFrame,
            duration
            ).SetEase(Ease.Linear);

    }
    //currentFrameによってスプライトを変更
    void UpdateSprite()
    {
        if (doorSprites == null || doorSprites.Length == 0) return;
        int index = Mathf.RoundToInt(currentFrame);
        index = Mathf.Clamp(index, 0,doorSprites.Length - 1);
        spr.sprite = doorSprites[index];
    }

}
