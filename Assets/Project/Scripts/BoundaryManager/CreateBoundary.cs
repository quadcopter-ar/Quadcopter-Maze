using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoundary : MonoBehaviour
{
    // Start is called before the first frame update
    public float length, width, height;
    public Material material;
    void Start()
    {
        DontDestroyOnLoad(this.transform);

        GameObject lWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lWall.transform.localScale = new Vector3(length, height, 1);
        lWall.transform.position = new Vector3(length / 2.0f, height / 2.0f, 0);
        GameObject lWall2 = GameObject.Instantiate(lWall);
        lWall2.transform.position = new Vector3(length / 2.0f, height / 2.0f, width);

        GameObject wWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wWall.transform.localScale = new Vector3(1, height, width);
        wWall.transform.position = new Vector3(0, height / 2.0f, width / 2.0f);
        GameObject wWall2 = GameObject.Instantiate(wWall);
        wWall2.transform.position = new Vector3(length, height / 2.0f, width / 2.0f);

        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.transform.localScale = new Vector3(length, 1, width);
        ceiling.transform.position = new Vector3(length / 2.0f, height, width / 2.0f);
        GameObject floor = GameObject.Instantiate(ceiling);
        floor.transform.position = new Vector3(length / 2.0f, 0.01f, width / 2.0f);

        /*lWall.GetComponent<MeshRenderer>().enabled = false;
        lWall2.GetComponent<MeshRenderer>().enabled = false;
        wWall.GetComponent<MeshRenderer>().enabled = false;
        wWall2.GetComponent<MeshRenderer>().enabled = false;
        ceiling.GetComponent<MeshRenderer>().enabled = false;
        floor.GetComponent<MeshRenderer>().enabled = false;*/

        lWall.GetComponent<Renderer>().material = material;
        lWall2.GetComponent<Renderer>().material = material;
        wWall.GetComponent<Renderer>().material = material;
        wWall2.GetComponent<Renderer>().material = material;
        ceiling.GetComponent<Renderer>().material = material;
        floor.GetComponent<Renderer>().material = material;

        lWall.AddComponent<Rigidbody>();
        lWall2.AddComponent<Rigidbody>();
        wWall.AddComponent<Rigidbody>();
        wWall2.AddComponent<Rigidbody>();
        ceiling.AddComponent<Rigidbody>();
        floor.AddComponent<Rigidbody>();

        lWall.GetComponent<Rigidbody>().isKinematic = true;
        lWall2.GetComponent<Rigidbody>().isKinematic = true;
        wWall.GetComponent<Rigidbody>().isKinematic = true;
        wWall2.GetComponent<Rigidbody>().isKinematic = true;
        ceiling.GetComponent<Rigidbody>().isKinematic = true;
        floor.GetComponent<Rigidbody>().isKinematic = true;

        lWall.GetComponent<Rigidbody>().useGravity = false;
        lWall2.GetComponent<Rigidbody>().useGravity = false;
        wWall.GetComponent<Rigidbody>().useGravity = false;
        wWall2.GetComponent<Rigidbody>().useGravity = false;
        ceiling.GetComponent<Rigidbody>().useGravity = false;
        floor.GetComponent<Rigidbody>().useGravity = false;

        lWall.transform.parent = this.transform;
        lWall2.transform.parent = this.transform;
        wWall.transform.parent = this.transform;
        wWall2.transform.parent = this.transform;
        ceiling.transform.parent = this.transform;
        floor.transform.parent = this.transform;
    }
}
