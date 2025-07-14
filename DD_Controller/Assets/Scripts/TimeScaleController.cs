using System;
using UnityEngine;
using Unity.Netcode;

public class TimeScaleController : NetworkBehaviour
{
    public static TimeScaleController Instance { get; private set; }

    [NonSerialized] public readonly NetworkVariable<float> syncedTimeScale = new NetworkVariable<float>(
        1f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    [Range(0.01f, 3f)] public float timeFlowNeutral = 1f;
    [Range(0.001f, 3f)] public float timeFlowPause = 0.005f;
    [Range(0.1f, 3f)] public float timeFlowLevelUp = 0.1f;
    
    [NonSerialized] public float previousTimeFlow = 1f;
    private bool _isMultiplayer;

    private void HandleTimeScaleChanged(float oldFlow, float newFlow)
    {
        previousTimeFlow = oldFlow;
        Time.timeScale = newFlow;
    }
    
    public void SetTimeScale(float newScale)
    {
        if (_isMultiplayer) SetTimeScaleServerRpc(newScale);
        else HandleTimeScaleChanged(Time.timeScale, newScale);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetTimeScaleServerRpc(float newScale)
    {
        syncedTimeScale.Value = newScale;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        _isMultiplayer = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient;
        syncedTimeScale.OnValueChanged += HandleTimeScaleChanged;
    }

    private new void OnDestroy()
    {
        syncedTimeScale.OnValueChanged -= HandleTimeScaleChanged;
    }
}