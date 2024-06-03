using MuteChatPlayer.Database;
using MuteChatPlayer.Utils;
using ProjectM.Network;
using System;
using VampireCommandFramework;

namespace MuteChatPlayer.Commands
{
    public static class MuteCommands
    {
        [Command("mute", usage: "[Player Name] [global|local|all] [time in minuts]", description: "Command for mute player.", adminOnly: true)]
        public static void MuteCommand(ChatCommandContext ctx, string name = "null", string channal = "all", int bantime = 60)
        {
            var banChat = -1;
            if (name != "null")
            {
                if (Helper.FindPlayer(name, out var user))
                {
                    switch (channal.ToLower())
                    {
                        case "global":
                            banChat = (int)ChatMessageType.Global;
                            break;
                        case "local":
                            banChat = (int)ChatMessageType.Local;
                            break;
                        case "all":
                            banChat = -1;
                            break;
                        default:
                            ctx.Reply($"<color=#ff0000>Invalid chat-type option. Options are: global, local,all</color>");
                            banChat = -55;
                            break;
                    }
                    if (banChat == -55) { return; }

                    if (DB.PlayerChatMute.ContainsKey(user.PlatformId))
                    {
                        DB.PlayerChatMute[user.PlatformId] = new PlayerMuteChat(DateTime.Now.AddMinutes(bantime), banChat);
                        ctx.Reply($"Mute player [{user.CharacterName}] is updated. Mute end: [{DB.PlayerChatMute[user.PlatformId].BanTime}]");
                    }
                    else
                    {
                        DB.PlayerChatMute.TryAdd(user.PlatformId, new PlayerMuteChat(DateTime.Now.AddMinutes(bantime), banChat));
                        ctx.Reply($"Player [{user.CharacterName}] is muted. Mute end: [{DB.PlayerChatMute[user.PlatformId].BanTime}]");
                    }
                    return;
                }
                else
                {
                    ctx.Reply($"Player with name : [{name}] not found!");
                    return;
                }
            }
            else
            {
                ctx.Reply($"You need use player name for mute.");
                return;
            }
        }
    }
}
