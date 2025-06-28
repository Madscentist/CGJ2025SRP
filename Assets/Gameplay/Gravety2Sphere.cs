using UnityEngine;

public class Gravety2Sphere : MonoBehaviour
{
    [Header("重力参数")]
    public float gravityForce = 9.81f; // 重力强度
    public Transform centerOfMass;    // 球心（默认为脚本挂载对象）

    private void Awake()
    {
        if (centerOfMass == null) centerOfMass = transform;
        Physics.gravity = Vector3.zero; // 禁用默认重力
    }

    private void FixedUpdate()
    {
        // 获取场景中所有Rigidbody
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb == centerOfMass.GetComponent<Rigidbody>()) continue; // 跳过球体本身

            // 计算指向球心的方向
            Vector3 directionToCenter = (centerOfMass.position - rb.position).normalized;

            // 应用重力（使用物理引擎的AddForce）
            rb.AddForce(directionToCenter * gravityForce, ForceMode.Acceleration);

            // 可选：使物体始终"站立"在球面
            AlignToSurface(rb);
        }
    }

    // 使物体垂直于球面（如角色站立）
    private void AlignToSurface(Rigidbody rb)
    {
        Vector3 surfaceNormal = (rb.position - centerOfMass.position).normalized;
        rb.rotation = Quaternion.FromToRotation(rb.transform.up, surfaceNormal) * rb.rotation;
    }
}