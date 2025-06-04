using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;

namespace LazyFace
{
    public class SteamLobbyHandlerController : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private OverlayManager overlayManager;

        private LobbyData lobbyToJoinData;

        public void AcceptInvitation()
        {
            lobbyManager.Join(lobbyToJoinData);
        }

        public void SteamLobbyInviteAccepted(LobbyInvite lobbyInvite)
        {
            lobbyToJoinData = lobbyInvite.ToLobby;
        }

        public void SteamLobbyJoinFriendList(LobbyData lobbyData, UserData userData)
        {
            lobbyToJoinData = lobbyData;
        }

        public void JoiningLobbyError(EChatRoomEnterResponse lobbyResponse)
        {
            Debug.Log(lobbyResponse);
        }
    }
}
