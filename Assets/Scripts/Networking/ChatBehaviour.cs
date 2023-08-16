using System.Collections.Generic;
// using FMGames.Scripts.Menu.Lobby;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CommandManager))]
public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform messageParent;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private ScrollRect scroll;

    
    private const int MaxNumberOfMessagesInList = 20;
    private List<ChatMessage> _messages;
    private const float MinIntervalBetweenChatMessages = 1f;
    private float _clientSendTimer;
    private CommandManager commandManager;

    private void Awake()
    {
        commandManager = GetComponent<CommandManager>();
    }
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
        if (message[0] == '/')
        {
            commandManager.ManageCommands(message);
        }
        FixedString32Bytes username = "Name";
        if (ServerManager.Instance.ClientData.TryGetValue(senderPlayerId, out ClientData clientData)) {
            username = clientData.username;
        }
        else {
            Debug.LogError("Couldn't find searched client data");
        }
        ReceiveChatMessageClientRpc(message, username);
    }
}