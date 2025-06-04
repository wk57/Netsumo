using LazyFace;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    PlayersManager playersManager;
    void Awake()
    {
        playersManager = FindAnyObjectByType<PlayersManager>();

        if(playersManager == null)
        {
            Debug.LogError("Couldn't find players manager in the scene");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerNetworkHandler playerEnteringDeadZone = other.GetComponent<PlayerNetworkHandler>();
            playersManager.AddPlayerToDeadList(playerEnteringDeadZone);
        }
    }
}
