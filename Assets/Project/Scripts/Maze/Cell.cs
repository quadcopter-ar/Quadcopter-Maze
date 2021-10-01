using UnityEngine;

public class Cell {
    public bool Visited;
    public int Row;
    public int Col;
    public GameObject[] Walls; // The walls which make up this cell

    public Cell(int row, int col, GameObject[] walls)
    {
        this.Visited = false;
        this.Row = row;
        this.Col = col;
        this.Walls = walls;
    }
}
