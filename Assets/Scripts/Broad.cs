using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] TetrisData;
    public Vector3Int SpawnPosition = new Vector3Int(-1, 8);
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int pos = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(pos, boardSize);
        }
    }
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        SpawnPosition = new Vector3Int(-1, 8);
        for (int i = 0; i < TetrisData.Length; i++)
        {
            TetrisData[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }
    List<int> listofTetris = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
    public void SpawnPiece()
    {
        if(listofTetris.Count <= 0)
        {
            listofTetris = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
        }
        int randomIndex = Random.Range(0, listofTetris.Count);
        int random = listofTetris[randomIndex];
        listofTetris.Remove(random);
        TetrominoData data = TetrisData[random];
        activePiece.Initialize(SpawnPosition, data, this);
        if (IsValidPos(activePiece, SpawnPosition))
            Set(activePiece);
        else
            tilemap.ClearAllTiles();
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, null);
        }
    }
    public bool IsValidPos(Piece piece, Vector3Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + position;

            if (!Bounds.Contains((Vector2Int)tilePos))
                return false;

            if (tilemap.HasTile(tilePos))
                return false;
        }
        return true;
    }
    public void ClearLine()
    {
        int row = Bounds.yMin;
        while (row < Bounds.yMax)
        {
            if (IsLineFull(row))
                LineClear(row);
            else
                row++;
        }
    }
    private void LineClear(int row)
    {
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            tilemap.SetTile(pos, null);
        }
        while (row < Bounds.yMax)
        {
            for (int col = Bounds.xMin; col < Bounds.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);
                tilemap.SetTile(pos, tilemap.GetTile(pos + new Vector3Int(0, 1, 0)));
            }
            row++;
        }
    }
    private bool IsLineFull(int row)
    {
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row);
            if (!tilemap.HasTile(pos))
                return false;
        }
        return true;
    }
}