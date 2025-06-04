using HeathenEngineering.SteamworksIntegration;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the actual Netcode session and also emerging Steam Events
/// [MVC] Uses NetcodeSessionData as the model
/// </summary>
public class NetcodeSessionHandler : NetworkBehaviour
{
    [Header("Model")]
    [SerializeField] private NetcodeSessionData serverOnlySessionData;

    [Header("Session callbacks")]
    [SerializeField] private UnityEvent OnFirstFrameServerCallbacks;
    [SerializeField] private UnityEvent<SessionUserData> ServerOnUserJoined;
    [SerializeField] private UnityEvent<SessionUserData> ServerOnUserLeft;

    private LobbyManager lobbyManager;

    private void Awake() => lobbyManager = FindFirstObjectByType<LobbyManager>();

    private void OnEnable()
    {
        if(lobbyManager != null )
        {
            lobbyManager.evtUserJoined.AddListener(OnUserJoinedLobby);
            lobbyManager.evtUserLeft.AddListener(OnUserLeftLobby);
        }
        else
        {
            Debug.LogError("Couldn't find lobby manager, object isn't getting transitioned from menu");
        }
    }

    private void OnDisable()
    {
        if (lobbyManager != null)
        {
            lobbyManager.evtUserJoined.RemoveListener(OnUserJoinedLobby);
            lobbyManager.evtUserLeft.RemoveListener(OnUserLeftLobby);
        }
        else
        {
            Debug.LogError("Couldn't find lobby manager, object isn't getting transitioned from menu");
        }
    }

    private void Start()
    {
        ReceiveUserIDsRPC(NetworkManager.LocalClientId, (ulong)UserData.Me.id);
        if(IsServer)
        {
            OnFirstFrameServerCallbacks?.Invoke();
        }
    }

    #region Steamworks Events
    public void OnUserJoinedLobby(UserData userJoined)
    {
        if (IsServer)
        {
            SessionUserData userData = new SessionUserData((ulong)userJoined.id, 0, userJoined.Name);
            ServerOnUserJoined?.Invoke(userData);
            Debug.Log("User joined");
        }
    }

    public void OnUserLeftLobby(UserLobbyLeaveData userLeft)
    {
        if (IsServer)
        {
            SessionUserData userData = new SessionUserData((ulong)userLeft.user.id, 0, userLeft.user.Name);
            ServerOnUserLeft?.Invoke(userData);
            Debug.Log("User left");
        }
    }
    #endregion

    [Rpc(SendTo.Server)]
    private void ReceiveUserIDsRPC(ulong netcodeId, ulong steamId)
    {
        serverOnlySessionData.AddNewUserMapping(netcodeId, steamId);
    }

    
}
