using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime.Injection;
using MuteChatPlayer.Database;
using ProjectM;
using ProjectM.Network;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using UnityEngine;

namespace MuteChatPlayer.Utils
{
    public class MutePlayersSystem : MonoBehaviour
    {
        private static MutePlayersSystem _instance;
        public static DateTime LastSaveMuteList = DateTime.Now;
        private static readonly string FileDirectory = Path.Combine("BepInEx", "config", MyPluginInfo.PLUGIN_NAME);
        private static readonly string FileName = "Mute_List.json";
        private static readonly string fullPath = Path.Combine(FileDirectory, FileName);
        private static int loop = 0;
        public static bool MutePlayerSystem_Enabled = true;
        public static bool OnChatMessage(ChatMessageEvent chatMessage, FromCharacter fromCharacter)
        {
            User user = Helper.EntityManager.GetComponentData<User>(fromCharacter.User);
            if (DB.PlayerChatMute.ContainsKey(user.PlatformId))
            {
                var now = DateTime.Now;
                var data = DB.PlayerChatMute[user.PlatformId];
                if (now > data.BanTime)
                {
                    DB.PlayerChatMute.TryRemove(user.PlatformId, out _);
                    return false;
                }
                else
                {
                    if (chatMessage.MessageType == (ChatMessageType)data.MuteChatChannel || data.MuteChatChannel == -1)
                    {
                        ServerChatUtils.SendSystemMessageToClient(Helper.EntityManager, user, $"You have been muted for {Math.Ceiling(DB.PlayerChatMute[user.PlatformId].BanTime.Subtract(now).TotalMinutes)} more minutes.");
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        void Update()
        {
        }

        void Start()
        {
            Load_Mute_List();
            StartCoroutine(UpdateMuteList().WrapToIl2Cpp());
        }
        private IEnumerator UpdateMuteList()
        {
            while (true)
            {
                foreach (var player in DB.PlayerChatMute.ToArray())
                {
                    if (DB.PlayerChatMute[player.Key].BanTime < DateTime.Now)
                    {
                        DB.PlayerChatMute.TryRemove(player.Key, out _);
                    }
                }
                var timespan = DateTime.Now - LastSaveMuteList;
                if (timespan.TotalSeconds >= 300)
                {
                    LastSaveMuteList = DateTime.Now;
                }
                yield return new WaitForSeconds(60);
            }
        }
        void LateUpdate()
        {
        }

        public static void Initialize()
        {
            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<MutePlayersSystem>())
            {
                ClassInjector.RegisterTypeInIl2Cpp<MutePlayersSystem>();
            }
            _instance = Plugin.Instance.AddComponent<MutePlayersSystem>();

        }
        public static void Save_Mute_List()
        {
            File.WriteAllText(fullPath, JsonSerializer.Serialize(DB.PlayerChatMute, new JsonSerializerOptions() { WriteIndented = true }));
            Plugin.Logger.LogWarning("Mute_List DB Saved.");
        }

        public static void Load_Mute_List()
        {
            if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);
            if (!File.Exists(fullPath))
            {
                DB.PlayerChatMute.Clear();
                Plugin.Logger.LogWarning("Mute_List DB Created.");
                Save_Mute_List();
            }
            else
            {
                string json = File.ReadAllText(fullPath);
                DB.PlayerChatMute = JsonSerializer.Deserialize<ConcurrentDictionary<ulong, PlayerMuteChat>>(json);
                Plugin.Logger.LogWarning("Mute_List DB Populated");

            }
        }

        public static void Uninitialize()
        {
            Destroy(_instance);
            _instance = null;
        }
    }
}
