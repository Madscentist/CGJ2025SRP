using UnityEngine;

public class SingleObjectFaceCamera : MonoBehaviour
{
    public Vector3 frontDirection; // 物体本身正面方向
    public Transform camTransform; // 相机
    public Transform sphereCenter; // 球心（必须指定）

    public void Setup(Vector3 front)
    {
        frontDirection = front.normalized;
        camTransform = UnityEngine.Camera.main.transform;
        sphereCenter = FindObjectOfType<Gravety2Sphere>().transform;
    }

    void LateUpdate()
    {
        if (camTransform == null || sphereCenter == null) return;

        // 计算从球心到物体位置的“法线”
        Vector3 normal = (transform.position - sphereCenter.position).normalized;

        // 让物体先以球面法线为“up”朝向相机
        Quaternion targetRotation = Quaternion.LookRotation(
            camTransform.position - transform.position,
            normal
        );

        // 调整自定义前向
        transform.rotation = targetRotation * Quaternion.Inverse(Quaternion.LookRotation(frontDirection, Vector3.up));
    }
}