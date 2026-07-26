using UnityEngine;

public class ArrowThroughWall : MonoBehaviour
{

    private int gridX;
    private int gridY;

    void Start()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);

        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) 
        {
            cell.isWalk = false; 
            cell.arrowBlock = false;
        }
    }

}
