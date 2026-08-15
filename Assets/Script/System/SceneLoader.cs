using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{

    public string sceneName;

    //UI
    public GameObject flashUI;
    public float blinkInterval = 0.5f;

    private bool playerInRange = false;
    private bool isLoading = false;
    private Coroutine blinkCoroutine;

    void Awake()
    {
        if (flashUI != null) flashUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || isLoading) return;
        bool pressed = false;

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) pressed = true;
        }
        else
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) pressed = true;
        }

        //ボタンが押されてたらシーンをロード
        if (pressed)
        {
            TriggerLoad();
        }

    }

    //当たり判定
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Player>() == null) return;
        playerInRange = true;
        StartBlink();
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<Player>() == null) return;
        playerInRange = false;
        StopBlink();
    }

    //点滅処理
    void StartBlink()
    {
        if (flashUI == null) return;
        StopBlink();
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }
    void StopBlink()
    {
        if(blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        if(flashUI != null)flashUI.SetActive(false);
    }
    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            flashUI.SetActive(!flashUI.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void TriggerLoad()
    {
        isLoading = true;
        StopBlink();
        StartCoroutine(LoadSceneRoutine());
    }
    IEnumerator LoadSceneRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while(!op.isDone)yield return null;
    }

}
