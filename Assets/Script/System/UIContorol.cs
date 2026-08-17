using UnityEngine;
using UnityEngine.InputSystem;

public class UIContorol : MonoBehaviour
{

    public GameObject gamepadUI;
    public GameObject keyboardUI;

    void Update()
    {
        //どちらかがnullならreturn
        if (gamepadUI == null || keyboardUI == null) return;

        //コントローラー接続か否かで表示するUIを変える
        if(Gamepad.current != null)
        {
            keyboardUI.SetActive(false);
            gamepadUI.SetActive(true);
        }
        else
        {
            keyboardUI.SetActive(true);
            gamepadUI.SetActive(false);
        }

    }

}
