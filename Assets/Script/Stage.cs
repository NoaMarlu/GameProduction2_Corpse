using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{

    public int stageID;

    [HideInInspector] public List<Enemy> stageEnemies = new List<Enemy>();
    [HideInInspector] public Player stagePlayer;

    public bool isActive;//ステージがアクティブか

    void Awake()
    {
        //エネミーはステージの子オブジェクトにする
        stageEnemies.AddRange(GetComponentsInChildren<Enemy>());
    }

    //ステージをアクティブにする
    public void ActiveStage()
    {
        isActive = true;
        foreach (var enemy in stageEnemies)
        {
            enemy.SetActive(true);
        }
    }
    //ステージを非アクティブにする
    public void InactiveStage()
    {
        isActive = false;
        foreach (var enemy in stageEnemies)
        {
            enemy.EnemyReset();
        }
    }
    //リセット呼び出し
    public void ResetStage()
    {
        foreach (var enemy in stageEnemies)
        {
            enemy.EnemyReset();
        }
        if (stagePlayer != null)
        {
            stagePlayer.PlayerReset();
        }
    }

}