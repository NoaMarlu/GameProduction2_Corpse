using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class StageManager : MonoBehaviour 
{
    public static StageManager Instance { get; private set; }

    private Stage currentStage;

    void Awake()
    {
        Instance = this;
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

}
