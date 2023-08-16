using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string characterName = "Characters name";
    [SerializeField] private NetworkObject gameplayPrefab;

    public int Id => id;
    public string CharacterName => characterName;

    public NetworkObject GameplayPrefab => gameplayPrefab;
}