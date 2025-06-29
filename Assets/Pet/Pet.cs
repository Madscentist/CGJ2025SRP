using System.Collections.Generic;
using UnityEngine;

namespace Pet
{
    [System.Serializable]
    public class Pet
    {
        public string name;
        public int value;
        public string comments;
        public string avatarPath;
        public string uuid;
        public string qrPath;
    }

    [System.Serializable]
    public class PetList
    {
        public List<Pet> pets = new List<Pet>();
    }

    public static class PetUtil
    {
        public static string SerializePetList(PetList petList)
        {
            return JsonUtility.ToJson(petList, true);
        }

        public static PetList DeserializePetList(string json)
        {
            
            if (json.TrimStart().StartsWith("["))
            {
                json = "{ \"pets\": " + json + " }";
            }
            
            return JsonUtility.FromJson<PetList>(json);
        }
    }
}