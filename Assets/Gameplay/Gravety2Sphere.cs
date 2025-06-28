using UnityEngine;

public class Gravety2Sphere : MonoBehaviour
{
    [Header("��������")]
    public float gravityForce = 9.81f; // ����ǿ��
    public Transform centerOfMass;    // ���ģ�Ĭ��Ϊ�ű����ض���

    private void Awake()
    {
        if (centerOfMass == null) centerOfMass = transform;
        Physics.gravity = Vector3.zero; // ����Ĭ������
    }

    private void FixedUpdate()
    {
        // ��ȡ����������Rigidbody
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb == centerOfMass.GetComponent<Rigidbody>()) continue; // �������屾��

            // ����ָ�����ĵķ���
            Vector3 directionToCenter = (centerOfMass.position - rb.position).normalized;

            // Ӧ��������ʹ�����������AddForce��
            rb.AddForce(directionToCenter * gravityForce, ForceMode.Acceleration);

            // ��ѡ��ʹ����ʼ��"վ��"������
            AlignToSurface(rb);
        }
    }

    // ʹ���崹ֱ�����棨���ɫվ����
    private void AlignToSurface(Rigidbody rb)
    {
        Vector3 surfaceNormal = (rb.position - centerOfMass.position).normalized;
        rb.rotation = Quaternion.FromToRotation(rb.transform.up, surfaceNormal) * rb.rotation;
    }
}