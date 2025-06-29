using TMPro;
using UnityEngine;
// 或者使用 TMPro

namespace Pet
{
    public class SpeechBubble : MonoBehaviour
    {
        public Transform target; // 要跟随的目标
        public Vector3 offset = new Vector3(0, 2f, 0); // 在目标头顶偏移

        public Canvas canvas; // 画布
        public RectTransform bubbleRect; // 气泡的 RectTransform
        public TMP_Text bubbleText; // 气泡里的文字

        private Camera mainCamera;
        
        public float showTime = 2f; // 气泡显示多久（秒）
        private float timer = 0f;


        void Start()
        {
            mainCamera = Camera.main;
            bubbleRect.gameObject.SetActive(false); // 初始隐藏
        }

        void Update()
        {
            if (target == null) return;

            // 更新位置
            Vector3 worldPos = target.position + offset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            bubbleRect.position = screenPos;

            // 计时关闭
            if (bubbleRect.gameObject.activeSelf)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    bubbleRect.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 设置说话内容，显示气泡，重置计时
        /// </summary>
        public void SetText(string text)
        {
            bubbleText.text = text;
            bubbleRect.gameObject.SetActive(true);
            timer = showTime;
        }
    }
}