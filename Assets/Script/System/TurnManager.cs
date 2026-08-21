using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;

public class TurnManager : MonoBehaviour
{

    //基本設定
    public static TurnManager Instance { get; private set; }
    public enum TurnState 
    {
        Wait,   //プレイヤーの入力待機
        Action, //実行処理
        Arrow,//矢の発射中
        StageClear,//ステージクリア状態
    }
    public TurnState turnState = TurnState.Wait;

    //キャラクター
    private List<Enemy> enemies = new List<Enemy>();
    private Player player;
    public GameObject arrowPrefab;
    private bool playerStuck = false;

    //スイッチ
    private List<Switch> switches = new List<Switch>();
    private List<Door> doors = new List<Door>();

    //プレイヤーの死亡演出
    private bool isDying = false;

    //クリアブロック
    private List<ClearBlock> clearBlocks = new List<ClearBlock>();
    private List<ClearDoor> clearDoors = new List<ClearDoor>();

    //コイン
    private List<Coin> coins = new List<Coin>();

    //SE
    private AudioSource audioSource;
    public AudioClip clearBlock;

    //リセット
    public float resetDuration = 1.5f;//リセット待機時間

    //ロード画面処理
    private List<SceneLoader> sceneLoaders = new List<SceneLoader>();

    void Awake()
    {
        //シングルトン化
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
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
        if (playerStuck) return;
        if (!player.CanMove(direction)) return;

        turnState = TurnState.Action;

        //移動方向を設定
        player.SetMoveDirection(direction);
        //移動先のチェック
        Vector2Int nextPos = new Vector2Int(player.gridX + direction.x, player.gridY + direction.y);
        StageManager.Instance.CheckStageGridPos(nextPos,player);
        //レベル順の敵
        var orderEnemies = enemies.OrderByDescending(e => e.level).ToList();

        foreach(var enemy in orderEnemies)
        {
            Vector2Int decide = enemy.DecideMove();
            enemy.ConfilmMove(decide);
            //衝突チェック
            if(enemy.gridX == player.gridX && enemy.gridY == player.gridY)
            {
                TriggerPlayerDie();
                return;
            }
        }

        player.PlayerMove();

        //ボス戦なら移動するたびにボスの行動を呼ぶ
        if (BossManager.Instance != null) BossManager.Instance.PlayerMove();

        //移動後にチェック
        CheckPlayerEnemyCollision();
        CheckSwitchDoor();
        CheckPlayerTrapped();
        CheckEnemyTrapped();
        CheckClearBlock();
        CheckCoinCollect();
        CheckDecay();
        CheckSticky();
        CheckClear();
        CheckClearDoor();
    }
    //敵とプレイヤーが同じマスにいるかどうか
    void CheckPlayerEnemyCollision()
    {
        if (player.isCreativeMode) return;

        foreach(var enemy in enemies)
        {
            //位置が同じならリセット
            if(enemy.gridX == player.gridX && enemy.gridY == player.gridY)
            {
                TriggerPlayerDie();
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
        CheckClearBlock();
        CheckDecay();
        CheckClearDoor();
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
        bool changed;

        do
        {
            changed = false;
            var cell = GridManager.Instance.GetCell(player.gridX, player.gridY);
            //プレイヤーが腐敗マスにいるか
            if (!player.isCreativeMode && cell != null && (cell.type & GridManager.GridType.Decay) != 0)
            {
                TriggerPlayerDie();
                return;
            }
            //敵が腐敗マスにいるか
            foreach (var enemy in enemies.ToList())
            {
                var enemyCell = GridManager.Instance.GetCell(enemy.gridX, enemy.gridY);
                //矢の衝突処理を使いまわす
                if (enemyCell != null && (enemyCell.type & GridManager.GridType.Decay) != 0) 
                {
                    enemy.HitArrow(Vector2Int.zero);
                    changed = true;
                }
            }
        } while (changed);

    }
    //クリアチェック
    void CheckClear() { StageManager.Instance.CheckCurrentStageClear(); }
    //プレイヤーの死亡処理
    void TriggerPlayerDie()
    {
        if (isDying) return;
        isDying = true;

        PlayHitEffects();
        Player.Instance.DamageSE();
        Player.Instance.PlayerDieVisual();
        StartCoroutine(DelayedReset());

    }
    IEnumerator DelayedReset()
    {
        //死亡演出中の待機時間
        yield return new WaitForSeconds(0.3f);
        StageManager.Instance.CurrentStageReset();
        yield return new WaitForSeconds(resetDuration);
        turnState = TurnState.Wait;
        isDying = false;
    }
    void PlayHitEffects() { CameraManager.Instance.Shake();}
    //プレイヤーの位置が壁になってないか確認
    void CheckPlayerTrapped()
    {
        if (player.isCreativeMode) return;
        var cell = GridManager.Instance.GetCell(player.gridX, player.gridY);
        if (cell != null && !cell.isWalk) { TriggerPlayerDie(); }
    }
    //敵の位置が壁になってないか確認
    void CheckEnemyTrapped()
    {
        foreach (var enemy in enemies.ToList())
        {
            var cell = GridManager.Instance.GetCell(enemy.gridX, enemy.gridY);
            if (cell != null && !cell.isWalk) enemy.HitArrow(Vector2Int.zero);
        }
    }
    //trunStateの変更
    public void ChangeTurnState(TurnState state){ turnState = state; }
    //クリアブロックの追加
    public void AddClearBlock(ClearBlock block) { if (!clearBlocks.Contains(block)) clearBlocks.Add(block); }
    public void AddClearDoor(ClearDoor door) { if (!clearDoors.Contains(door)) clearDoors.Add(door); }
    //全クリアブロックの確認
    void CheckClearBlock() { foreach (var block in clearBlocks) block.CheckBlock(); }
    void CheckClearDoor() { foreach (var door in clearDoors) { door.CheckDoor(); } }
    public void BlockedMove(int x,int y)
    {
        foreach(var block in clearBlocks)
        {
            if (block.IsPosition(x, y))
            {
                CameraManager.Instance.Shake();
                if(audioSource != null && clearBlock != null)audioSource.PlayOneShot(clearBlock);
                block.PlayFlash();
                return;
            }
        }
    }
    //コイン用関数
    public void AddCoin(Coin coin) { if(!coins.Contains(coin))coins.Add(coin); }
    public void RemoveCoin(Coin coin) {coins.Remove(coin); }
    //プレイヤーがコインのマスにいるかチェック
    void CheckCoinCollect()
    {
        foreach(var coin in coins.ToList())
        {
            if(coin.IsPosition(player.gridX,player.gridY))coin.Collect();
        }
    }
    //外部から呼ばれるリセット処理
    public void RequestStageReset() { StartCoroutine(ResetWithLock()); }
    //リセット待機
    IEnumerator ResetWithLock()
    {
        turnState = TurnState.Action;
       StageManager.Instance.CurrentStageReset();
        yield return new WaitForSeconds(resetDuration);
        turnState = TurnState.Wait;
    }
    //ロード画面の登録
    public void AddSceneLoader(SceneLoader loader) { if (!sceneLoaders.Contains(loader)) sceneLoaders.Add(loader); }
    //カメレオン関連チェック
    void CheckSticky()
    {
        var playerCell = GridManager.Instance.GetCell(player.gridX, player.gridY);
        playerStuck = (playerCell != null && (playerCell.type & GridManager.GridType.Sticky) != 0);
    }

}
