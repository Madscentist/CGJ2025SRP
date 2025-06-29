using UnityEngine;
using Cinemachine;

public class CameraBlend : MonoBehaviour
{
    [Header("相机")]
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;

    [Header("混合速度")]
    public float blendSpeed = 1f;

    [Header("vcam1 距离范围")]
    public float minRange = 20f;
    public float maxRange = 50f;

    [Header("天空盒")]
    public Material skybox1;
    public Material skybox2;

    private float blendWeight = 0f;

    private Transform vcam1Transform;
    private float targetSkyBoxWeight = 0f;
    private float currentSkyBoxWeight = 0f;

    void Start()
    {
        if (vcam1 != null)
            vcam1Transform = vcam1.transform;
    }

    void Update()
    {
        // 鼠标滚轮输入
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        blendWeight = Mathf.Clamp01(blendWeight + scroll * blendSpeed);

        // 根据 blendWeight 设置 vcam1 的 z 轴（这里以 localPosition.z 举例）
        if (vcam1Transform != null)
        {
            float distance = Mathf.Lerp(minRange, maxRange, 1f - blendWeight);
            vcam1Transform.localPosition = new Vector3(
                vcam1Transform.localPosition.x,
                vcam1Transform.localPosition.y,
                -distance // 假设相机 forward 向 -Z
            );
        }

        // 优先级控制
        vcam1.Priority = blendWeight < 1f ? 10 : 0;
        vcam2.Priority = blendWeight >= 1f ? 10 : 0;

        targetSkyBoxWeight = blendWeight < 1f ? 0f : 1f;
        currentSkyBoxWeight = Mathf.Lerp(currentSkyBoxWeight, targetSkyBoxWeight, 1f * Time.deltaTime);


        if (targetSkyBoxWeight == 0f)
        {
            RenderSettings.skybox = skybox1;

        }else if (Mathf.Approximately(targetSkyBoxWeight, 1f))
        {

            if (Mathf.Abs(targetSkyBoxWeight - currentSkyBoxWeight) < 0.01f)
            {
                RenderSettings.skybox = skybox2;
            }
            else
            {
                RenderSettings.skybox.Lerp(skybox1, skybox2, currentSkyBoxWeight);

            }
            
        }
    }
}