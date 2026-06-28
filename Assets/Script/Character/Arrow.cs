using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Arrow : MonoBehaviour
{

    public float moveSpeed = 0.05f;//1マスにかかる時間

    //位置設定
    private int gridX;
    private int gridY;
    private Vector2Int direction;

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

            //壁なら停止
            if (targetCell == null || !targetCell.isWalk) break;

            Enemy hitEnemy = FindEnemy(targetX, targetY);
            yield return StartCoroutine(MoveOneCell(targetX, targetY));

            gridX = targetX;
            gridY = targetY;
            if(hitEnemy != null)
            {
                hitEnemy.HitArrow();
                break;//敵に衝突で停止するおん
            }

        }
        Destroy(gameObject);
        TurnManager.Instance.IsArrowFinish();

    }
    //1マスを移動させる
    private IEnumerator MoveOneCell(int targetX,int targetY)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GridManager.Instance.GridToWorld(targetX, targetY);

        float elapsed = 0f;//経過時間
        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveSpeed);
            yield return null;
        }
        transform.position = endPos;
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
