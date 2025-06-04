using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

namespace LazyFace
{
    public class SteamworksPreservationHandler : MonoBehaviour
    {
        public static SteamworksPreservationHandler Instance { get; private set; }

        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private OverlayManager overlayManager;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        public void OnSceneChange()
        {
            //Remove lobby events
            lobbyManager.evtCreated.RemoveAllListeners();
            lobbyManager.evtCreateFailed.RemoveAllListeners();
            lobbyManager.evtEnterSuccess.RemoveAllListeners();
            lobbyManager.evtEnterFailed.RemoveAllListeners();
            lobbyManager.evtLobbyInvite.RemoveAllListeners();

            //Remove overlay events
            overlayManager.evtGameLobbyJoinRequested.RemoveAllListeners();
        }
    }
}
