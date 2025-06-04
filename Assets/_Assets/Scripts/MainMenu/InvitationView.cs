using TMPro;
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;

namespace LazyFace
{
    public class InvitationView : MonoBehaviour
    {
        [SerializeField] private GameObject invitationPopup;
        [SerializeField] private TextMeshProUGUI nameInvite;

        public void InvitationReceived(LobbyInvite lobbyData)
        {
            invitationPopup.SetActive(true);
            nameInvite.text = $"{lobbyData.FromUser.Name} te ha invitado";
        }
    }
}
