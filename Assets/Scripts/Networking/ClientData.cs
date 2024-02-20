using System;
using Unity.Collections;

namespace Networking
{
    [Serializable]
    public class ClientData
    {
        public ulong clientId;
        public int characterId = -1;
        public FixedString32Bytes username;
        public bool isLeader;

        public ClientData(ulong clientId) {
            this.clientId = clientId;
        }
    }
}
