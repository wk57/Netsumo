using UnityEngine;
using Unity.Netcode;
using Netcode.Transports;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using HeathenEngineering.SteamworksIntegration;

namespace LazyFace
{
    public class NetcodeHandlerController : MonoBehaviour
    {
        [SerializeField] private string NextSceneName;

        [SerializeField] private UnityEvent NetcodeTransportFailed;
        [SerializeField] private UnityEvent OnNetcodeSceneChange;

        private void OnEnable()
        {
            NetworkManager.Singleton.OnServerStarted += NetcodeServerStarted;
            NetworkManager.Singleton.OnClientStarted += NetcodeClientStarted;
            NetworkManager.Singleton.OnTransportFailure += NetcodeTransportFail;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnServerStarted -= NetcodeServerStarted;
            NetworkManager.Singleton.OnClientStarted -= NetcodeClientStarted;
            NetworkManager.Singleton.OnTransportFailure -= NetcodeTransportFail;
        }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnServerStarted -= NetcodeServerStarted;
            NetworkManager.Singleton.OnClientStarted -= NetcodeClientStarted;
            NetworkManager.Singleton.OnTransportFailure -= NetcodeTransportFail;
        }

        public void StartNetcodeHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNetcodeClient(LobbyData lobbyToJoinData)
        {
            Debug.Log("StartNetcodeClient");
            ulong hostId = (ulong)lobbyToJoinData.Owner.user.id;
            NetworkManager.Singleton.gameObject.GetComponent<SteamNetworkingSocketsTransport>().ConnectToSteamID = hostId;
            NetworkManager.Singleton.StartClient();
        }


        private void NetcodeServerStarted()
        {
            OnNetcodeSceneChange?.Invoke();
            NetworkManager.Singleton.SceneManager.LoadScene(NextSceneName, LoadSceneMode.Single);
        }

        private void NetcodeClientStarted()
        {
            Debug.Log("NetcodeClientStarted");
            OnNetcodeSceneChange?.Invoke();
            NetworkManager.Singleton.SceneManager.LoadScene(NextSceneName, LoadSceneMode.Single);
        }

        private void NetcodeTransportFail()
        {
            NetcodeTransportFailed?.Invoke();
        }
    }
}
