using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{

    public int stageID;

    [HideInInspector] public List<Enemy> stageEnemies = new List<Enemy>();
    [HideInInspector] public Player stagePlayer;

    public bool isActive;//ステージがアクティブか

    //クリア設定
    public GameObject clearMarker;//クリアを判定するグリッドの位置
    public bool isCleared = false;//クリア済み判定

    //初期位置
    public GameObject initMarker;//スポーンを判定するグリッドの位置

    //カメラ設定
    public GameObject cameraMarker;//カメラを判定するグリッドの位置

    //オブジェクトから変換する変数
    private Vector2Int clearGridPos;
    private Vector2Int initGridPos;
    private Vector3 cameraPos;

    void Awake()
    {
        //エネミーはステージの子オブジェクトにする
        stageEnemies.AddRange(GetComponentsInChildren<Enemy>());
    }
    void Start()
    {
        //GameObjectの位置からグリッド座標を返還
        if (clearMarker != null) clearGridPos = GridManager.Instance.WorldToGrid(clearMarker.transform.position);
        if (initMarker != null) initGridPos = GridManager.Instance.WorldToGrid(initMarker.transform.position);
        if (cameraMarker != null) cameraPos = cameraMarker.transform.position;
    }

    //ステージをアクティブにする
    public void ActiveStage()
    {
        isActive = true;
        foreach (var enemy in stageEnemies)
        {
            enemy.SetActive(true);
        }

        //プレイヤーの初期位置を設定
        if (stagePlayer != null) stagePlayer.SetInitPos(initGridPos);
        //カメラをLerp遷移
        CameraManager.Instance.MoveTo(cameraPos);
    }
    //ステージを非アクティブにする
    public void InactiveStage()
    {
        isActive = false;
        foreach (var enemy in stageEnemies)
        {
            enemy.SetActive(false);
        }
    }
    //リセット呼び出し
    public void ResetStage()
    {
        if (isCleared) return;//クリア済みならリセットしない

        foreach (var enemy in stageEnemies)
        {
            enemy.EnemyReset();
        }
        if (stagePlayer != null)
        {
            stagePlayer.PlayerReset();
        }
    }
    //未クリア時にステージを出た際の処理
    public void ExitStageAndNoClear()
    {
        if (isCleared) return;//クリア済みならリセットしない

        foreach (var enemy in stageEnemies)
        {
            enemy.EnemyReset();
        }
    }

    //クリアしているかチェック
    public void CheckClear()
    {
        if (isCleared) return;
        if (stagePlayer == null) return;

        //モンスターが全て遺体かチェック
        bool allEnemiesDefeated = stageEnemies.All(e => e.IsCorpse());
        //指定グリッドを踏んでいる状態でモンスターを討伐していればクリア
        bool onClearGrid = stagePlayer.gridX == clearGridPos.x && stagePlayer.gridY == clearGridPos.y;

        if(allEnemiesDefeated && onClearGrid)
        {
            isCleared = true;
            OnClear();
        }
    }
    //クリア時処理
    void OnClear()
    {
        //処理なし
    }


}