using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class HandleMultiplayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    bool cameraHandled = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() //super inefficient but does the job :P
    {
        if (!cameraHandled) {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            if (temp.Length == 2) {
                GameObject player1 = GameObject.Find("Player [connId=0]");
                Object.Destroy(player1.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject);
                GameObject camera = new GameObject("camera");
                camera.AddComponent<Camera>();
                camera.transform.SetParent(player1.transform.GetChild(0).transform.GetChild(0).transform);
                camera.AddComponent<TrackedPoseDriver>();
                camera.GetComponent<Camera>().nearClipPlane = 0.01f;
                cameraHandled = true;
            }
        }
    }
}
