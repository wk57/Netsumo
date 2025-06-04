using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyUIHandler : NetworkBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject hostStartGameButton;
    [SerializeField] private TextMeshProUGUI notificationText;

    private Coroutine actualNotificationCoroutine = null;

    public void SetStartButtonState(bool state)
    {
        hostStartGameButton.SetActive(state);
    }

    public void PlayerConnectedRPC(SessionUserData data)
    {
        if (actualNotificationCoroutine != null)
        {
            StopCoroutine(actualNotificationCoroutine);
            actualNotificationCoroutine = null;
            notificationText.enabled = false;
        }

        string notifText = $"{data.userName} has joined the game!";
        SendNotificationToPlayersRPC(notifText);

    }

    
    public void PlayerLeftRPC(SessionUserData data)
    {
        if(actualNotificationCoroutine != null)
        {
            StopCoroutine(actualNotificationCoroutine);
            actualNotificationCoroutine = null;
            notificationText.enabled = false;
        }

        string notifText = $"{data.userName} has left the game!";
        SendNotificationToPlayersRPC(notifText);
    }

    [Rpc(SendTo.Everyone)]
    public void SendNotificationToPlayersRPC(string sendMessage, float timeNotification = 3f)
    {
        actualNotificationCoroutine =  StartCoroutine(ShowNotificationCoroutine(sendMessage, timeNotification));
    }

    
    public IEnumerator ShowNotificationCoroutine(string text, float timeNotification)
    {
        notificationText.text = text;
        notificationText.enabled = true;

        yield return new WaitForSeconds(timeNotification);

        notificationText.enabled = false;

        actualNotificationCoroutine = null;
    }


}
