using System;
using Unity.Collections;
using Unity.Netcode;

namespace Networking.Lobby
{
    public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
    {
        public ulong ClientId;
        public int CharacterId;
        public FixedString32Bytes UserName;
        public bool IsReady;

        public CharacterSelectState(ulong clientId, int characterId = -1, bool isReady = false, FixedString32Bytes userName = default)
        {
            ClientId = clientId;
            CharacterId = characterId;
            IsReady = isReady;
            UserName = userName;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref CharacterId);
            serializer.SerializeValue(ref UserName);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(CharacterSelectState other)
        {
            return ClientId == other.ClientId 
                   && CharacterId == other.CharacterId 
                   && IsReady == other.IsReady;
        }
    }
}
