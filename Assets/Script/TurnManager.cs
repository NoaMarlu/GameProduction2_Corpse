using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{

    //基本設定
    public static TurnManager Instance { get; private set; }
    public enum TurnState 
    {
        Wait,   //プレイヤーの入力待機
        Action, //実行処理
    }
    public TurnState turnState = TurnState.Wait;

    //キャラクター
    private List<Enemy> enemies = new List<Enemy>();
    private Player player;

    void Awake()
    {
        //シングルトン化
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //プレイヤーの登録
    public void SetPlayer(Player p) { player = p; }
    //敵の登録
    public void AddEnemy(Enemy enemy) { enemies.Add(enemy); }
    //敵の登録解除
    public void RemoveEnemy(Enemy enemy) { enemies.Remove(enemy); }
    //プレイヤーの移動入力通知
    public void isPlayerInput(Vector2Int direction)
    {
        if ( turnState != TurnState.Wait) return;

        if (!player.CanMove(direction)) return;

        turnState = TurnState.Action;

        //移動方向を設定
        player.SetMoveDirection(direction);

        //実行
        foreach(var enemy in enemies) { enemy.EnemyMove(); }
        player.PlayerMove();

        turnState = TurnState.Wait;
    }

}
