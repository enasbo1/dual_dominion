using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace menu.script
{
    public struct PlayerInfo
    {
        public enum Role
        {
            God,
            Survivor
        }
        
        public string username;
        public Role role;
        public ulong clientID;
    }
    public class SharedDataManagerScript : NetworkBehaviour
    {
        public List<PlayerInfo> playerInfos = new ();
    }
}
