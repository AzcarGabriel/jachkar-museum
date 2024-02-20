using Networking.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Networking.Lobby
{
    public class CharacterSelectButton : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        private CharacterSelectDisplay _characterSelect;
        private CharacterData _character;
        
        public void SetCharacter(CharacterSelectDisplay characterSelect, CharacterData character)
        {
            iconImage.sprite = character.Icon;
            _characterSelect = characterSelect;
            _character = character;
        }

        public void SelectCharacter()
        {
            _characterSelect.Select(_character);
        }
    }
}
