using UnityEngine;

public class Cell {
    public int Row;
    public int Col;
    public int Stack;
    public GameObject[] Walls; // The walls which make up this cell

    public Cell(int row, int col, GameObject[] walls)
    {
        this.Row = row;
        this.Col = col;
        this.Walls = walls;
    }

    public Cell(int row, int col, int stack, GameObject[] walls)
    {
        this.Row = row;
        this.Col = col;
        this.Stack = stack;
        this.Walls = walls;
    }
}