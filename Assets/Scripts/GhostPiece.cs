using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = cells[i] + position;
            tilemap.SetTile(tilePos, null);
        }
    }
    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
            cells[i] = trackingPiece.cells[i];

    }
    private void Drop()
    {
        Vector3Int trackPos = trackingPiece.position;
        int current = trackPos.y;
        int bottom = -board.boardSize.y / 2 - 1;

        board.Clear(trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            trackPos.y = row;
            if (board.IsValidPos(trackingPiece, trackPos))
                position = trackPos;
            else 
                break;
        }

        board.Set(trackingPiece);
    }
    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = cells[i] + position;
            tilemap.SetTile(tilePos, tile);
        }
    }
}
