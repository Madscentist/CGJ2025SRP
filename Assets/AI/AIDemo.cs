using UnityEngine;
using TMPro;  // 如果用 TextMeshPro

public class AIDemo : MonoBehaviour
{
    [Header("引用")]
    public OpenAIClient openAIClient;      // 拖拽 OpenAIManager
    public TMP_InputField inputField;      // 用户输入框
    public TMP_Text outputText;            // 显示 AI 回复的文本框

    private void Start()
    {
        // 可选：自动发送一条开场白
        // StartChat("你好，AI！");
    }

    public void OnSendButtonClicked()
    {
        string prompt = inputField.text.Trim();
        if (string.IsNullOrEmpty(prompt)) return;
        StartChat(prompt);
    }

    private void StartChat(string prompt)
    {
        // 禁用输入，避免重复点击
        inputField.interactable = false;

        StartCoroutine(openAIClient.SendChatCompletion(prompt, response =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                // 显示到 UI
                outputText.text = response;
            }
            else
            {
                outputText.text = "<color=red>请求失败，请检查网络或 API Key</color>";
            }
            inputField.interactable = true;
        }));
    }
}