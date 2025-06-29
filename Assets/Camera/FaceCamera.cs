using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Tooltip("需要朝向相机的物体标签")]
    public string targetTag = "Pet";

    [Tooltip("指定物体的哪个面作为正面（默认Z轴）")]
    public Vector3 objectFront = Vector3.forward;

    private Transform camTransform;

    void Start()
    {
        Check();
    }

    public void Check()
    {
        camTransform = Camera.main.transform;
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in targets)
        {
            obj.AddComponent<SingleObjectFaceCamera>().Setup(objectFront);
        }
    }

    public void Add(Transform root)
    {
        root.Find("Body").gameObject.AddComponent<SingleObjectFaceCamera>().Setup(objectFront);
    }
}
