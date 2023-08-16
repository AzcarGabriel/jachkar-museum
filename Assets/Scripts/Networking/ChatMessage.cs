using TMPro;
using UnityEngine;
using Unity.Collections;


public class ChatMessage : MonoBehaviour {
    [SerializeField] private TMP_Text textField;

    public void SetMessage(FixedString32Bytes playerName, string message) {
        textField.text = $"<color=grey>{playerName}</color>: {message}";
    }
}