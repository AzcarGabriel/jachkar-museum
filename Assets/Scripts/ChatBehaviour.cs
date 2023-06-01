using System.Collections.Generic;
// using FMGames.Scripts.Menu.Lobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;

namespace FMGames.Scripts.Menu.Chat
{
    public class ChatBehaviour : NetworkBehaviour
    {
        [SerializeField] private ChatMessage chatMessagePrefab;
        [SerializeField] private Transform messageParent;
        [SerializeField] private TMP_InputField chatInputField;
        [SerializeField] private ScrollRect scroll;

        static public FixedString32Bytes username = "";

        private const int MaxNumberOfMessagesInList = 20;
        private List<ChatMessage> _messages;

        private const float MinIntervalBetweenChatMessages = 1f;
        private float _clientSendTimer;

        private void Start()
        {
            _messages = new List<ChatMessage>();
        }

        private void Update()
        {
            _clientSendTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (chatInputField.text.Length > 0 && _clientSendTimer > MinIntervalBetweenChatMessages)
                {
                    SendMessage();
                    chatInputField.DeactivateInputField(clearSelection: true);
                }
                else
                {
                    chatInputField.Select();
                    chatInputField.ActivateInputField();
                }
            }
        }

        public void SendMessage()
        {
            string message = chatInputField.text;
            chatInputField.text = "";

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _clientSendTimer = 0;
            SendChatMessageServerRpc(message, NetworkManager.Singleton.LocalClientId);
            scroll.verticalNormalizedPosition = 0;
        }

        private void AddMessage(string message, FixedString32Bytes senderPlayerUsername)
        {
            var msg = Instantiate(chatMessagePrefab, messageParent);
            // message = _profanityFilter.CensorString(message);
            
            
            msg.SetMessage(senderPlayerUsername, message);

            _messages.Add(msg);

            if (_messages.Count > MaxNumberOfMessagesInList)
            {
                Destroy(_messages[0]);
                _messages.RemoveAt(0);
            }
        }

        [ClientRpc]
        private void ReceiveChatMessageClientRpc(string message, FixedString32Bytes senderPlayerUsername)
        {
            AddMessage(message, senderPlayerUsername);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendChatMessageServerRpc(string message, ulong senderPlayerId)
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            PlayerObject playerObject = networkManager.ConnectedClients[senderPlayerId].PlayerObject.GetComponent<PlayerObject>();
            FixedString32Bytes name = playerObject.PlayerName;
            ReceiveChatMessageClientRpc(message, name);
        }
    }
}