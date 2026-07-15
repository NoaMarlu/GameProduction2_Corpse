using UnityEngine;
using DG.Tweening;

public class CharacterVisual : MonoBehaviour
{

    //ストレッチ情報
    public float stretchX = 1.3f;//移動方向伸び
    public float stretchY = 0.7f;//移動方向縮み
    public float squashX = 0.7f;//着地時縮み
    public float squashY = 1.3f;//着地時伸び
    public float stretchDuration = 0.08f;//移動中の時間
    public float squashDuration = 0.06f;//着地中の時間
    public float returnDuration = 0.1f;//元の大きさに戻る時間
    //シークエンス
    private Sequence currentSequence;

    //エフェクト
    public ParticleSystem dustParticle;//砂埃のエフェクト

    //点滅
    private SpriteRenderer spr;

    void Awake()
    {
        spr = GetComponent<SpriteRenderer>();    
    }

    //移動開始時
    public void PlayMoveStretch(Vector2Int direction)
    {
        //実行中のシークエンスがあれば停止
        currentSequence?.Kill();
        transform.localScale = Vector3.one;

        //移動方向に応じて伸び縮み
        Vector3 stretchScale;
        Vector3 squashScale;

        if (direction.x != 0)//左右移動
        {
            stretchScale = new Vector3(stretchX, stretchY, 1f);
            squashScale = new Vector3(squashX, squashY, 1f);
        }
        else//上下移動
        {
            stretchScale = new Vector3(stretchY, stretchX, 1f);
            squashScale = new Vector3(squashY, squashX, 1f);
        }

        //シークエンス作成
        currentSequence = DOTween.Sequence();
        currentSequence
            .Append(transform.DOScale(stretchScale, stretchDuration).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(squashScale, squashDuration).SetEase(Ease.InQuad))
            .Append(transform.DOScale(Vector3.one, returnDuration).SetEase(Ease.OutElastic));

        //砂埃エフェクト
        currentSequence.InsertCallback(stretchDuration, PlayDust);
    }
    //砂埃エフェクト
    void PlayDust()
    {
        if (dustParticle == null) return;
        dustParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dustParticle.Play();
    }
    //点滅
    public void PlayHitEffect()
    {
        //実行中の点滅があればキャンセル
        spr.DOKill(spr);
        //点滅
        spr.DOFade(0f, 0.08f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.Linear).SetTarget(spr).OnComplete(() =>
            {
                Color c = spr.color;
                c.a = 1f;
                spr.color = c;
            });
    }

}
