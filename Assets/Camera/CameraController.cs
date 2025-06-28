using UnityEngine;

/// <summary>
/// 鼠标滚轮控制相机远近，并在一段时间无操作后自动恢复到默认距离。
/// 将脚本挂在需要移动的相机上。
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("基本参数")]
    [Tooltip("相机注视的目标Transform。如果为空，则以世界原点为中心。")]
    public Transform target;

    [Tooltip("无操作时恢复的默认距离 (单位：m)")] public float defaultDistance = 10f;
    [Tooltip("允许的最近距离")]        public float minDistance = 2f;
    [Tooltip("允许的最远距离")]        public float maxDistance = 30f;

    [Header("滚轮缩放")]
    [Tooltip("滚轮缩放速度系数")]       public float zoomSpeed = 5f;

    [Header("自动复位")]
    [Tooltip("无操作多久(秒)开始恢复")] public float resetDelay = 3f;
    [Tooltip("恢复到默认距离的插值速度")] public float resetSmoothSpeed = 2f;

    private float currentDistance;
    private float inactivityTimer;

    void Start()
    {
        currentDistance = defaultDistance;
    }

    void Update()
    {
        // 读取滚轮输入
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            // 根据滚轮缩放
            currentDistance = Mathf.Clamp(currentDistance - scroll * zoomSpeed, minDistance, maxDistance);
            inactivityTimer = 0f; // 重置计时器
        }
        else
        {
            // 统计无操作的时间
            inactivityTimer += Time.deltaTime;

            // 达到延迟后，向默认距离缓动
            if (inactivityTimer >= resetDelay)
            {
                currentDistance = Mathf.Lerp(currentDistance, defaultDistance, Time.deltaTime * resetSmoothSpeed);
            }
        }
    }

    void LateUpdate()
    {
        // 根据当前距离，沿相机forward方向摆放位置
        Vector3 focusPoint = target ? target.position : Vector3.zero;
        transform.position = focusPoint - transform.forward * currentDistance;
    }
}
