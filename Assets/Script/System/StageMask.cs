using UnityEngine;

public class StageMask : MonoBehaviour
{
    public static StageMask Instance { get; private set; }

    public SpriteRenderer maskRenderer;//マスクのスプライト
    public SpriteRenderer maskedRenderer;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        HideMask();
    }

    //ステージに入ったら呼ぶ
    public void ShowMask()
    {
        if(maskRenderer != null)maskRenderer.gameObject.SetActive(true);
        if(maskedRenderer != null)maskedRenderer.gameObject.SetActive(true);
    }
    public void HideMask()
    {
        if (maskRenderer != null)maskRenderer.gameObject.SetActive(false);
        if(maskedRenderer != null)maskedRenderer.gameObject.SetActive(false);
    }

}
