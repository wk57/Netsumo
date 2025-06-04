using HeathenEngineering.SteamworksIntegration;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// [MVC] Model data for NetcodeSessionHandler controller
/// </summary>
[CreateAssetMenu(fileName = "Netcode session handler Model", menuName = "MVC/Netcode session model")]
public class NetcodeSessionData : ScriptableObject
{
    private Dictionary<ulong, ulong> userIdMapping = new Dictionary<ulong, ulong>(); //Only handled by Server, maps NetcodeID -> SteamID

    public void AddNewUserMapping(ulong netcodeId, ulong steamId)
    {
        if (!userIdMapping.ContainsKey(netcodeId))
        {
            userIdMapping.Add(netcodeId, steamId);
            Debug.Log("Actual mapping: ");
            foreach(var userid in userIdMapping.Values)
            {
                Debug.Log(userid);
            }
        }
        else
        {
            Debug.LogWarning("User already exists in mapping.");
        }
    }

    public SessionUserData GetUserByNetcodeId(ulong netcodeId)
    {   

        if (userIdMapping.ContainsKey(netcodeId))
        {
            UserData userRetrieved = UserData.Get(userIdMapping[netcodeId]);
            SessionUserData user = new SessionUserData((ulong)userRetrieved.id, netcodeId, userRetrieved.Name);

            return user;
        }
        else
        {
            Debug.LogWarning($"User NetcodeID {netcodeId} not found.");
            return new SessionUserData(0, 0, "NULL");
        }
    }

    public SessionUserData GetUserByCSteamID(ulong CSteamID)
    {

        ulong netcodeId = 0;
        bool userFound = false;

        foreach (var userMapped in userIdMapping)
        {
            if (userMapped.Value == CSteamID)
            {
                netcodeId = userMapped.Key;
                userFound = true;
                break;
            }
        }

        if (userFound)
        {
            UserData userRetrieved = UserData.Get(netcodeId);
            SessionUserData user = new SessionUserData((ulong)userRetrieved.id, netcodeId, userRetrieved.Name);
            return user;
        }
        else
        {
            Debug.LogWarning($"User CSteamID {CSteamID} not found.");
            return new SessionUserData(0, 0, "NULL");
        }
    }
}

public struct SessionUserData: INetworkSerializable
{
    public ulong steamId;
    public ulong netcodeId;
    public string userName;

    public SessionUserData(ulong steamId, ulong netcodeId, string userName)
    {
        this.steamId = steamId;
        this.netcodeId = netcodeId;
        this.userName = userName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref steamId);
        serializer.SerializeValue(ref netcodeId);
        serializer.SerializeValue(ref userName);
    }
}
