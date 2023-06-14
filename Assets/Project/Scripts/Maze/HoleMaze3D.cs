using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoleMaze3D : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] [Range(5f, 21f)] private int rows; //im not sure where else i can find the values of rows and cols aside from holemaze and holemaze3d so i made em public
    //[SerializeField] [Range(5f, 21f)] private int cols;
    [Range(5f, 21f)] public int cols;
    [SerializeField] [Range(5f, 21f)] private int stacks; // What's after rows and columns? Rows, cols, ... stacks?
    [Space]
    //[SerializeField] [Range(3f, 19f)] private int holeRows;
    [Range(3f, 19f)] public int holeRows;
    [SerializeField] [Range(3f, 19f)] private int holeCols;
    [SerializeField] [Range(3f, 19f)] private int holeStacks;
    [Space]
    [SerializeField] private float wallLength;
    [SerializeField] private float wallHeight;

    [Header("Objects")]
    [SerializeField] private GameObject wallPrefab;

    private Transform _transform;
    private Cell[] _cells;  // Starts at bottom left then goes column major order, then goes vertically
    public GameObject boundary;
    private void Awake()
    {
        _transform = transform;
        _cells = new Cell[rows * cols * stacks];
    }
    private void Start()
    {
        InitializeCells();
        InitializeMaze();
        GenerateMaze();
        GenerateBoundary();
        //DestroyWallsForEntranceAndGoal();
    }

    private void OnValidate()
    {
        // We want odd mazes so there's a clear center
        if (holeRows % 2 == 0) { holeRows++; }
        if (holeCols % 2 == 0) { holeCols++; }
        if (holeStacks % 2 == 0) { holeStacks++; }

        // hole dimentions can't be >= maze dimentions
        if (holeRows >= rows) { holeRows = rows - 2; }
        if (holeCols >= cols) { holeCols = cols - 2; }
        if (holeStacks >= stacks) { holeStacks = holeStacks - 2; }

        // We want odd mazes so there's a clear center
        if (rows % 2 == 0) { rows++; }
        if (cols % 2 == 0) { cols++; }
        if (stacks % 2 == 0) { stacks++; }
    }

    private void GenerateMaze() // Wilson's Algorithm
    {
        HashSet<Cell> notInMaze = new HashSet<Cell>(_cells);
        IEnumerator notInMazeEnumerator = notInMaze.GetEnumerator();
        notInMazeEnumerator.MoveNext();

        Cell startingCell = (Cell)notInMazeEnumerator.Current;
        notInMaze.Remove(startingCell);
        notInMaze.Remove(null); // The cells in the hole will be null. Since this is a set it has no duplicates so removing null means removing all cells in the hole

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

    private void DestroyWallsForEntranceAndGoal()
    {

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
        Cell upNeighbor = GetNeighbor(cell, Direction.UP);
        Cell downNeighbor = GetNeighbor(cell, Direction.DOWN);

        if (northNeighbor != null) { validNeighbors.Add(northNeighbor); }
        if (eastNeighbor != null) { validNeighbors.Add(eastNeighbor); }
        if (southNeighbor != null) { validNeighbors.Add(southNeighbor); }
        if (westNeighbor != null) { validNeighbors.Add(westNeighbor); }
        if (upNeighbor != null) { validNeighbors.Add(upNeighbor); }
        if (downNeighbor != null) { validNeighbors.Add(downNeighbor); }

        return validNeighbors;
    }

    private Cell GetNeighbor(Cell cell, Direction dir)
    {
        int row = cell.Row;
        int col = cell.Col;
        int stack = cell.Stack;

        if (dir == Direction.NORTH) { row++; }
        else if (dir == Direction.EAST) { col++; }
        else if (dir == Direction.SOUTH) { row--; }
        else if (dir == Direction.WEST) { col--; }
        else if (dir == Direction.UP) { stack++; }
        else if (dir == Direction.DOWN) { stack--; }

        int index = GetIndex(row, col, stack);

        if (index == -1) { return null; }

        return _cells[index];
    }

    private void InitializeMaze()
    {
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int s = 0; s < stacks; s++)
                {
                    int index = GetIndex(r, c, s);
                    Cell cell = _cells[index];

                    if (cell == null) { continue; } // Cell is in the hole

                    for (int dir = 0; dir < 6; dir++) // Looping through directions
                    {
                        Cell neighbor = GetNeighbor(cell, (Direction)dir);
                        int oppositeDir;

                        if (dir == (int)Direction.UP) { oppositeDir = (int)Direction.DOWN; } // Special cases for up and down
                        if (dir == (int)Direction.DOWN) { oppositeDir = (int)Direction.UP; }
                        else { oppositeDir = (dir + 2) % 4; } // Normal case

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
    }

    private GameObject InstantiateNewWall(Cell cell, Direction dir)
    {
        Vector3 position = new Vector3(cell.Col, cell.Stack, cell.Row) * wallLength;
        Quaternion rotation = Quaternion.identity;

        if (dir == Direction.NORTH)
        {
            position.x += 0.5f * wallLength;
            position.z += wallLength;
        }
        else if (dir == Direction.EAST)
        {
            position.x += wallLength;
            position.z += 0.5f * wallLength;

            rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (dir == Direction.SOUTH)
        {
            position.x += 0.5f * wallLength;
        }
        else if (dir == Direction.WEST) 
        {
            position.z += 0.5f * wallLength;

            rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (dir == Direction.UP) 
        {
            position.x += 0.5f * wallLength;
            position.y += 0.5f * wallHeight;
            position.z += 0.5f * wallLength;

            rotation = Quaternion.Euler(90, 0, 0);
        }
        else if (dir == Direction.DOWN) 
        {
            position.x += 0.5f * wallLength;
            position.y -= 0.5f * wallHeight;
            position.z += 0.5f * wallLength;

            rotation = Quaternion.Euler(90, 0, 0);
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
                for (int s = 0; s < stacks; s++)
                {
                    int index = GetIndex(r, c, s);

                    if (r  >= (rows - holeRows) / 2 && r <= (rows + holeRows) / 2 - 1) {
                        if (c  >= (cols - holeCols) / 2 && c <= (cols + holeCols) / 2 - 1) {
                            if (s  >= (stacks - holeStacks) / 2 && s <= (stacks + holeStacks) / 2 - 1) {
                                // This cell is in the hole so leave it as null
                                continue;
                            }
                        }
                    }

                    _cells[index] = new Cell(r, c, s, new GameObject[6]);
                }
            }
        }
    }

    private int GetIndex(int row, int col, int stack)
    {
        if (row < 0 || row > rows - 1 || col < 0 || col > cols - 1 || stack < 0 || stack > stacks - 1) { return -1; }

        return row + col * rows + (cols * rows) * stack;
    }

    private int GetIndex(int row, int col)
    {

        return 0;
    }

    void GenerateBoundary() {
        GameObject temp = Instantiate(boundary, new Vector3(cols / 2.0f, cols / 2.0f - 0.5f, cols / 2.0f), Quaternion.identity);
        temp.transform.localScale = new Vector3(holeRows, holeRows, holeRows);
    }
}