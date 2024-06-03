using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MuteChatPlayer.Utils;
using System.Reflection;
using VampireCommandFramework;

namespace MuteChatPlayer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    public class Plugin : BasePlugin
    {
        Harmony _harmony;
        public static ManualLogSource Logger;
        internal static Plugin Instance;
        public override void Load()
        {
            Instance = this;
            Logger = Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            CommandRegistry.RegisterAll(Assembly.GetExecutingAssembly());
        }
        public static void GamedataInit()
        {
            MutePlayersSystem.Initialize();
        }
        public override bool Unload()
        {
            CommandRegistry.UnregisterAssembly();
            _harmony?.UnpatchSelf();
            return true;
        }
    }
}
