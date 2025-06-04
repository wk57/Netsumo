using HeathenEngineering.SteamworksIntegration;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles ingame lobby while waiting for new players
/// </summary>
public class GameLobby : NetworkBehaviour
{
    private LobbyUIHandler uiHandler;

    [Header("Game lobby callbacks")]
    [SerializeField] private UnityEvent ClientOnHostStartedGame;


    private void Awake()
    {
        uiHandler = FindFirstObjectByType<LobbyUIHandler>();
        if (uiHandler == null)
            Debug.LogWarning($"{nameof(LobbyUIHandler)} wasn't found in object {gameObject.name}");
    }

    /// <summary>
    /// Starts game server side, closes steam lobby and player cannot connect after starting game
    /// </summary>
    public void StartGame()
    {
        Debug.Log("Start game");
        if(IsServer)
        {
            Debug.Log("Is server");
            LobbyManager steamLobbyManager = FindFirstObjectByType<LobbyManager>();
            if(steamLobbyManager != null)
            {
                steamLobbyManager.SetJoinable(false);
                uiHandler.SetStartButtonState(false);
                ClientOnHostStartedGame?.Invoke();
            }
            else
            {
                Debug.LogError("Lobby manager not found");
            }
        }
    }
}
