using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VectorGraphics;

public class BossManager : MonoBehaviour
{

    public static BossManager Instance { get; private set; }

    public Boss boss;
    public List<Stage> puzzleStage;//フェーズごとに出現させるパズル
    private int currentPhase = 0;
    private Stage activeStage;

    //アニメーション
    private bool isFinalHit = false;
    public float bossAnimeSpeed = 1.5f;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }    
        Instance = this;
    }

    void Start()
    {
        ApplyBossAnimeSpeed();
        boss.SetMode(Boss.BossMode.Opening);
    }

    //フェーズの開始
    public void OpeningFinished() { StartPhase(0); }
    void StartPhase(int index)
    {
        currentPhase = index;
        boss.SetMode(Boss.BossMode.Play);

        if (index < puzzleStage.Count)
        {
            ActivatePuzzle(puzzleStage[index]);
        }
    }
    void ActivatePuzzle(Stage stage)
    {
        if (activeStage != null) activeStage.InactiveStage();
        foreach(var ps in puzzleStage)
        {
            ps.ResetStage();
            ps.gameObject.SetActive(false); 
        }
        stage.gameObject.SetActive(true);
        activeStage = stage;
        stage.ActiveStage();
        stage.WarpPlayerInit(TurnManager.Instance.GetPlayer());
        //アニメーション速度も変更
        ApplyBossAnimeSpeed();
    }
    //プレイヤーが動くたびに呼ばれる
    public void PlayerMove()
    {
        boss.OnPlayerMove();
    }
    public void OnBossDamaged(bool hit)
    {
        if(activeStage != null)
        {
            activeStage.InactiveStage();
            activeStage = null;
        }
        isFinalHit = hit;
    }
    public void DamageFinished()
    {

        if (isFinalHit)
        {
            //死亡演出と死亡処理
            return;
        }
        currentPhase++;
        if (currentPhase < puzzleStage.Count)
        {
            StartPhase(currentPhase);
        }
    }
    //各アニメーションの速度を変更する
    void ApplyBossAnimeSpeed()
    {
        TurnManager.Instance.GetPlayer().SetAnimatorSpeed(bossAnimeSpeed);
        foreach(var enemy in TurnManager.Instance.GetEnemies())
        {
            enemy.SetAnimatorSpeed(bossAnimeSpeed);
        }
    }

}
