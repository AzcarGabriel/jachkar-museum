using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System;
using System.IO;
using UnityEngine.Networking;

[RequireComponent(typeof(StoneService))]
public class NetworkStoneSpawner : NetworkBehaviour
{
    StoneService stoneService;
    private void Awake()
    {
        stoneService = GetComponent<StoneService>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnStoneServerRpc(int stoneId, Vector3 sp, Quaternion rt)
    {
        SpawnStoneClientRpc(stoneId, sp, rt);
    }

    [ClientRpc]
    public void SpawnStoneClientRpc(int stoneId, Vector3 sp, Quaternion rt)
    {
        Debug.Log("Trying to spawn: " + stoneId.ToString());
        StartCoroutine(stoneService.SpawnStoneWithPositionAndRotation(stoneId, sp, rt));
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveStoneServerRpc(int stoneId, Vector3 sp, ServerRpcParams serverRpcParams = default)
    {
        MoveStoneClientRpc(stoneId, sp, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void MoveStoneClientRpc(int stoneId, Vector3 sp, ulong originalSender, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(originalSender);

        if (NetworkManager.Singleton.LocalClientId == originalSender)
        {
            Debug.Log("Yo estoy moviendo");
            return;
        }
        Debug.Log("El otro está moviendo");
        ServerManager.Instance.MoveStoneById(stoneId, sp);
    }
}
