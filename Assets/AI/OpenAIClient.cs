using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// 用于构造聊天消息体
[Serializable]
public class ChatMessage {
    public string role;
    public string content;
    public ChatMessage(string role, string content) {
        this.role = role;
        this.content = content;
    }
}

// 用于构造请求体
[Serializable]
public class ChatRequest {
    public string model = "deepseek-chat";  // 可根据需要修改为其他模型
    public ChatMessage[] messages;
    public ChatRequest(ChatMessage[] messages) {
        this.messages = messages;
    }
}

// 用于解析回应
[Serializable]
public class ChatChoice {
    public int index;
    public ChatMessage message;
}

[Serializable]
public class ChatResponse {
    public ChatChoice[] choices;
}

public class OpenAIClient : MonoBehaviour {
    [Header("OpenAI 设置")]
    [Tooltip("在此处填入你的 OpenAI API Key")]  
    [SerializeField]
    private string apiKey;

    /// <summary>
    /// 发送聊天补全请求
    /// </summary>
    /// <param name="userPrompt">用户输入内容</param>
    /// <param name="callback">收到回复后的回调，返回字符串内容</param>
    public IEnumerator SendChatCompletion(string userPrompt, Action<string> callback) {
        // 构造消息列表，当前仅包含用户角色消息
        var messages = new ChatMessage[] { new ChatMessage("user", userPrompt) };
        var requestData = new ChatRequest(messages);
        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest("https://api.deepseek.com/v1/chat/completions", "POST")) {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // 设置请求头
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // 发送请求并等待完成
            yield return request.SendWebRequest();
            bool isError = request.result != UnityWebRequest.Result.Success;

            if (isError) {
                // —— 新增：打印服务器返回 body —— 
                string errorBody = request.downloadHandler.text;
                Debug.LogError($"[OpenAI] HTTP Error: {request.error}\nResponse Body: {errorBody}");
                callback?.Invoke(null);
            } else {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                callback?.Invoke(response.choices[0].message.content);
            }

        }
    }
}
