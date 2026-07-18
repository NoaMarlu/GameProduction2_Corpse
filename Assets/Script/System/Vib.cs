using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemが必要です

public class Vib : MonoBehaviour
{
    void Update()
    {
        // 毎フレームこの関数を呼び出してチェック
        Vibr();
    }

    void Vibr()
    {
        // 接続されている最初のゲームパッドを取得
        var gamepad = Gamepad.current;
        if (gamepad == null) return; // コントローラーが繋がっていない場合は何もしない

        // キーボードのスペースキーが押されているかチェック
        if (Keyboard.current.spaceKey.isPressed)
        {
            // 振動を開始 (左モーターの強さ, 右モーターの強さ) ※1.0が最大
            gamepad.SetMotorSpeeds(0.5f, 0.5f);
        }
        else
        {
            // スペースキーが離されたら振動を止める
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    // アプリ終了時やオブジェクトが消えた時に振動を確実に止めるクリーンアップ
    void OnDisable()
    {
        Gamepad.current?.SetMotorSpeeds(0f, 0f);
    }
}