using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Pet;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gameplay
{
    public class PetManager : MonoBehaviour
    {
        public Transform bornPlace;
        public Transform earth;
        public PetList petList;
        public string customFilePath;
        public string feedFilePath;
        public List<Pet.Pet> LivingPet = new List<Pet.Pet>();
        public List<GameObject> petPrefab;
        public List<PetObj> petObjs = new List<PetObj>();


        [Space(20)] public GameObject qrPlace;
        public Image qrCodeImage;
        public TMP_Text feedText;

        public Transform rankingListParent;
        public GameObject rankingItemPrefab;
        public int topN = 5;
        
        public void QuitQr()
        {
            qrPlace.SetActive(false);
        }
        
        public void SelectPet(GameObject petObj)
        {
            qrPlace.SetActive(true);
            // 在 petObjs 里找到对应的 uuid
            var match = petObjs.Find(p => p.obj == petObj);

            if (string.IsNullOrEmpty(match.uuid))
            {
                Debug.LogWarning("未找到对应的 PetObj！");
                return;
            }

            string uuid = match.uuid;

            // 找到 Pet
            Pet.Pet selectedPet = petList.pets.Find(p => p.uuid == uuid);
            if (selectedPet == null)
            {
                Debug.LogWarning($"未找到 UUID={uuid} 的宠物！");
                return;
            }

            // 加载二维码
            if (System.IO.File.Exists(selectedPet.qrPath))
            {
                byte[] imageData = System.IO.File.ReadAllBytes(selectedPet.qrPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);

                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                qrCodeImage.sprite = sprite;
                feedText.text = selectedPet.name;
                Debug.Log($"已加载 {selectedPet.name} 的二维码！");
            }
            else
            {
                Debug.LogWarning($"二维码图片不存在：{selectedPet.qrPath}");
            }
        }
        
        private string lastFeedHash = "";

        public struct PetObj
        {
            public string uuid;
            public GameObject obj; // 改为GameObject类型
        }

        string GetOrdinal(int number)
        {
            if (number % 100 >= 11 && number % 100 <= 13) return number + "th";
            switch (number % 10)
            {
                case 1: return number + "st";
                case 2: return number + "nd";
                case 3: return number + "rd";
                default: return number + "th";
            }
        }
        
        public void UpdateRanking()
        {
            // 先收集所有宠物和它们的 heavy
            var petDataList = new List<(PetObj petObj, int heavy)>();

            foreach (var p in petObjs)
            {
                var ctrl = p.obj.GetComponent<PetController>();
                if (ctrl != null)
                {
                    petDataList.Add((p, ctrl.heavy));
                }
            }

            // 按 heavy 降序排列
            petDataList.Sort((a, b) => b.heavy.CompareTo(a.heavy));

            // 最多显示 3 名
            int maxRank = 3;

            for (int i = 0; i < maxRank; i++)
            {
                Transform panel = rankingListParent.GetChild(i);

                var text = panel.GetComponentInChildren<TMPro.TMP_Text>();
                if (i < petDataList.Count)
                {
                    var data = petDataList[i];

                    var petInfo = petList.pets.Find(p => p.uuid == data.petObj.uuid);
                    string petName = petInfo != null ? petInfo.name : "Unknown";

                    string ordinal = GetOrdinal(i + 1);

                    text.text = $"{ordinal}: {petName} | {data.heavy}";
                }
                else
                {
                    text.text = $"{GetOrdinal(i + 1)}: -";
                }
            }

            Debug.Log("固定排行榜已更新！");
        }
        
        void Start()
        {
            StartCoroutine(LoadPetListPeriodically());
            StartCoroutine(CheckFeedFilePeriodically());
        }

        IEnumerator LoadPetListPeriodically()
        {
            while (true)
            {
                LoadPetListFromCustomPath();
                CheckForNewPets(); // 每次加载后检查新宠物
                yield return new WaitForSeconds(2f);
            }
        }

        void LoadPetListFromCustomPath()
        {
            if (File.Exists(customFilePath))
            {
                string json = File.ReadAllText(customFilePath);
                petList = PetUtil.DeserializePetList(json);
            }
            else
            {
                Debug.LogWarning("File not found at: " + customFilePath);
            }
        }

        public void ConstructPet(Pet.Pet pet)
        {
            if (petPrefab.Count > 0)
            {
                var petType = Random.Range(0, petPrefab.Count);
                var targetPetPrefab = petPrefab[petType]; // 修正拼写错误

                // 实例化宠物
                var newPetObj = Instantiate(targetPetPrefab, Vector3.zero, Quaternion.identity);
                newPetObj.transform.position = bornPlace.position;
                newPetObj.transform.localScale = 0.5f * Vector3.one;
                newPetObj.transform.parent = earth;
                newPetObj.GetComponent<PetCustomizer>().Config(pet.avatarPath);
                GetComponent<FaceCamera>().Add(newPetObj.transform);
                
                // 添加到petObjs列表
                petObjs.Add(new PetObj
                {
                    uuid = pet.uuid,
                    obj = newPetObj
                });

                // 这里可以添加初始化宠物属性的代码
                Debug.Log($"Created new pet: {pet.name} with UUID: {pet.uuid}");
                
                UpdateRanking();
            }
        }

        private void CheckForNewPets()
        {
            if (petList == null || petList.pets == null) return;

            foreach (var pet in petList.pets)
            {
                // 检查LivingPet和petObjs中是否已存在
                bool alreadyExists = LivingPet.Exists(p => p.uuid == pet.uuid) ||
                                   petObjs.Exists(p => p.uuid == pet.uuid);

                if (!alreadyExists)
                {
                    Debug.Log($"New pet found: {pet.name} (UUID: {pet.uuid})");
                    ConstructPet(pet);
                    LivingPet.Add(pet);
                }
            }
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
                Pet.Pet pet = petList.pets.Find(p => p.uuid == feed.uuid);
                if (pet != null)
                {
                    FeedPet(pet, feed);
                }
                else
                {
                    Debug.LogWarning($"未找到 UUID={feed.uuid} 的宠物！");
                }
            }
            
            UpdateRanking();

        }

        void FeedPet(Pet.Pet pet, FeedItem feed)
        {
            Debug.Log($"喂食宠物：{pet.name}，评分：{feed.score}，评论：{feed.comments}");
            // 👉 在这里实现你的喂食逻辑，如更新状态、调用动画、加经验值等等

            var feedPetObj = petObjs.Find(p => p.uuid == feed.uuid);
            
            feedPetObj.obj.GetComponent<PetController>().Feed(feed.score, feed.comments);

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