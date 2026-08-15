using UnityEngine;

public class StageMask : MonoBehaviour
{
    public static StageMask Instance { get; private set; }

    public SpriteRenderer maskRenderer;//マスクのスプライト

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //ステージに入ったら呼ぶ
    public void ShowMaskFor(Stage stage)
    {
        if (maskRenderer == null) return;
        maskRenderer.gameObject.SetActive(true);
    }
    public void HideMask()
    {
        if (maskRenderer == null) return;
        maskRenderer.gameObject.SetActive(false);
    }

}
