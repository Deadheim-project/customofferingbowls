using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Linq;
using System.IO;
using UnityEngine;
using Jotunn.Managers;

namespace CustomOfferingBowls
{
    [BepInPlugin("Detalhes.CustomOfferingBowls", "CustomOfferingBowls", "1.0.0")]
    public class CustomOfferingBowls : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.CustomOfferingBowls";
        Harmony harmony = new Harmony(PluginGUID);
        static Root root = new Root();
        public static bool hasSpawned = false;
        public static readonly string ModPath = Path.GetDirectoryName(typeof(CustomOfferingBowls).Assembly.Location);

        public static ConfigEntry<string> AltarConfigList;

        private void Awake()
        {
            AltarConfigList = Config.Bind("Server config", "AltarConfigList", "name=Boss Altar 1;m_itemPrefab=Ruby;m_bossPrefab=Deer;m_bossItems=10;m_bossItem=AmberPearl;m_useItemText=Summon boss;prefabToCopy=woodwall|name=Boss Altar 2;m_itemPrefab=Ruby;m_bossPrefab=Neck;m_bossItems=10;m_bossItem=AmberPearl;m_useItemText=Summon boss;prefabToCopy=stone_wall_4x2",
                    new ConfigDescription("SpawnAreaConfigList", null,
                    new ConfigurationManagerAttributes { IsAdminOnly = true }));
            harmony.PatchAll();
        }

        public static bool hasAwake = false;
        [HarmonyPatch(typeof(Game), "Logout")]
        public static class Logout
        {
            private static void Postfix()
            {
                hasAwake = false;
            }
        }

        [HarmonyPatch(typeof(Player), "OnSpawned")]
        public static class OnSpawned
        {
            private static void Postfix()
            {
                if (hasAwake == true) return;
                hasAwake = true;
                AddClonedItems();
            }
        }

        static OfferingBowl BowlToCopy = null;

        public static void AddClonedItems()
        {
            var hammer = ObjectDB.instance.m_items.FirstOrDefault(x => x.name == "Hammer");

            if (!hammer)
            {
                Debug.LogError("Custom OfferingBowls - Hammer could not be loaded"); return;
            }

            PieceTable table = hammer.GetComponent<ItemDrop>().m_itemData.m_shared.m_buildPieces;

            var list = root.GetSpawnAreaConfigs();
            foreach (AltarConfig areaConfig in list)
            {
                string newName = "COB_" + string.Join("_", areaConfig.name);

                if (table.m_pieces.Exists(x => x.name == newName))
                {
                    continue;
                }
                GameObject customOfferingBowls = PrefabManager.Instance.CreateClonedPrefab(newName, areaConfig.prefabToCopy);
                if (customOfferingBowls == null)
                {
                    Debug.LogError("original prefab not found for " + areaConfig.name);
                    continue;
                }

                customOfferingBowls.GetComponent<ZNetView>().m_syncInitialScale = true;

                if (BowlToCopy is null)
                {
                    foreach (OfferingBowl ob in Resources.FindObjectsOfTypeAll(typeof(OfferingBowl)))
                    {
                        if (ob.m_name == "$piece_offerbowl_eikthyr") 
                        {
                            BowlToCopy = ob;
                            break;
                        }                        
                    }
                }

                var itemPrefab = PrefabManager.Instance.GetPrefab(areaConfig.m_itemPrefab);
                if (itemPrefab is null)
                {
                    Debug.LogError("m_itemPrefab not found for " + areaConfig.name);
                }

                var bossPrefab = PrefabManager.Instance.GetPrefab(areaConfig.m_bossPrefab);
                if (bossPrefab is null)
                {
                    Debug.LogError("m_bossPrefab not found for " + areaConfig.name);
                }

                var bossItem = PrefabManager.Instance.GetPrefab(areaConfig.m_bossItem);
                if (bossItem is null)
                {
                    Debug.LogError("m_bossItem not found for " + areaConfig.name);
                }

                OfferingBowl bowl = customOfferingBowls.AddComponent<OfferingBowl>();
                bowl.m_name = areaConfig.name;
                bowl.m_itemPrefab = itemPrefab.GetComponent<ItemDrop>();
                bowl.m_bossPrefab = bossPrefab;
                bowl.m_bossItems = areaConfig.m_bossItems;
                bowl.m_bossItem = bossItem.GetComponent<ItemDrop>();
                bowl.m_useItemText = areaConfig.m_useItemText;
                bowl.m_bossSpawnPoint = new Vector3();
                bowl.m_spawnBossDelay = 10;
                bowl.m_spawnBossMaxDistance = 20;
                bowl.m_spawnBossMaxYDistance = 99999;
                bowl.m_spawnBossStartEffects = BowlToCopy.m_spawnBossStartEffects;
                bowl.m_spawnBossDoneffects = BowlToCopy.m_spawnBossDoneffects;    

                Piece piece = customOfferingBowls.GetComponent<Piece>();
                if (piece is null) piece = customOfferingBowls.AddComponent<Piece>();

                piece.m_description = "Offering Bowl";
                piece.name = customOfferingBowls.name;

                Object.Destroy(customOfferingBowls.GetComponent<Destructible>());
                Object.Destroy(customOfferingBowls.GetComponent<WearNTear>());

                PieceManager.Instance.RegisterPieceInPieceTable(customOfferingBowls, "Hammer", "OfferingBowls");

                if (!SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    table.m_pieces.Remove(customOfferingBowls);
                }
            }
        }
    }
}
