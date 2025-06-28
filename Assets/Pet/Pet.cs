using System.Collections.Generic;
using UnityEngine;

namespace Pet
{
    public class Pet
    {
        public string name;
        public int value;
        public string comments;
        public string avatarPath;
        public List<string> sayings;
        public string uuid;
    }
    
    public class PetList
    {
        public List<Pet> pets = new List<Pet>();
    }

    public class PetUtil
    {
        public static string SerializePetList(PetList petList)
        {
            return JsonUtility.ToJson(petList, true); // true 表示格式化输出
        }
    
        public static PetList DeserializePetList(string json)
        {
            return JsonUtility.FromJson<PetList>(json);
        }
    }
    

}