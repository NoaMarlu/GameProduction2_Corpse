using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class StageManager : MonoBehaviour 
{
    public static StageManager Instance { get; private set; }
    public Stage currentStage;
    private List<Stage> allStages  = new List<Stage>();

    //SE
    private AudioSource audioSource;
    public AudioClip clearClip;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        PlayerInput();
    }

    //プレイヤー入力処理
    void PlayerInput()
    {
        if (TurnManager.Instance.turnState != TurnManager.TurnState.Wait) return;
        //リセット入力
        if(Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            CurrentStageReset();
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            CurrentStageReset();
        }
    }
    //現在のステージをリセット
    public void CurrentStageReset()
    {
        if (currentStage == null) return;
        currentStage.ResetStage();
        //リセット後にスイッチの状況が変化したかチェック
        TurnManager.Instance.CheckSwitchDoor();
    }
    //currentStageの更新
    public void isPlayerStage(Stage newStage,Player player)
    {
        if (currentStage == newStage) return;
        //前のステージを非アクティブにする
        if (currentStage != null) currentStage.InactiveStage();
        //新しいステージをアクティブにする
        currentStage = newStage;
        currentStage.stagePlayer = player;
        currentStage.ActiveStage();
    }
    //ステージから出たら
    public void IsPlayerExitStage(Stage exitStage)
    {
        if (currentStage != exitStage) return;
        if (!currentStage.isCleared) currentStage.ExitStageAndNoClear();
        currentStage.InactiveStage();
        currentStage = null;
    }
    //現在のステージがクリアしているかチェック
    public void CheckCurrentStageClear()
    {
        if (currentStage == null) return;
        currentStage.CheckClear();
    }
    //ステージクリア時に呼び出し
    public void PlayClearSE() { audioSource.PlayOneShot(clearClip); }
    //ステージ追加
    public void AddStage(Stage stage)
    {
        if (!allStages.Contains(stage))allStages.Add(stage);
    }
    //プレイヤーがどのステージか判定
    public void CheckStageGridPos(Vector2Int gridPos,Player player)
    {

        //クリアマスがトリガー外にいたらnull
        if (currentStage != null &&currentStage.ClearGrid(gridPos))
        {
            currentStage = null;
        }

        foreach (var stage in allStages)
        {
            if (stage.ContainsGridPos(gridPos))
            {
                isPlayerStage(stage, player);
                return;
            }
        }

        //どこにもいなかったら
        if(currentStage != null)
        {
            if (currentStage.isCleared) return;
            currentStage.ExitStageAndNoClear();
            currentStage.InactiveStage();
            currentStage = null;
        }
    }
    //現在アクティブなステージを返す
    public Stage GetCurrentStage() { return currentStage; }
    //クリアマスにいるか判定
    public bool IsClearGridCurrentStage(Vector2Int gridPos)
    {
        if (currentStage == null) return false;
        return currentStage.ClearGrid(gridPos);
    }

}
