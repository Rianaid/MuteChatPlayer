using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MuteChatPlayer.Utils
{
    public class Helper
    {
        private static World? _serverWorld;
        public static EntityManager EntityManager => Server.EntityManager;
        public static World Server
        {
            get
            {
                if (_serverWorld != null) return _serverWorld;
                _serverWorld = GetWorld("Server")
                    ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
                return _serverWorld;
            }
        }
        public static bool IsServer => Application.productName == "VRisingServer";
        private static World GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }
            return null;
        }
        internal static bool FindPlayer(string name, out User user)
        {
            user = new User();
            EntityQuery query = Server.EntityManager.CreateEntityQuery(new EntityQueryDesc()
            { All = new ComponentType[] { ComponentType.ReadOnly<User>() }, Options = EntityQueryOptions.IncludeDisabled });
            var userEntities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in userEntities)
            {
                user = EntityManager.GetComponentData<User>(entity);
                if (user.CharacterName.ToString().ToLower() == name)
                {
                    userEntities.Dispose();
                    return true;
                }
            }
            userEntities.Dispose();
            return false;
        }
    }
}
