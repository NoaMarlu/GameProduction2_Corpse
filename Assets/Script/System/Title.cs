using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) SceneManager.LoadScene("GameScene");
    }

}
