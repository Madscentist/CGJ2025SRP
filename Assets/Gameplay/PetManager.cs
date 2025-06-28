using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pet;
using UnityEngine;

namespace Gameplay
{
    public class PetManager : MonoBehaviour
    {
        //TODO 服务器轮询
        //TODO 如果有新的宠物json，

        public PetList PetList;
        public string customFilePath;
        public List<Pet.Pet> LivingPet = new List<Pet.Pet>(); 
        
        

        
        void Start()
        {
            StartCoroutine(LoadPetListPeriodically());
        }

        IEnumerator LoadPetListPeriodically()
        {
            while (true)
            {
                LoadPetListFromCustomPath();
                yield return new WaitForSeconds(2f);
            }
        }
        
        void LoadPetListFromCustomPath()
        {
            if (File.Exists(customFilePath))
            {
                string json = File.ReadAllText(customFilePath);
                PetList = JsonUtility.FromJson<PetList>(json);
            }
            else
            {
                Debug.LogWarning("File not found at: " + customFilePath);
            }
        }

        public void ConstructPet(Pet.Pet pet)
        {
            
        }


        private void CheckForNewPets()
        {
            foreach (var pet in PetList.pets)
            {
                // 如果 living_pet 中没有这个 uuid，说明是新宠物
                bool alreadyExists = LivingPet.Exists(p => p.uuid == pet.uuid);

                if (!alreadyExists)
                {
                    Debug.Log($"New pet found: {pet.name} (UUID: {pet.uuid})");
                    ConstructPet(pet);
                    LivingPet.Add(pet);
                }
            }
        }
        
    }
}