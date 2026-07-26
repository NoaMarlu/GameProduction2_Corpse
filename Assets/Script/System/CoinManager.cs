using UnityEngine;

public class CoinManager : MonoBehaviour
{

    public static CoinManager Instance { get; private set; }
    private int totalCoins = 0;

    //SE
    private AudioSource audioSource;
    public AudioClip getCoinSE;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //コイン取得時に呼び出し
    public void AddCoin()
    {
        totalCoins++;
        //SE再生
        if (getCoinSE != null)audioSource.PlayOneShot(getCoinSE);
    }
    //全体コイン数を取得
    public int GetTotalCoins() { return totalCoins; }

}
