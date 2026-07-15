using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;
using System.Linq;

public class TurnManager : MonoBehaviour
{

    //基本設定
    public static TurnManager Instance { get; private set; }
    public enum TurnState 
    {
        Wait,   //プレイヤーの入力待機
        Action, //実行処理
        Arrow,//矢の発射中
    }
    public TurnState turnState = TurnState.Wait;

    //キャラクター
    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    public GameObject arrowPrefab;

    //スイッチ
    private List<Switch> switches = new List<Switch>();
    private List<Door> doors = new List<Door>();

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
    public void AddEnemy(Enemy enemy) 
    {
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
    }
    //敵の登録解除
    public void RemoveEnemy(Enemy enemy) 
    {
        enemies.Remove(enemy); 
    }
    //全ての敵を返す
    public List<Enemy> GetEnemies() { return enemies; }
    //プレイヤーの移動入力通知
    public void isPlayerInput(Vector2Int direction)
    {
        if ( turnState != TurnState.Wait) return;
        if (!player.CanMove(direction)) return;

        turnState = TurnState.Action;

        //移動方向を設定
        player.SetMoveDirection(direction);
        //level順にソートして実行
        foreach(var enemy in enemies.OrderByDescending(e => e.level))
        {
            enemy.EnemyMove();
            //実行中に重なってもリセット
            if(enemy.gridX == player.gridX && enemy.gridY == player.gridY)
            {
                PlayHitEffects();
                turnState = TurnState.Wait;
                StageManager.Instance.CurrentStageReset();
                return;
            }
        }
        player.PlayerMove();

        //移動後にチェック
        CheckPlayerEnemyCollision();
        CheckDecay();
        CheckSwitchDoor();
        CheckClear();

        turnState = TurnState.Wait;
    }
    //敵とプレイヤーが同じマスにいるかどうか
    void CheckPlayerEnemyCollision()
    {
        foreach(var enemy in enemies)
        {
            //位置が同じならリセット
            if(enemy.gridX == player.gridX && enemy.gridY == player.gridY)
            {
                PlayHitEffects();
                StageManager.Instance.CurrentStageReset();
                return;
            }
        }
    }

    /*矢関連*/
    //矢の発射
    public void FireArrow(Vector2Int direction)
    {
        if (turnState != TurnState.Wait) return;

        turnState = TurnState.Arrow;
        //矢を設定
        GameObject arrowObj = Instantiate(arrowPrefab);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.Fire(player.gridX, player.gridY, direction);
    }
    //矢の発射が終了時に呼ばれる
    public void IsArrowFinish()
    {
        turnState = TurnState.Wait;
        //打ってから状態が変わった可能性があるため
        CheckSwitchDoor();
    }

    /*スイッチ関連*/
    public void AddSwitch(Switch sw) { if (!switches.Contains(sw)) switches.Add(sw); }
    public void AddDoor(Door door) { if (!doors.Contains(door)) doors.Add(door); }
    public Player GetPlayer() { return player; }
    //スイッチとドアの状況をチェック
    public void CheckSwitchDoor()
    {
        foreach (var sw in switches) { sw.CheckSwitch(); }
        foreach (var door in doors) { door.CheckDoor(); }
    }

    /*遺体効果関連*/
    //腐敗マスチェック
    void CheckDecay()
    {
        var cell = GridManager.Instance.GetCell(player.gridX, player.gridY);
        //プレイヤーが腐敗マスにいるか
        if(cell != null && (cell.type & GridManager.GridType.Decay) != 0)
        {
            PlayHitEffects();
            turnState = TurnState.Wait;
            StageManager.Instance.CurrentStageReset();
            return;
        }
        //敵が腐敗マスにいるか
        foreach (var enemy in enemies.ToList())
        {
            var enemyCell = GridManager.Instance.GetCell(enemy.gridX, enemy.gridY);
            //矢の衝突処理を使いまわす
            if (enemyCell != null && (enemyCell.type & GridManager.GridType.Decay) != 0) enemy.HitArrow();
        }
    }
    //クリアチェック
    void CheckClear() { StageManager.Instance.CheckCurrentStageClear(); }

    void PlayHitEffects()
    {
        CameraManager.Instance.Shake();
        player.GetVisual()?.PlayHitEffect();
    }

}
