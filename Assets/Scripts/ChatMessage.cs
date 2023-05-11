using TMPro;
using UnityEngine;

namespace FMGames.Scripts.Menu.Chat {
    public class ChatMessage : MonoBehaviour {
        [SerializeField] private TMP_Text textField;

        public void SetMessage(string playerName, string message) {
            textField.text = $"<color=grey>{playerName}</color>: {message}";
        }
    }
}