// That script is responsible for spawning the character prefabs for each connected client.
// It is called when the server is started and it will spawn the character prefab for each connected client.
// The character prefab is determined by the characterId that is stored in the client data.
#if USE_MULTIPLAYER
using Unity.Netcode;
using UnityEngine;

namespace Networking.Character
{
    public class CharacterSpawner : NetworkBehaviour
    {

        [SerializeField] private CharacterDatabase characterDatabase;
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            foreach (var client in ServerManager.Instance.ClientData)
            {
                var character = characterDatabase.GetCharacterById(client.Value.characterId);
                if (character != null)
                {
                    var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                    var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                    characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                }
            }
        }
    }
}
#endif
