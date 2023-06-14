using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoundries : MonoBehaviour
{
    public bool gameOver = false;
    bool player1Found = false, player2Found = false;
    GameObject player1, player2, CanvasManager, Canvas;

    private void ActivateCanvas(int player) {
        BasicMovement.gameOver = true;
        CanvasManager = GameObject.Find("CanvasManager");
        CanvasManager.GetComponent<ActivateCanvas>().activeCanvas = true;
        if (player == 1) {
            CanvasManager.GetComponent<ActivateCanvas>().player1Won = true;
        } else {
            CanvasManager.GetComponent<ActivateCanvas>().player2Won = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name != "Player [connId=0]" || other.name != "Wall") {
            ActivateCanvas(1); //0 is player 1
        } else if (other.name == "Player [connId=0]") {
            ActivateCanvas(0); //1 is player 2
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!player1Found) //need better way to access the player1 and player 2 objects, must have the same if statement for player2 as well
            if (GameObject.Find("Player [connId=0]") != null) {
                player1 = GameObject.Find("Player [connId=0]");
                player1Found = true;
            }
        if (!player2Found) {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            if (temp.Length == 2) {
                player2 = temp[1];
                player2Found = true;
            }
        }
    }
}
