using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public int size = 6; //must be even
    public GameObject wall, finalWall;
    private Dictionary<Vector3, GameObject> wallPositions = new Dictionary<Vector3, GameObject>();
    private GameObject HalfMaze; //reflect this to create a full maze
    private bool[,,] traversed;

    bool LeftMoveIsValid(int x, int y, int z) {
        if (x > 1 && !traversed[x - 1, y, z]) {
            return true;
        }
        return false;
    }

    bool RightMoveIsValid(int x, int y, int z) {
        if (x + 1 < size / 2 && !traversed[x + 1, y, z]) {
            return true;
        }
        return false;
    }

    bool DownMoveIsValid(int x, int y, int z) {
        if (y > 0 && !traversed[x, y - 1, z]) {
            return true;
        }
        return false;
    }

    bool UpMoveIsValid(int x, int y, int z) {
        if (y + 1 < size && !traversed[x, y + 1, z]) {
            return true;
        }
        return false;
    }

    bool ForwardMoveIsValid(int x, int y, int z) { //forward should be renamed with backward
        if (z > 1 && !traversed[x, y, z - 1]) {
            return true;
        }
        return false;
    }

    bool BackwardMoveIsValid(int x, int y, int z) {
        if (z + 1 < size && !traversed[x, y, z + 1]) {
            return true;
        }
        return false;
    }

    void GenerateGrid() {
        GameObject GridFloor = new GameObject("Grid Floor"), Ceiling = new GameObject("Ceiling");
        GridFloor.transform.parent = HalfMaze.transform;
        for (int i = 0; i < size / 2; ++i) { //create walls from "top down" e.g. --------, later I will do | | | |
            for (int j = 0; j <= size; ++j) { //i is x, and j is z
                Instantiate(wall, new Vector3(i, 0, j), Quaternion.identity, GridFloor.transform);
            }
        }
        for (float i = -0.5f; i <= size / 2 - 0.5f; ++i) {
            for (float j = 0.5f; j < size; ++j) {
                Instantiate(wall, new Vector3(i, 0, j), Quaternion.Euler(0, 90, 0), GridFloor.transform);
            }
        }
        for (int i = 0; i < size / 2; ++i) {
            for (float j = 0.5f; j < size; ++j) {
                Instantiate(wall, new Vector3(i, -0.5f, j), Quaternion.Euler(90, 0, 0), GridFloor.transform);
            }
        }
        for (int i = 0; i < size / 2; ++i) {
            for (float j = 0.5f; j < size; ++j) {
                Instantiate(wall, new Vector3(i, size - 0.5f, j), Quaternion.Euler(90, 0, 0), Ceiling.transform);
            }
        }
        for (int i = 1; i < size; ++i) {
            Instantiate(GridFloor, new Vector3(0, i, 0), Quaternion.identity, HalfMaze.transform);
        }
        Ceiling.transform.parent = HalfMaze.transform;
        foreach (Transform child in HalfMaze.transform) {
            foreach (Transform grandchild in child) {
                if (!(grandchild.position.y == -0.5f || grandchild.position.y == -0.5f + size || grandchild.position.z == 0 || grandchild.position.z == size)) { //not including z == 0 and z == size because we want to destroy one tile in the front and back
                    wallPositions[grandchild.position] = grandchild.gameObject;
                }
            }
        }
    }

    void GenerateMaze(int x, int y, int z) {
        traversed[x, y, z] = true;
        while (LeftMoveIsValid(x, y, z) || RightMoveIsValid(x, y, z) || DownMoveIsValid(x, y, z) || UpMoveIsValid(x, y, z) || BackwardMoveIsValid(x, y, z) || ForwardMoveIsValid(x, y, z)) {
            bool validMove = false;
            while (!validMove) {
                int random = Random.Range(1, 7);
                switch(random) {
                    case 1:
                        if (LeftMoveIsValid(x, y, z)) { //destroy object here
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x - 1.5f, y, z + 0.5f)]);
                            GenerateMaze(x - 1, y, z);
                        }
                        break;
                    case 2:
                        if (RightMoveIsValid(x, y, z)) {
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x + 0.5f, y, z + 0.5f)]);
                            GenerateMaze(x + 1, y, z);
                        }
                        break;
                    case 3:
                        if (DownMoveIsValid(x, y, z)) {
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x, y - 0.5f, z + 0.5f)]);
                            GenerateMaze(x, y - 1, z);
                        }
                        break;
                    case 4:
                        if (UpMoveIsValid(x, y, z)) {
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x, y + 0.5f, z + 0.5f)]);
                            GenerateMaze(x, y + 1, z);
                        }
                        break;
                    case 5:
                        if (ForwardMoveIsValid(x, y, z)) {
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x, y, z)]);
                            GenerateMaze(x, y, z - 1);
                        }
                        break;
                    case 6:
                        if (BackwardMoveIsValid(x, y, z)) {
                            validMove = true;
                            Destroy(wallPositions[new Vector3(x, y, z + 1)]);
                            GenerateMaze(x, y, z + 1);
                        }
                        break;
                }
            }
        }
    }
    void Start()
    {
        HalfMaze = new GameObject("Half Maze");
        traversed = new bool[size / 2, size, size];
        GenerateGrid();
        int startY = Random.Range(0, size), startZ = Random.Range(0, size), endY = Random.Range(0, size), endZ = Random.Range(0, size);
        wallPositions[new Vector3(-0.5f, startY, startZ + 0.5f)].transform.parent = null;
        Destroy(wallPositions[new Vector3(-0.5f, startY, startZ + 0.5f)]);
        GenerateMaze(0, startY, startZ);
        wallPositions[new Vector3(size / 2 - 0.5f, endY, endZ + 0.5f)].transform.parent = null;
        Destroy(wallPositions[new Vector3(size / 2 - 0.5f, endY, endZ + 0.5f)]);
        Instantiate(finalWall, new Vector3(size / 2 - 0.5f, endY, endZ + 0.5f), Quaternion.Euler(0, 90, 0), HalfMaze.transform);
        HalfMaze.transform.rotation = Quaternion.Euler(0, 180, 0);
        GameObject HalfMaze2 = Instantiate(HalfMaze, new Vector3(-size + 1, 0, 0), Quaternion.Euler(0, 180, 0));
        HalfMaze2.transform.localScale = new Vector3(-1, 1, 1);
    }
}
