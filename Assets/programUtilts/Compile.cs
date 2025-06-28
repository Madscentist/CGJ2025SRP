using UnityEngine;
using System.Collections.Generic;

public class AdvancedObjectRotation : MonoBehaviour
{
    [Header("Settings")]
    public string selectableTag = "Selectable"; // ��ѡ�ж���ı�ǩ
    public float clickRotationSpeed = 90f;     // �����ת�ٶ�
    public float dragRotationSpeed = 2f;       // ��ק��ת�ٶ�
    public float dragThreshold = 0.3f;         // ��ק������ֵ���룩

    [Header("References")]
    public GameObject instanceB;               // ��ת����ʵ��B

    private GameObject selectedObject;         // ��ǰѡ�е�ʵ��A
    private bool isDragging;                   // �Ƿ�������ק
    private float rightClickHoldTime;          // �Ҽ���סʱ��
    private Vector2 dragStartMousePos;         // ��ק��ʼ���λ��
    private List<Transform> linkedObjects = new List<Transform>(); // ������Aͬ��ǩ�Ķ��󣨲�����A��B��

    void Update()
    {
        HandleLeftClickSelection();
        HandleRightClickActions();
    }

    void HandleLeftClickSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag(selectableTag))
                {
                    SelectObject(hit.collider.gameObject);
                }
                else
                {
                    DeselectObject();
                }
            }
            else
            {
                DeselectObject();
            }
        }
    }

    void HandleRightClickActions()
    {
        // �Ҽ�����ʱ��¼ʱ��
        if (Input.GetMouseButtonDown(1))
        {
            rightClickHoldTime = 0f;
            dragStartMousePos = Input.mousePosition;
        }

        // �Ҽ�������ס
        if (Input.GetMouseButton(1))
        {
            rightClickHoldTime += Time.deltaTime;

            // ������ֵ��ʼ��ק
            if (rightClickHoldTime >= dragThreshold && !isDragging)
            {
                isDragging = true;
                CacheLinkedObjects();
            }
        }

        // �Ҽ��ͷ�
        if (Input.GetMouseButtonUp(1))
        {
            if (!isDragging && selectedObject != null) // �̰���ת
            {
                RotateWithLinkedObjects(clickRotationSpeed);
            }
            isDragging = false;
        }

        // ��ק��ת
        if (isDragging && selectedObject != null)
        {
            Vector2 currentMousePos = Input.mousePosition;
            float angleDelta = (currentMousePos.x - dragStartMousePos.x) * dragRotationSpeed;
            RotateWithLinkedObjects(angleDelta);
            dragStartMousePos = currentMousePos;
        }
    }

    // ����������Ҫ������ת�Ķ���ͬ��ǩ��������ѡ�е�A����ת����B��
    void CacheLinkedObjects()
    {
        linkedObjects.Clear();
        GameObject[] allTaggedObjects = GameObject.FindGameObjectsWithTag(selectableTag);

        foreach (GameObject obj in allTaggedObjects)
        {
            if (obj != selectedObject && obj != instanceB)
            {
                // ��¼ÿ�����������B�ı���λ�ú���ת
                linkedObjects.Add(obj.transform);
            }
        }
    }

    // ��תʵ��B�����������
    void RotateWithLinkedObjects(float degrees)
    {
        if (instanceB == null) return;

        // 1. ��¼��תǰ��λ�ù�ϵ
        Vector3 bPosition = instanceB.transform.position;
        Quaternion bRotation = instanceB.transform.rotation;

        // 3. ���ֹ����������λ�ò���
        foreach (Transform objTransform in linkedObjects)
        {
            if (objTransform != null)
            {
                // ���������B�ı���λ�ú���ת
                Vector3 localPos = bRotation * Quaternion.Inverse(bRotation) * (objTransform.position - bPosition);
                Quaternion localRot = Quaternion.Inverse(bRotation) * objTransform.rotation;

                // Ӧ����ת�����λ��
                objTransform.position = instanceB.transform.rotation * localPos + instanceB.transform.position;
                objTransform.rotation = instanceB.transform.rotation * localRot;
            }
        }
    }

    void SelectObject(GameObject obj)
    {
        DeselectObject();
        selectedObject = obj;
        Debug.Log($"Selected: {obj.name}", obj);

        // ���ӻ�ѡ��Ч������ѡ��
        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.green;
        }
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            // �ָ�ԭ��ۣ���ѡ��
            if (selectedObject.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.material.color = Color.white;
            }

            Debug.Log($"Deselected: {selectedObject.name}");
            selectedObject = null;
        }
        linkedObjects.Clear();
    }
}