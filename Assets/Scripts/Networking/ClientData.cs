using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[Serializable]
public class ClientData
{
    public ulong clientId;
    public int characterId = -1;
    public string username;

    public ClientData(ulong clientId) {
        this.clientId = clientId;
    }
}
