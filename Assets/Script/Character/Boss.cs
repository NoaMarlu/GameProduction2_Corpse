using UnityEngine;

public class Boss : MonoBehaviour
{

    public enum BossMode 
    {
        Opening,//登場演出
        Play,//パズル中
        Damage,//弱点を撃たれた時
        Die//討伐演出
    }
    public  BossMode mode = BossMode.Opening;

    //スプライト
    public string[] anime;
    private Animator animator;

    //弱点設定
    public int totalHit = 3;//何回で死亡するか
    private int currentHits = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //モードにあわせて処理を切り替え
    public void SetMode(BossMode newMode)
    {
        mode = newMode;
        switch (mode) 
        {
            case BossMode.Opening:PlayOpening(); break;
            case BossMode.Play:break;
            case BossMode.Damage:PlayDamage(); break;
            case BossMode.Die:PlayDie(); break;

        }
    }

    //登場演出
    void PlayOpening()
    {
        if(animator != null)animator.SetTrigger("Opening");
    }
    //弱点を撃たれた時の演出
    void PlayDamage()
    {
        if(animator != null)animator.SetTrigger("Damage");
    }
    //討伐演出
    void PlayDie()
    {
        if(animator != null)animator.SetTrigger("Die");
    }

    //プレイヤーが1マス動くたびに呼ばれる
    public void OnPlayerMove()
    {
        if (mode != BossMode.Play) return;
        if (anime == null || anime.Length == 0) return;
        animator.Play(anime[Random.Range(0, anime.Length)]);
    }

    public void OnWeakPointHit()
    {
        currentHits++;
        bool isDead = currentHits >= totalHit;
        SetMode(isDead ? BossMode.Die : BossMode.Damage);
        BossManager.Instance.OnBossDamaged(isDead);
    }
    //アニメーションから呼ぶイベント処理
    public void OnOpeningAnimeEnd() { BossManager.Instance.OpeningFinished(); }
    public void OnDamageAnimeEnd() { BossManager.Instance.DamageFinished(); }

}
