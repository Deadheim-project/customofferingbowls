using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomOfferingBowls
{
	[HarmonyPatch]
	class RPC
	{
		public static void RPC_FileSync(long sender, ZPackage pkg)
		{
			if (!ZNet.instance.IsServer()) return;

			ZPackage inventory = new ZPackage();

			if (File.Exists(CustomOfferingBowls.FileDirectory))
			{
				inventory.Write((File.ReadAllText(CustomOfferingBowls.FileDirectory)));
			}

			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "FileSyncClientCustomOfferingBowls", inventory);

		}

		public static void RPC_FileSyncClient(long sender, ZPackage pkg)
		{
			string json = pkg.ReadString();
			CustomOfferingBowls.AddClonedItems(new JsonLoader().GetSpawnAreaConfigs(json));
		}

		[HarmonyPatch(typeof(Game), "Start")]
		public static class GameStart
		{
			public static void Postfix()
			{
				if (ZRoutedRpc.instance == null)
					return;

				ZRoutedRpc.instance.Register<ZPackage>("FileSyncCustomOfferingBowls", new Action<long, ZPackage>(RPC_FileSync));
				ZRoutedRpc.instance.Register<ZPackage>("FileSyncClientCustomOfferingBowls", new Action<long, ZPackage>(RPC_FileSyncClient));
			}
		}

	}
}
