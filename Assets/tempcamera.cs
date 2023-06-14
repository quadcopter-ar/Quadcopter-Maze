using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempcamera : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject parent;
    void Start()
    {
        this.transform.parent = parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
