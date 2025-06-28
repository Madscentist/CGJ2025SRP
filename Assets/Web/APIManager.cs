// ApiManager.cs

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Web
{
    public class APIManager : MonoBehaviour
    {
        public static APIManager Instance { get; private set; }

        void Awake()
        {
            // 单例模式
            if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);
        }

        /// <summary>
        /// POST 文字，调用后端文字处理接口
        /// </summary>
        public void ProcessText(string prompt, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(ProcessTextCoroutine(prompt, onSuccess, onError));
        }

        private IEnumerator ProcessTextCoroutine(string prompt, Action<string> onSuccess, Action<string> onError)
        {
            string url = ApiConfig.BaseUrl + ApiConfig.TextEndpoint;
            // 构造请求体
            var payload = new { prompt = prompt };
            string json = JsonUtility.ToJson(payload);
            byte[] body = Encoding.UTF8.GetBytes(json);

            using (var req = new UnityWebRequest(url, "POST"))
            {
                req.uploadHandler   = new UploadHandlerRaw(body);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.SetRequestHeader("Accept", "application/json");

                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    onError?.Invoke($"[{req.responseCode}] {req.error}");
                else
                {
                    try
                    {
                        var resp = JsonUtility.FromJson<TextResponse>(req.downloadHandler.text);
                        if (resp.status == 200)
                            onSuccess?.Invoke(resp.result);
                        else
                            onError?.Invoke($"Server error: {resp.status}");
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke("解析失败: " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// POST 图片，上传 Texture2D 并接收处理后图片
        /// </summary>
        public void ProcessImage(Texture2D tex, Action<Texture2D> onSuccess, Action<string> onError)
        {
            StartCoroutine(ProcessImageCoroutine(tex, onSuccess, onError));
        }

        private IEnumerator ProcessImageCoroutine(Texture2D tex, Action<Texture2D> onSuccess, Action<string> onError)
        {
            string url = ApiConfig.BaseUrl + ApiConfig.ImageEndpoint;
            byte[] pngData = tex.EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddBinaryData("image", pngData, "upload.png", "image/png");

            using (var req = UnityWebRequest.Post(url, form))
            {
                // 如果后端直接返回图片流
                req.downloadHandler = new DownloadHandlerTexture();
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    onError?.Invoke($"[{req.responseCode}] {req.error}");
                else
                {
                    Texture2D outTex = DownloadHandlerTexture.GetContent(req);
                    onSuccess?.Invoke(outTex);
                }
            }
        }

        /// <summary>
        /// GET 拉取纯文本（示例）
        /// </summary>
        public void FetchText(string relativePath, Action<string> onSuccess, Action<string> onError)
        {
            StartCoroutine(FetchTextCoroutine(relativePath, onSuccess, onError));
        }

        private IEnumerator FetchTextCoroutine(string relativePath, Action<string> onSuccess, Action<string> onError)
        {
            string url = ApiConfig.BaseUrl + relativePath;
            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                    onError?.Invoke(req.error);
                else
                    onSuccess?.Invoke(req.downloadHandler.text);
            }
        }

        /// <summary>
        /// GET 拉取图片（示例）
        /// </summary>
        public void FetchImage(string relativePath, Action<Texture2D> onSuccess, Action<string> onError)
        {
            StartCoroutine(FetchImageCoroutine(relativePath, onSuccess, onError));
        }

        private IEnumerator FetchImageCoroutine(string relativePath, Action<Texture2D> onSuccess, Action<string> onError)
        {
            string url = ApiConfig.BaseUrl + relativePath;
            using (var req = UnityWebRequestTexture.GetTexture(url))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                    onError?.Invoke(req.error);
                else
                    onSuccess?.Invoke(DownloadHandlerTexture.GetContent(req));
            }
        }

        // 用于 JSON 反序列化
        [Serializable]
        private class TextResponse
        {
            public int    status;
            public string result;
        }
    }
}
