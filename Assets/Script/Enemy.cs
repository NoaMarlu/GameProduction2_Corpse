using UnityEngine;

public class Enemy : MonoBehaviour
{

    //グリッド情報
    public int gridX;
    public int gridY;

    public Vector2Int lastDirection;

    //Inspector上で指定する移動モード
    public enum MoveMode 
    {
        NoMove,         //移動なし
        RightToLeft,    //左右移動（右優先）
        UpToDown,       //上下移動（上優先）
    }
    public MoveMode moveMode;

    //Inspector上で指定する遺体の効果
    public enum CourpseMode 
    {
        NoCourpseMode,  //遺体効果なし
    }
    public CourpseMode courpseMode;

    void Awake()
    {
        //数値設定
        switch (moveMode)
        {
            case MoveMode.NoMove:   /*処理なし*/    break;
            case MoveMode.RightToLeft: lastDirection =new Vector2Int(1,0); break;
            case MoveMode.UpToDown: UpToDownFunc(); break;
        }
    }
    public void EnemyMove()
    {
        //移動モードにより動作を変更
        switch (moveMode) 
        {
            case MoveMode.NoMove:   /*処理なし*/    break;
            case MoveMode.RightToLeft:  RightToLeftFunc();  break;
            case MoveMode.UpToDown:   UpToDownFunc();   break;
        }
    }

    //移動系関数
    void RightToLeftFunc()
    {
    }
    void UpToDownFunc()
    {
        
    }
    //遺体効果系関数

}
