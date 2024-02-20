using Unity.Netcode;
using UnityEngine;

namespace Networking.Character
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private int id = -1;
        [SerializeField] private string characterName = "Characters name";
        [SerializeField] private NetworkObject gameplayPrefab;
        [SerializeField] private Sprite icon;

        public int Id => id;
        public string CharacterName => characterName;

        public Sprite Icon => icon;

        public NetworkObject GameplayPrefab => gameplayPrefab;
    }
}
