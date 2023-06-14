using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Canvas temp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() //this is very inefficient, once gameobject.find finds the camera, the script should be turned off
    {
        temp.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        temp.planeDistance = 1;
    }
}
