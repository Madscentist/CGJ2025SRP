using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Tooltip("��Ҫ��������������ǩ")]
    public string targetTag = "Pet";

    [Tooltip("ָ��������ĸ�����Ϊ���棨Ĭ��Z�ᣩ")]
    public Vector3 objectFront = Vector3.forward;

    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in targets)
        {
            obj.AddComponent<SingleObjectFaceCamera>().Setup(objectFront);
        }
    }
}
