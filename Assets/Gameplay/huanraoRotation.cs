using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class huanraoRotation : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject huanrao;
    public float rotationSpeed;
    public Vector3 pivot = new Vector3(-2.9f, 2.6f, -4.1f);
    void Start()
    {
        huanrao = GameObject.Find("huan3");
    }

    // Update is called once per frame
    void Update()
    {
        huanrao.transform.Rotate(pivot, rotationSpeed*Time.deltaTime, Space.World);
    }
}
