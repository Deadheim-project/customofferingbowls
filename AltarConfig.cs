using System.Collections.Generic;
using System;
using UnityEngine;
using Jotunn.Managers;

namespace CustomOfferingBowls
{
    public class AltarConfig
    {
        public string name;
        public string m_itemPrefab;
        public string m_bossPrefab;
        public int m_bossItems;
        public string m_bossItem;
        public string m_useItemText;
        public string prefabToCopy;
    }

    public class Root
    {       
        public List<AltarConfig> GetSpawnAreaConfigs()
        {
            List<AltarConfig> SpawnAreaConfigList = new List<AltarConfig>();
            foreach (string str in CustomOfferingBowls.AltarConfigList.Value.Trim(' ').Split('|'))
            {
                var areaInfo = str.Split(';');
                var cfg = new AltarConfig();
                cfg.name = areaInfo[0].Split('=')[1];
                cfg.m_itemPrefab = areaInfo[1].Split('=')[1];
                cfg.m_bossPrefab = areaInfo[2].Split('=')[1];
                cfg.m_bossItems = System.Convert.ToInt32(areaInfo[3].Split('=')[1]);
                cfg.m_bossItem = areaInfo[4].Split('=')[1];
                cfg.m_useItemText = areaInfo[5].Split('=')[1];
                cfg.prefabToCopy = areaInfo[6].Split('=')[1];

                SpawnAreaConfigList.Add(cfg);
            }

            return SpawnAreaConfigList;
        }
    }
}
