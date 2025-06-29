using System.Collections.Generic;
using UnityEngine;

namespace Pet
{
    [System.Serializable]
    public class FeedItem
    {
        public int score;
        public string comments;
        public string uuid;
    }

    [System.Serializable]
    public class FeedList
    {
        public List<FeedItem> feeds = new List<FeedItem>();
    }
    
    public static class FeedUtil
    {
        public static FeedList DeserializeFeedList(string json)
        {
            if (json.TrimStart().StartsWith("["))
            {
                json = "{ \"feeds\": " + json + " }";
            }
            return JsonUtility.FromJson<FeedList>(json);
        }
    }
}