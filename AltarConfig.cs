using System.Collections.Generic;
using System;
using UnityEngine;
using Jotunn.Managers;
using System.IO;

namespace CustomOfferingBowls
{


    public class JsonLoader
    {
        public List<Altar> GetSpawnAreaConfigs()
        {
            Root items = SimpleJson.SimpleJson.DeserializeObject<Root>(File.ReadAllText(CustomOfferingBowls.FileDirectory));
            return items.Altars;
        }

        public List<Altar> GetSpawnAreaConfigs(string json)
        {
            Root items = SimpleJson.SimpleJson.DeserializeObject<Root>(json);

            return items.Altars;

        }
    }

    public class Altar
    {
        public string name { get; set; }
        public string m_itemPrefab { get; set; }
        public string m_bossPrefab { get; set; }
        public int m_bossItems { get; set; }
        public string m_bossItem { get; set; }
        public string m_useItemText { get; set; }
        public string prefabToCopy { get; set; }
    }

    public class Root
    {
        public List<Altar> Altars { get; set; }
    }


}
