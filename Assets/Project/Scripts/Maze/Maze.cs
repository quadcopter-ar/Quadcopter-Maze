using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float wallSize;

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
        WilsonsAlgorithm();
    }

    private void WilsonsAlgorithm()
    {
        HashSet<Cell> notInMaze = new HashSet<Cell>(_cells);
        IEnumerator notInMazeEnumerator = notInMaze.GetEnumerator();
        notInMazeEnumerator.MoveNext();

        Cell startingCell = (Cell)notInMazeEnumerator.Current;
        notInMaze.Remove(startingCell);

        while (notInMaze.Count > 0)
        {
            notInMazeEnumerator = notInMaze.GetEnumerator();
            notInMazeEnumerator.MoveNext();

            Cell currentCell = (Cell)notInMazeEnumerator.Current;

            List<Cell> walk = LoopErasedRandomWalk(currentCell, notInMaze);            

            foreach (Cell cell in walk)
            {
                notInMaze.Remove(cell);
            }

            for (int i = walk.Count - 1; i >= 1; i--)
            {
                DestroyWallBetween(walk[i], walk[i - 1]);
            }
        }
    }

    private List<Cell> LoopErasedRandomWalk(Cell currentCell, HashSet<Cell> notInMaze)
    {
        List<Cell> visitedThisWalk = new List<Cell>();
        visitedThisWalk.Add(currentCell);

        while (notInMaze.Contains(currentCell))
        {
            Cell randomNeighbor = GetRandomNeighbor(currentCell);

            int indexOfRandomNeighbor = visitedThisWalk.IndexOf(randomNeighbor);

            if (indexOfRandomNeighbor == -1) 
            {
                visitedThisWalk.Add(randomNeighbor);
            }
            else
            {
                visitedThisWalk.RemoveRange(indexOfRandomNeighbor + 1, visitedThisWalk.Count - indexOfRandomNeighbor - 1); // Erase loop
            }

            currentCell = visitedThisWalk[visitedThisWalk.Count - 1];
        }

        return visitedThisWalk;
    }

    private void DestroyWallBetween(Cell a, Cell b)
    {
        if (a.Row == b.Row - 1)
        {
            Destroy(a.Walls[(int)Direction.NORTH]);
            a.Walls[(int)Direction.NORTH] = null;
            b.Walls[(int)Direction.SOUTH] = null;
        }
        else if (a.Col == b.Col - 1)
        {
            Destroy(a.Walls[(int)Direction.EAST]);
            a.Walls[(int)Direction.EAST] = null;
            b.Walls[(int)Direction.WEST] = null;
        }
        else if (a.Row == b.Row + 1)
        {
            Destroy(a.Walls[(int)Direction.SOUTH]);
            a.Walls[(int)Direction.SOUTH] = null;
            b.Walls[(int)Direction.NORTH] = null;
        }
        else if (a.Col == b.Col + 1)
        {
            Destroy(a.Walls[(int)Direction.WEST]);
            a.Walls[(int)Direction.WEST] = null;
            b.Walls[(int)Direction.EAST] = null;
        }
    }

    private Cell GetRandomNeighbor(Cell cell) 
    { 
        List<Cell> validNeighbors = GetValidNeighbors(cell);

        return validNeighbors[Random.Range(0, validNeighbors.Count)];
    }

    private List<Cell> GetValidNeighbors(Cell cell)
    {
        List<Cell> validNeighbors = new List<Cell>();

        Cell northNeighbor = GetNeighbor(cell, Direction.NORTH);
        Cell eastNeighbor = GetNeighbor(cell, Direction.EAST);
        Cell southNeighbor = GetNeighbor(cell, Direction.SOUTH);
        Cell westNeighbor = GetNeighbor(cell, Direction.WEST);

        if (northNeighbor != null) { validNeighbors.Add(northNeighbor); }
        if (eastNeighbor != null) { validNeighbors.Add(eastNeighbor); }
        if (southNeighbor != null) { validNeighbors.Add(southNeighbor); }
        if (westNeighbor != null) { validNeighbors.Add(westNeighbor); }

        return validNeighbors;
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
        Vector3 position = new Vector3(cell.Col, 0, cell.Row) * wallSize;
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

    private int GetIndex(int row, int col)
    {
        if (row < 0 || row > rows - 1 || col < 0 || col > cols - 1) { return -1; }

        return col * rows + row;
    }
}