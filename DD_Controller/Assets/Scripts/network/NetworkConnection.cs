using System.Collections.Generic;
using System.Threading.Tasks;
using LobbyCustom;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace network
{
    public abstract class NetworkConnection
    {
        static NetworkEndpoint GetEndpointForAllocation(
            List<RelayServerEndpoint> endpoints,
            string ip,
            int port,
            out bool isSecure)
        {
#if ENABLE_MANAGED_UNITYTLS
            foreach (RelayServerEndpoint endpoint in endpoints)
            {
                if (endpoint.Secure && endpoint.Network == RelayServerEndpoint.NetworkOptions.Udp)
                {
                    isSecure = true;
                    return NetworkEndpoint.Parse(endpoint.Host, (ushort)endpoint.Port);
                }
            }
#endif
            isSecure = false;
            return NetworkEndpoint.Parse(ip, (ushort)port);
        }
        
        static string AddressFromEndpoint(NetworkEndpoint endpoint)
        {
            return endpoint.Address.Split(':')[0];
        }
        
        public static async Task SetRelayHostConnection()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();
            var allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.Instance.GetMaxPlayers ?? 2);
            var joincode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            await LobbyManager.Instance.SetRelayCode(joincode);
            bool isSecure = false;
            var endpoint = GetEndpointForAllocation(allocation.ServerEndpoints,
                allocation.RelayServer.IpV4, allocation.RelayServer.Port, out isSecure);

            transport.SetHostRelayData(AddressFromEndpoint(endpoint), endpoint.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, isSecure);

            GameMultiplayer.Instance.StartHost();
        }

        public static async Task SetRelayClientConnection()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();
            Debug.Log("Trying to Join Session");
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(LobbyManager.Instance.GetRelayCode.Value);
            bool isSecure = false;
            var endpoint = GetEndpointForAllocation(joinAllocation.ServerEndpoints,
                joinAllocation.RelayServer.IpV4, joinAllocation.RelayServer.Port, out isSecure);

            transport.SetClientRelayData(AddressFromEndpoint(endpoint), endpoint.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key,
                joinAllocation.ConnectionData, joinAllocation.HostConnectionData, isSecure);
            Debug.Log("Connection Data Set");
            GameMultiplayer.Instance.StartClient();

        }
    }
    
}