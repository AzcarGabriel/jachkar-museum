using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private Character[] characters = Array.Empty<Character>();

    public Character[] GetAllCharacters() => characters;

    public Character GetCharacterById(int id)
    {
        return characters.FirstOrDefault(character => character.Id == id);
    }
}
