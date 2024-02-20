using System;
using Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.XML
{
    public class MainMenuBehaviour
    {
        private readonly Button _preVisualize;
        private readonly Button _start;
        private readonly Button _exit;
        private readonly Button _server;
        
        public Action OpenLobby { get; internal set; }

        public MainMenuBehaviour(VisualElement root)
        {
            _preVisualize =  root.Q<Button>("Pre-Visualize");
            _start = root.Q<Button>("Start");
            _exit = root.Q<Button>("Exit");
            _server = root.Q<Button>("Server");
            SetupButtons();
        }

        private void SetupButtons()
        {
            _preVisualize.clicked += OnPreVisualizeClick;
            _start.clicked += OnStartClick;
            _exit.clicked += OnExitClick;
            _server.clicked += OnServerClick;
        }
        
        private void OnPreVisualizeClick()
        {
            StaticValues.OfflineMode = true;
            ServerManager.Instance.StartHost();
            ServerManager.Instance.OpenScene("OfflineNoradus");
        }

        private void OnStartClick()
        {
            //OpenLobby();
            StaticValues.OfflineMode = true;
            ServerManager.Instance.StartClient();
            //ServerManager.Instance.OpenScene("LobbyTempScene");
        }

        private void OnServerClick()
        {
            ServerManager.Instance.StartHost();
        }

        private void OnExitClick()
        {
            Application.Quit();
        }
        
        
    }
}
