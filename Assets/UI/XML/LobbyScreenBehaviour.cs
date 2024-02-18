using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.XML
{
    public class LobbyScreenBehaviour
    {
        private readonly Button _backButton;
        public Action OpenMainMenu { get; internal set; }

        public LobbyScreenBehaviour(VisualElement root)
        {
            _backButton = root.Q<Button>("BackButton");
            SetupButton();
        }

        private void SetupButton()
        {
            _backButton.clicked += OnBackClick;
        }

        private void OnBackClick()
        {
            OpenMainMenu();
        }
    }
}
