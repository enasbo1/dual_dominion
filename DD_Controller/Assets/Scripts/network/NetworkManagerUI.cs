using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = NetworkManager.Singleton;

        serverButton.onClick.AddListener(() => { _networkManager.StartServer(); });

        hostButton.onClick.AddListener(() => { _networkManager.StartHost(); });

        clientButton.onClick.AddListener(() => { _networkManager.StartClient(); });
    }
}