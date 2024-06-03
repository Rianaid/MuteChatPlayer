using System;
using System.Collections.Concurrent;

namespace MuteChatPlayer.Database
{
    public struct PlayerMuteChat
    {
        public DateTime BanTime { get; set; }
        public int MuteChatChannel { get; set; }
        public PlayerMuteChat(DateTime bantime, int mutechatchannel)
        {
            BanTime = bantime;
            MuteChatChannel = mutechatchannel;
        }
    }
    internal class DB
    {
        public static ConcurrentDictionary<ulong, PlayerMuteChat> PlayerChatMute = new ConcurrentDictionary<ulong, PlayerMuteChat>();
    }
}
