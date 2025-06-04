using LazyFace;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Model")]
    [SerializeField] private GameManagerData gameManagerData;

    [Header("Handlers")]
    [SerializeField] private LobbyUIHandler uiHandler;
    [SerializeField] private PlayersManager playersManager;

    [Header("Visuals")]
    [SerializeField] private GameObject lobbyScene;


    private int actualLevelIndex = -1;
    private MapSpawnPointModel levelGenerated = null;

    Coroutine graceCoroutine = null;

    public void InitializeGame()
    {
        SelectRandomLevelRPC();
    }

    [Rpc(SendTo.Server)]
    public void SelectRandomLevelRPC()
    {
        int levelsAmount = gameManagerData.gameLevels.Length;
        int levelSelected = -1;
        do
        {
            levelSelected = Random.Range(0, levelsAmount);
        } 
        while (levelSelected == actualLevelIndex);
        actualLevelIndex = levelSelected;

        PreparePlayers(gameManagerData.gameLevels[levelSelected].GetSpawnPointsTransform());

        LoadLevelRPC(levelSelected);
    }

    [Rpc(SendTo.Everyone)]
    public void LoadLevelRPC(int index)
    {
        UnloadLevel();

        levelGenerated = gameManagerData.gameLevels[index];

        levelGenerated.gameObject.SetActive(true);    
    }

    private void PreparePlayers(List<Transform> spawnPoints)
    {
        List<PlayerNetworkHandler> players = playersManager.GetPlayersAliveNetworkHandlers();
        players.AddRange(playersManager.GetDeathPlayersNetworkHandlers());

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetSpawnPositionRpc(spawnPoints[i].position);
            players[i].SetPlayerAliveRpc(true);

            Debug.Log("Called for player "+i);
        }

        StartCoroutine(StartRoundCoroutine(players));
    }


    private void UnloadLevel()
    {
        if (lobbyScene.activeSelf)
            lobbyScene.SetActive(false);

        if (levelGenerated != null)
        {
            levelGenerated.gameObject.SetActive(false);
            levelGenerated = null;
        }
    }

    [Rpc(SendTo.Server)]
    public void OnPlayerDeathRPC(SessionUserData userDeadData)
    {
        Debug.Log("Players alive : "+playersManager.GetPlayersAliveNetworkHandlers().Count);

        uiHandler.SendNotificationToPlayersRPC($"{userDeadData.userName} has been defeated!");

        if (playersManager.GetPlayersAliveNetworkHandlers().Count == 1)
        {
            graceCoroutine = StartCoroutine(WaitGraceTimeCoroutine());
        }
        else if(playersManager.GetPlayersAliveNetworkHandlers().Count < 1)
        {
            if(graceCoroutine != null)
                StopCoroutine(graceCoroutine);

            SelectWinner();
        }
    }

    private void SelectWinner()
    {
        List<PlayerNetworkHandler> playersAlive = playersManager.GetPlayersAliveNetworkHandlers();
        if (playersAlive.Count == 1)
        {
            SessionUserData winnerData = playersManager.GetPlayerSteamSessionUserData(playersAlive[0].gameObject);
            uiHandler.SendNotificationToPlayersRPC($"Player {winnerData.userName} won the round!");
        }
        else
        {
            uiHandler.SendNotificationToPlayersRPC("All players are dead, it's a draw");
        }

        SelectRandomLevelRPC();
    }

    IEnumerator WaitGraceTimeCoroutine()
    {
        yield return new WaitForSeconds(gameManagerData.lastSurvivorWinTime);

        SelectWinner();
    }

    IEnumerator StartRoundCoroutine(List<PlayerNetworkHandler> players)
    {
        yield return new WaitForSeconds(gameManagerData.roundStartWaitTime);

        uiHandler.SendNotificationToPlayersRPC("FIGHT!");

        foreach (PlayerNetworkHandler player in players)
        {
            player.SetPlayerInputRpc(true);
        }
    }

}

[System.Serializable]
public class GameManagerData
{
    public float lastSurvivorWinTime;
    public float roundStartWaitTime;
    public MapSpawnPointModel[] gameLevels;
}

