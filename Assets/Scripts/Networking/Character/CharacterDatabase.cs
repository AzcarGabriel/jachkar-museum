using System;
using System.Linq;
using Networking.Character;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private CharacterData[] characters = Array.Empty<CharacterData>();

    public CharacterData[] GetAllCharacters() => characters;

    public CharacterData GetCharacterById(int id)
    {
        return characters.FirstOrDefault(character => character.Id == id);
    }

    public bool IsValidCharacterId(int id)
    {
        return characters.Any(x => x.Id == id);
    }
}
