using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{

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

    void Start()
    {
        //グリッドの作成
        grid = new Cell[width,height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(x, y);
            }
        }
        DrawGrid();
    }

    //グリッド番号から座標を取得
    public Vector3 GridToWorld(int x, int y)  {return new Vector3(x * cellSize, y * cellSize, 0); }
    //座標からグリッド番号を取得
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
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


}
