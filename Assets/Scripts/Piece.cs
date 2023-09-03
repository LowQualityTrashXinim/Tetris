using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public TetrominoData data { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = .1f;
    public float lockDelay = .25f;

    private float stepTime;
    private float lockTime;
    public void Initialize(Vector3Int position, TetrominoData data, Board board)
    {
        this.position = position;
        this.data = data;
        this.board = board;
        rotationIndex = 0;

        stepTime = Time.time + stepDelay;
        lockTime = 0;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }
    private bool Left, Right, Down = false;
    private void ResetEffect()
    {
        Left = false;
        Right = false;
        Down = false;
    }
    int delay = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        ResetEffect();

        board.Clear(this);

        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E))
            Rotate(1);
        else if (Input.GetKeyDown(KeyCode.Q))
            Rotate(-1);

        delay = delay > 0 ? delay - 1 : 0;
        if (delay == 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                delay = 60;
                Left = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                delay = 60;
                Right = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                delay = 10;
                Down = true;
            }
        }

        MovementHandle();

        if (Input.GetKeyDown(KeyCode.Space))
            InstantDrop();

        if (Time.time >= stepTime)
            Step();


        board.Set(this);
    }
    private void MovementHandle()
    {
        if (Input.GetKeyDown(KeyCode.A) || Left)
            Move(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D) || Right)
            Move(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.S) || Down)
            Move(Vector2Int.down);
    }
    private void InstantDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);
        if (lockTime >= lockDelay)
            Lock();
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLine();
        board.SpawnPiece();
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPos = position;
        newPos.x += translation.x;
        newPos.y += translation.y;
        if (board.IsValidPos(this, newPos))
        {
            position = newPos;
            lockTime = 0;
            return true;
        }
        return false;
    }
    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotation(direction);
        if (!WallKickTest(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotation(-direction);
        }
    }
    private void ApplyRotation(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;
            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= .5f;
                    cell.y -= .5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }
    private bool WallKickTest(int rotationIndex, int direction)
    {
        int index = GetWallKickIndex(rotationIndex, direction);
        for (int i = 0; i < data.wallkick.GetLength(1); i++)
        {
            Vector2Int translation = data.wallkick[index, i];
            if (Move(translation))
                return true;
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int direction)
    {
        int index = rotationIndex * 2;
        if (direction < 0)
            index--;
        return Wrap(index, 0, data.wallkick.GetLength(0));
    }
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
}