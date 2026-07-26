using UnityEngine;

public class Coin : MonoBehaviour
{

    private int gridX;
    private int gridY;


    private void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);
        TurnManager.Instance.AddCoin(this);
    }

    public bool IsPosition(int x, int y) => (gridX == x && gridY == y);
    //ƒRƒCƒ“Žæ“¾Žž‚ÉŒÄ‚Ô
    public void Collect()
    {
        CoinManager.Instance.AddCoin();
        TurnManager.Instance.RemoveCoin(this);
        Destroy(gameObject);
    }

}
