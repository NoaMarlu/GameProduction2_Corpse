using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance { get; private set; }

    //グリッドの基本サイズ
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    //Flagsにするとビット演算をInspecterでチェックボックス形式にしてくれる（らしい）
    [Flags]
    public enum GridType
    {
        None                      =0,
        PlayerDamaged       =1<<0,
        Weight               　   = 1<<1,///重し判定
    }

    public class Cell
    {
        public int x, y;
        public bool isWalk = true;//歩けるマスかの判定
        public GridType type = GridType.None;
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    private Cell[,] grid;

    //グリッド描画
    public Tilemap gridTilemap;
    public TileBase gridTileBase;

    void Awake()
    {
        //シングルトン化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //グリッドの作成
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(x, y);
            }
        }
        DrawGrid();
}
    //グリッド番号から座標を取得
    public Vector3 GridToWorld(int x, int y)  {return new Vector3(x * cellSize+ cellSize * 0.5f, y * cellSize+ cellSize * 0.5f, 0); }
    //座標からグリッド番号を取得
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }
    //座標からセルを取得
    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return grid[x, y];
    }

    //グリッドの描画
    void DrawGrid()
    {
        for(int x = 0; x < width ; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                gridTilemap.SetTile(pos, gridTileBase);
            }
        }
    }
    //実行前用の描画
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for(int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, height * cellSize,0);
            Gizmos.DrawLine(start, end);
        }
        for(int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(width * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }


}
