using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Arrow : MonoBehaviour
{

    public float moveSpeed = 0.05f;//1マスにかかる時間

    //位置設定
    private int gridX;
    private int gridY;
    private Vector2Int direction;

    //エフェクト
    public GameObject bomb;

    //SE
    private AudioSource audioSource;
    public AudioClip shotSE;

    void Awake()
    {
        //生成時にSE再生
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(shotSE);
    }

    //発射時に呼ぶ
    public void Fire(int startX,int startY,Vector2Int dir)
    {
        gridX = startX;
        gridY = startY;
        direction = dir;
        transform.position  = GridManager.Instance.GridToWorld(gridX, gridY);
        StartCoroutine(FlyRoutine());
    }
    private IEnumerator FlyRoutine()
    {
        while (true)
        {
            int targetX  = gridX + direction.x;
            int targetY = gridY + direction.y;
            var targetCell  = GridManager.Instance.GetCell(targetX, targetY);

            //画面外なら削除
            if (targetCell == null) break;

            Enemy hitEnemy = FindEnemy(targetX, targetY);

            yield return StartCoroutine(MoveOneCell(targetX, targetY,targetCell));

            gridX = targetX;
            gridY = targetY;

            //壁なら爆発
            if (!targetCell.isWalk) break;
            //敵なら待つ
            if(hitEnemy != null)
            {
                //端に行くまで待機
                hitEnemy.HitArrow();
                break;//敵に衝突で停止するおん
            }

        }

        //エフェクト再生・オブジェクト削除
        if(bomb != null)Instantiate(bomb, transform.position, Quaternion.identity);
        Destroy(gameObject);
        //終了通知
        TurnManager.Instance.IsArrowFinish();

    }
    //1マスを移動させる
    private IEnumerator MoveOneCell(int targetX,int targetY,GridManager.Cell targetCell)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GridManager.Instance.GridToWorld(targetX, targetY);

        //移動先が壁なら0.5fの位置を終点に設定
        Vector3 finalEndPos = endPos;
        if (!targetCell.isWalk) finalEndPos = Vector3.Lerp(startPos, endPos, 0.5f);

        float elapsed = 0f;//経過時間
        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveSpeed);

            //壁なら
            if (!targetCell.isWalk && (elapsed / moveSpeed) >= 0.5f) break;

            yield return null;
        }
        transform.position = finalEndPos;
    }
    //引数のマスにエネミーがいるか検索
    private Enemy FindEnemy(int x,int y)
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(var enemy in allEnemies)
        {
            if (enemy.gridX == x && enemy.gridY == y) return enemy;
        }
        return null;
    }

}
