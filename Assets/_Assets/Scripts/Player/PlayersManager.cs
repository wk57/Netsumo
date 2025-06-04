using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace LazyFace
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] private NetcodeSessionData netcodeSessionModel;

        [SerializeField] private NetcodeSessionHandler netcodeSessionHandler;

        private List<PlayerNetworkHandler> alivePlayersNetworkHandlertList = new List<PlayerNetworkHandler>();
        private List<PlayerNetworkHandler> deadPlayersNetworkHandlerList = new List<PlayerNetworkHandler>();

        [SerializeField] private UnityEvent<SessionUserData> OnPlayerDeath;


        public void AddPlayerToDeadList(PlayerNetworkHandler player)
        {
            alivePlayersNetworkHandlertList.Remove(player);

            deadPlayersNetworkHandlerList.Add(player);

            player.SetPlayerAliveRpc(false);
            player.SetPlayerInputRpc(false);

            OnPlayerDeath?.Invoke(GetPlayerSteamSessionUserData(player.gameObject));
        }

        public SessionUserData GetPlayerSteamSessionUserData(GameObject player)
        {
            ulong playerNetcodeId = player.gameObject.GetComponent<NetworkObject>().OwnerClientId;

            SessionUserData playerSteamData = netcodeSessionModel.GetUserByNetcodeId(playerNetcodeId);

            return playerSteamData;
        }

        private void ClearPlayersLists()
        {
            alivePlayersNetworkHandlertList.Clear();
            deadPlayersNetworkHandlerList.Clear();
        }

        #region public methods

        /// <summary>
        /// Search for all alive players in the scene (Deletes previous listed alive and dead players)
        /// </summary>
        public void SearchPlayers()
        {
            ClearPlayersLists();

            if (alivePlayersNetworkHandlertList != null)
            {
                alivePlayersNetworkHandlertList = FindObjectsByType<PlayerNetworkHandler>(FindObjectsSortMode.None).ToList();
            }
        }

        public void PlayerDisconnected(SessionUserData disconectedUserData)
        {
            PlayerNetworkHandler playerToDisconnect = null;

            foreach(PlayerNetworkHandler alivePlayer in alivePlayersNetworkHandlertList)
            {
                if (GetPlayerSteamSessionUserData(alivePlayer.gameObject).steamId == disconectedUserData.steamId)
                {
                    playerToDisconnect = alivePlayer;
                    break;
                }
            }

            if (playerToDisconnect != null)
            {
                alivePlayersNetworkHandlertList.Remove(playerToDisconnect);

                deadPlayersNetworkHandlerList.Add(playerToDisconnect);

                OnPlayerDeath?.Invoke(disconectedUserData);
            }
        }

        public List<PlayerNetworkHandler> GetPlayersAliveNetworkHandlers()
        {
            return alivePlayersNetworkHandlertList;
        }

        public List<PlayerNetworkHandler> GetDeathPlayersNetworkHandlers()
        {
            return deadPlayersNetworkHandlerList;
        }

        #endregion
    }
}
