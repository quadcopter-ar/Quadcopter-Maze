using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateCanvas : MonoBehaviour
{
    public GameObject canvas;
    public GameObject TextObject;
    public bool activeCanvas = false, player1Won = false, player2Won = false, displayed = false;

    void Start() {
        canvas.SetActive(false);
    }
    void Update()
    {
        if (activeCanvas) {
            canvas.SetActive(true);
            if (!displayed) {
                if (player1Won == true) {
                    TextObject.GetComponent<Text>().text = "PLAYER 1 WON";
                } else {
                    TextObject.GetComponent<Text>().text = "PLAYER 2 WON";
                }
                displayed = true;
            }
        }
    }
}
