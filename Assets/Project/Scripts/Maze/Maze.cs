using UnityEngine;

public enum Direction {
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public class Maze : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    [SerializeField] private int wallSize;

    [Header("Objects")]
    [SerializeField] private GameObject wallPrefab;

    private Transform _transform;
    private Cell[] _cells;  // Starts at bottom left then goes column major order
    private void Awake()
    {
        _transform = transform;
        _cells = new Cell[rows * cols];
    }
    private void Start()
    {
        InitializeCells();
        InitializeMaze();
    }

    private void InitializeMaze()
    {
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                int index = GetIndex(r, c);
                Cell cell = _cells[index];

                for (int dir = 0; dir < 4; dir++) // Looping through directions
                {
                    Cell neighbor = GetNeighbor(cell, (Direction)dir);
                    int oppositeDir = (dir + 2) % 4;

                    if (neighbor != null && neighbor.Walls[oppositeDir] != null) 
                    { 
                        cell.Walls[dir] = neighbor.Walls[oppositeDir];

                        continue;
                    }

                    GameObject newWall = InstantiateNewWall(cell, (Direction)dir);
                    cell.Walls[dir] = newWall;
                }
            }
        }
    }

    private GameObject InstantiateNewWall(Cell cell, Direction dir)
    {
        Vector3 position = new Vector3(cell.Col, 0, cell.Row);
        Quaternion rotation = Quaternion.identity;

        if (dir == Direction.NORTH)
        {
            position.x += 0.5f * wallSize;
            position.z += wallSize;
        }
        else if (dir == Direction.EAST)
        {
            position.x += wallSize;
            position.z += 0.5f * wallSize;

            rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (dir == Direction.SOUTH)
        {
            position.x += 0.5f * wallSize;
        }
        else if (dir == Direction.WEST) 
        {
            position.z += 0.5f * wallSize;

            rotation = Quaternion.Euler(0, 90, 0);
        }

        GameObject wall = Instantiate(wallPrefab, position, rotation, _transform);

        return wall;
    }

    private void InitializeCells()
    {
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                int index = GetIndex(r, c);

                _cells[index] = new Cell(r, c, new GameObject[4]);
            }
        }
    }

    private Cell GetNeighbor(Cell cell, Direction dir)
    {
        int row = cell.Row;
        int col = cell.Col;

        if (dir == Direction.NORTH) { row++; }
        else if (dir == Direction.EAST) { col++; }
        else if (dir == Direction.SOUTH) { row--; }
        else if (dir == Direction.WEST) { col--; }

        int index = GetIndex(row, col);

        if (index == -1) { return null; }

        return _cells[index];
    }

    private int GetIndex(int row, int col)
    {
        if (row < 0 || row > rows - 1 || col < 0 || col > cols - 1) { return -1; }

        return col * rows + row;
    }
}