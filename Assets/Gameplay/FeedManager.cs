using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Pet;

namespace Gameplay
{
    public class FeedManager : MonoBehaviour
    {
        public string feedFilePath = "D:/GJ2025/Flask/feeds.json";

        public List<Pet.Pet> pets = new List<Pet.Pet>(); // 已经加载好的宠物列表

        private string lastFeedHash = "";

        void Start()
        {
            // 假设你的 pets 已经从别的地方加载好了
            StartCoroutine(CheckFeedFilePeriodically());
        }

        IEnumerator CheckFeedFilePeriodically()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                if (File.Exists(feedFilePath))
                {
                    string json = File.ReadAllText(feedFilePath);
                    string currentHash = GetMD5(json);

                    if (currentHash != lastFeedHash)
                    {
                        lastFeedHash = currentHash;

                        FeedList feedList = FeedUtil.DeserializeFeedList(json);
                        ProcessFeeds(feedList);
                    }
                }
            }
        }

        void ProcessFeeds(FeedList feedList)
        {
            Debug.Log("检测到 feed.json 有更新！");
            foreach (var feed in feedList.feeds)
            {
                Debug.Log($"处理 Feed：UUID={feed.uuid} Score={feed.score} Comments={feed.comments}");

                Pet.Pet pet = pets.Find(p => p.uuid == feed.uuid);
                if (pet != null)
                {
                    FeedPet(pet, feed);
                }
                else
                {
                    Debug.LogWarning($"未找到 UUID={feed.uuid} 的宠物！");
                }
            }
        }

        void FeedPet(Pet.Pet pet, FeedItem feed)
        {
            Debug.Log($"喂食宠物：{pet.name}，评分：{feed.score}，评论：{feed.comments}");
            // 👉 在这里实现你的喂食逻辑，如更新状态、调用动画、加经验值等等
        }

        string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in data) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
