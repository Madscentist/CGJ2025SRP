using UnityEngine;
using System.Collections.Generic;

public class AdvancedObjectRotation : MonoBehaviour
{
    [Header("Settings")]
    public string selectableTag = "Selectable"; // 可选中对象的标签
    public float clickRotationSpeed = 90f;     // 点击旋转速度
    public float dragRotationSpeed = 2f;       // 拖拽旋转速度
    public float dragThreshold = 0.3f;         // 拖拽触发阈值（秒）

    [Header("References")]
    public GameObject instanceB;               // 旋转中心实例B

    private GameObject selectedObject;         // 当前选中的实例A
    private bool isDragging;                   // 是否正在拖拽
    private float rightClickHoldTime;          // 右键按住时长
    private Vector2 dragStartMousePos;         // 拖拽起始鼠标位置
    private List<Transform> linkedObjects = new List<Transform>(); // 所有与A同标签的对象（不包括A和B）

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
        // 右键按下时记录时间
        if (Input.GetMouseButtonDown(1))
        {
            rightClickHoldTime = 0f;
            dragStartMousePos = Input.mousePosition;
        }

        // 右键持续按住
        if (Input.GetMouseButton(1))
        {
            rightClickHoldTime += Time.deltaTime;

            // 超过阈值开始拖拽
            if (rightClickHoldTime >= dragThreshold && !isDragging)
            {
                isDragging = true;
                CacheLinkedObjects();
            }
        }

        // 右键释放
        if (Input.GetMouseButtonUp(1))
        {
            if (!isDragging && selectedObject != null) // 短按旋转
            {
                RotateWithLinkedObjects(clickRotationSpeed);
            }
            isDragging = false;
        }

        // 拖拽旋转
        if (isDragging && selectedObject != null)
        {
            Vector2 currentMousePos = Input.mousePosition;
            float angleDelta = (currentMousePos.x - dragStartMousePos.x) * dragRotationSpeed;
            RotateWithLinkedObjects(angleDelta);
            dragStartMousePos = currentMousePos;
        }
    }

    // 缓存所有需要跟随旋转的对象（同标签，不包括选中的A和旋转中心B）
    void CacheLinkedObjects()
    {
        linkedObjects.Clear();
        GameObject[] allTaggedObjects = GameObject.FindGameObjectsWithTag(selectableTag);

        foreach (GameObject obj in allTaggedObjects)
        {
            if (obj != selectedObject && obj != instanceB)
            {
                // 记录每个对象相对于B的本地位置和旋转
                linkedObjects.Add(obj.transform);
            }
        }
    }

    // 旋转实例B及其关联对象
    void RotateWithLinkedObjects(float degrees)
    {
        if (instanceB == null) return;

        // 1. 记录旋转前的位置关系
        Vector3 bPosition = instanceB.transform.position;
        Quaternion bRotation = instanceB.transform.rotation;

        // 3. 保持关联对象相对位置不变
        foreach (Transform objTransform in linkedObjects)
        {
            if (objTransform != null)
            {
                // 计算相对于B的本地位置和旋转
                Vector3 localPos = bRotation * Quaternion.Inverse(bRotation) * (objTransform.position - bPosition);
                Quaternion localRot = Quaternion.Inverse(bRotation) * objTransform.rotation;

                // 应用旋转后的新位置
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

        // 可视化选中效果（可选）
        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.color = Color.green;
        }
    }

    void DeselectObject()
    {
        if (selectedObject != null)
        {
            // 恢复原外观（可选）
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