using Unity.Netcode;
using UnityEngine;

namespace menu.script
{
    public enum Role
    {
        God,
        Survivor,
    }
    public struct PlayerInfo
    {
        public string username;
        public Role role;
    }
    public class GlobalMenuScript : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
