using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Gameplay
{
    public class PetCustomizer : MonoBehaviour
    {
        public float headSize = 1f;
        [SerializeField] private SpriteRenderer headRenderer; // 需要换贴图的部位
        [SerializeField] private string petTextureUrl; // 从后端 JSON 里取


        public void Config(string path)
        {
            petTextureUrl = path;
            SetAvatar();
        }

        private void Start()
        {
            // SetAvatar();
        }

        public void SetAvatar()
        {
            LoadTextureFromPath(petTextureUrl);
        }

        private void LoadTextureFromPath(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"文件不存在: {path}");
                return;
            }

            // 1️⃣ 读取字节
            byte[] fileData = File.ReadAllBytes(path);

            // 2️⃣ 创建贴图并载入
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false); // 尺寸会被 LoadImage 自动替换
            tex.LoadImage(fileData);
            
            float ppu = tex.width / headSize;
            
            var spr = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 
                ppu
            );


            // targetImage.sprite = spr;
            // targetImage.SetNativeSize();
            // 3️⃣ 赋给材质
            headRenderer.sprite = spr;

            // （可选）如果模型只想改某张特定贴图槽位：
            // targetRenderer.material.SetTexture("_BaseMap", tex); // URP 标准材质
        }
    }
}