using HarmonyLib;
using MuteChatPlayer.Utils;
using ProjectM;
using Unity.Collections;

namespace MuteChatPlayer.Hooks
{
    [HarmonyPatch(typeof(LoadPersistenceSystemV2), nameof(LoadPersistenceSystemV2.SetLoadState))]
    public class PersistenceSystem_Patch
    {
        public static void Prefix(ServerStartupState.State loadState, LoadPersistenceSystemV2 __instance)
        {
            if (loadState == ServerStartupState.State.SuccessfulStartup)
            {
                Plugin.GamedataInit();
            }
        }
    }
    [HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.TriggerSave))]
    public class TriggerPersistenceSaveSystem_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SaveReason reason, FixedString128Bytes saveName, ServerRuntimeSettings saveConfig)
        {
            MutePlayersSystem.Save_Mute_List();
        }
    }
}
