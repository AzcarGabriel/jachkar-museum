using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.Lobby
{
    public class PlayerCard : MonoBehaviour
    {
        [SerializeField] private CharacterDatabase characterDatabase;
        [SerializeField] private GameObject visuals;
        [SerializeField] private Image characterIconImage;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text characterNameText;

        public void UpdateDisplay(CharacterSelectState state)
        {
            if (state.CharacterId != -1)
            {
                Character character = characterDatabase.GetCharacterById(state.CharacterId);
                characterIconImage.sprite = character.Icon;
                characterIconImage.enabled = true;
                characterNameText.text = character.CharacterName;
                playerNameText.text = $"Player {state.ClientId}";
            }
            else
            {
                characterIconImage.enabled = false;
                visuals.SetActive(true);
            }
        }

        public void DisableDisplay()
        {
            visuals.SetActive(false);
        }
    }
}
