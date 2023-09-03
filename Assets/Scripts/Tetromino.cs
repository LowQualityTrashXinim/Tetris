using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells {get; private set;}
    public Vector2Int[,] wallkick {get; private set;}
    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallkick = Data.WallKicks[tetromino];
    }
}