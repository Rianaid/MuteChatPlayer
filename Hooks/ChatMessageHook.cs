using HarmonyLib;
using MuteChatPlayer.Utils;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;

namespace MuteChatPlayer.Hooks
{
    [HarmonyAfter("gg.deca.VampireCommandFramework")]
    [HarmonyBefore("gg.deca.Bloodstone")]
    [HarmonyPatch(typeof(ChatMessageSystem), nameof(ChatMessageSystem.OnUpdate))]
    public class ChatMessageSystem_Patch
    {
        [HarmonyPrefix]
        public static void prefix(ChatMessageSystem __instance)
        {

            var entities = __instance._ChatMessageQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var chatMessage = Helper.Server.EntityManager.GetComponentData<ChatMessageEvent>(entity);
                var fromCharacter = Helper.Server.EntityManager.GetComponentData<FromCharacter>(entity);
                User user = Helper.EntityManager.GetComponentData<User>(fromCharacter.User);
                if (MutePlayersSystem.MutePlayerSystem_Enabled && !user.IsAdmin)
                {
                    bool check = MutePlayersSystem.OnChatMessage(chatMessage, fromCharacter);
                    if (check)
                    {
                        Helper.EntityManager.DestroyEntity(entity);
                    }
                }

            }
        }
    }
}
