using UnityEngine;

public class rightDragging : MonoBehaviour
{
    public float offsetLimit;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) // 1 表示右键
        {
            // 计算鼠标点击点到物体中心的偏移
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            if (Vector3.Distance(mouseWorldPos, transform.position) < offsetLimit)
            {
                transform.position = mouseWorldPos;
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z; // 保持深度
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}